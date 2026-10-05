Shader "XENOASIS/PlanetEarth3D"
{
    Properties
    {
        _MainTex ("Earth Albedo (Equirectangular)", 2D) = "white" {}
        _SunDir ("Sun Direction", Vector) = (0.6, 0.4, -0.7, 0)
        _SunColor ("Sun Light Color", Color) = (1.0, 0.98, 0.92, 1.0)
        _AmbientColor ("Space Ambient Color", Color) = (0.02, 0.03, 0.06, 1.0)
        _AtmosphereColor ("Atmosphere Rim Color", Color) = (0.15, 0.65, 1.0, 1.0)
        _AtmospherePower ("Atmosphere Fresnel Power", Range(1.0, 8.0)) = 2.8
        _AtmosphereIntensity ("Atmosphere Intensity", Range(0.5, 4.0)) = 2.2
        _OceanSpecPower ("Ocean Specular Power", Range(4.0, 64.0)) = 24.0
        _OceanSpecIntensity ("Ocean Specular Intensity", Range(0.0, 2.0)) = 0.85
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" "RenderPipeline"="UniversalPipeline" }
        LOD 200

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
                float3 positionWS   : TEXCOORD0;
                float3 normalWS     : TEXCOORD1;
                float2 uv           : TEXCOORD2;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _SunDir;
                float4 _SunColor;
                float4 _AmbientColor;
                float4 _AtmosphereColor;
                float _AtmospherePower;
                float _AtmosphereIntensity;
                float _OceanSpecPower;
                float _OceanSpecIntensity;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                VertexPositionInputs posInputs = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normInputs = GetVertexNormalInputs(input.normalOS);

                output.positionCS = posInputs.positionCS;
                output.positionWS = posInputs.positionWS;
                output.normalWS = normalize(normInputs.normalWS);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float3 N = normalize(input.normalWS);
                float3 V = normalize(_WorldSpaceCameraPos - input.positionWS);
                float3 L = normalize(_SunDir.xyz);
                float3 H = normalize(L + V);

                // Sample Earth texture
                half4 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);

                // Day / Night diffuse lighting with soft terminator
                float NdotL = dot(N, L);
                float dayFactor = smoothstep(-0.15, 0.25, NdotL);

                // Simple ocean detection: blue channel > red channel by significant margin
                float isOcean = saturate((albedo.b - albedo.r) * 2.5);
                float NdotH = saturate(dot(N, H));
                float spec = pow(NdotH, _OceanSpecPower) * isOcean * _OceanSpecIntensity * dayFactor;

                // Atmospheric Rayleigh scattering rim (fresnel glow along planetary horizon)
                float NdotV = saturate(dot(N, V));
                float fresnel = pow(1.0 - NdotV, _AtmospherePower);
                
                // Atmosphere glows brightest on sunlit side, with subtle glow around whole rim
                float atmoSun = saturate(NdotL * 0.5 + 0.5);
                float3 atmoGlow = _AtmosphereColor.rgb * fresnel * _AtmosphereIntensity * (atmoSun * 0.75 + 0.25);

                // Base surface color
                float3 dayColor = albedo.rgb * _SunColor.rgb * (dayFactor * 0.95 + 0.05) + spec * _SunColor.rgb;
                float3 nightColor = albedo.rgb * _AmbientColor.rgb;
                float3 surfaceColor = lerp(nightColor, dayColor, dayFactor);

                // Final composite with atmospheric scattering rim
                float3 finalColor = surfaceColor + atmoGlow;

                return half4(finalColor, 1.0);
            }
            ENDHLSL
        }
    }
    FallBack "Universal Render Pipeline/Lit"
}
