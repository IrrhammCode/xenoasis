Shader "XENOASIS/ObsidianFloor"
{
    Properties
    {
        _MainTex ("Albedo", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (0.04, 0.05, 0.08, 1)
        _VeinColor ("Vein Color", Color) = (0, 1, 0.82, 1)
        _VeinIntensity ("Vein Glow Intensity", Range(0, 5)) = 1.5
        _VeinSpeed ("Vein Pulse Speed", Range(0, 5)) = 0.8
        _VeinScale ("Vein Pattern Scale", Range(0.5, 20)) = 5.0
        _Smoothness ("Smoothness", Range(0, 1)) = 0.9
        _Metallic ("Metallic", Range(0, 1)) = 0.3
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" "Queue"="Geometry" }
        LOD 200

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                half4 _BaseColor;
                half4 _VeinColor;
                half _VeinIntensity;
                half _VeinSpeed;
                half _VeinScale;
                half _Smoothness;
                half _Metallic;
            CBUFFER_END

            // Simple procedural vein pattern
            float veinPattern(float2 uv, float scale, float time)
            {
                float2 p = uv * scale;
                float v1 = sin(p.x * 3.7 + time * 0.4) * cos(p.y * 2.3 - time * 0.3);
                float v2 = sin(p.x * 1.3 - p.y * 4.1 + time * 0.2);
                float v3 = cos(p.x * 5.2 + p.y * 1.7 + time * 0.5);
                float combined = (v1 + v2 + v3) / 3.0;
                // Sharpen into vein-like lines
                float vein = pow(saturate(1.0 - abs(combined)), 8.0);
                return vein;
            }

            Varyings vert(Attributes input)
            {
                Varyings output;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = vertexInput.positionCS;
                output.positionWS = vertexInput.positionWS;
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                half4 baseTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                half3 baseAlbedo = baseTex.rgb * _BaseColor.rgb;

                // Lighting
                Light mainLight = GetMainLight();
                half3 lightDir = normalize(mainLight.direction);
                half NdotL = saturate(dot(input.normalWS, lightDir));
                half3 diffuse = mainLight.color * NdotL * 0.5;
                half3 ambient = half3(0.04, 0.05, 0.09); // Ambient celestial starlight

                // Fresnel / Specular rim for polished obsidian stone
                half3 viewDir = normalize(GetCameraPositionWS() - input.positionWS);
                half fresnel = pow(1.0 - saturate(dot(input.normalWS, viewDir)), 4.0);
                half3 rim = half3(0.1, 0.25, 0.45) * fresnel * 0.8;

                // Procedural bioluminescent veins pulsing beneath the glass
                float vein = veinPattern(input.positionWS.xz, _VeinScale, _Time.y * _VeinSpeed);
                half pulse = 0.7 + 0.3 * sin(_Time.y * _VeinSpeed * 2.0);
                half3 emission = _VeinColor.rgb * vein * _VeinIntensity * pulse;

                // Glowing circular boundary ring at radius ~5.8m
                float distFromCenter = length(input.positionWS.xz);
                float rimRing = smoothstep(5.7, 5.85, distFromCenter) * (1.0 - smoothstep(5.95, 6.05, distFromCenter));
                half3 daisEdge = _VeinColor.rgb * rimRing * 2.0;

                half3 finalColor = baseAlbedo * (ambient + diffuse) + rim + emission + daisEdge;
                return half4(finalColor, 1.0);
            }
            ENDHLSL
        }
    }

    FallBack "Universal Render Pipeline/Lit"
}
