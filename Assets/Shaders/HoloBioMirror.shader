Shader "XENOASIS/HoloBioMirror"
{
    Properties
    {
        _ReflectionTex ("Reflection Texture", 2D) = "black" {}
        _TintColor ("Holographic Tint", Color) = (0.96, 0.98, 1.0, 1.0)
        _RimColor ("Rim Emissive Color", Color) = (0.0, 0.85, 1.0, 0.25)
        _RimPower ("Rim Power", Range(0.5, 6.0)) = 3.5
        _ScanlineIntensity ("Scanline Intensity", Range(0.0, 0.05)) = 0.008
        _ScanlineFreq ("Scanline Frequency", Float) = 90.0
        _MirrorClarity ("Mirror Clarity", Range(0.5, 1.3)) = 1.05
        _UseScreenSpaceUV ("Use Screen Space UV (1=Physical Mirror, 0=Quad UV)", Float) = 1.0
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
            "RenderPipeline" = "UniversalPipeline"
        }
        LOD 200

        Pass
        {
            Name "BioMirrorForward"
            Tags { "LightMode" = "UniversalForward" }

            ZWrite On
            ZTest LEqual
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 screenPos : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 viewDirWS : TEXCOORD2;
                float2 uv : TEXCOORD3;
            };

            TEXTURE2D(_ReflectionTex);
            SAMPLER(sampler_ReflectionTex);

            CBUFFER_START(UnityPerMaterial)
                half4 _TintColor;
                half4 _RimColor;
                half _RimPower;
                half _ScanlineIntensity;
                float _ScanlineFreq;
                half _MirrorClarity;
                float _UseScreenSpaceUV;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                VertexPositionInputs posInputs = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normInputs = GetVertexNormalInputs(input.normalOS);

                output.positionCS = posInputs.positionCS;
                output.screenPos = ComputeScreenPos(output.positionCS);
                output.normalWS = normInputs.normalWS;
                output.viewDirWS = GetWorldSpaceNormalizeViewDir(posInputs.positionWS);
                output.uv = input.uv;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float2 reflUV;
                if (_UseScreenSpaceUV > 0.5)
                {
                    // Physical 1:1 Screen-space planar reflection with horizontal mirror flip
                    reflUV = input.screenPos.xy / max(0.0001, input.screenPos.w);
                    reflUV.x = 1.0 - reflUV.x;
                }
                else
                {
                    // Direct Quad UV mapping
                    reflUV = input.uv;
                }

                // Sample planar reflection texture
                half4 reflColor = SAMPLE_TEXTURE2D(_ReflectionTex, sampler_ReflectionTex, reflUV);

                // Very subtle high-tech holographic scanline
                float scanline = sin(input.uv.y * _ScanlineFreq + _Time.y * 2.5) * 0.5 + 0.5;
                scanline = smoothstep(0.4, 0.6, scanline) * _ScanlineIntensity;

                // Subtle edge rim / glass fresnel
                float NdotV = saturate(dot(normalize(input.normalWS), normalize(input.viewDirWS)));
                float rim = pow(1.0 - NdotV, _RimPower);

                // Combined color: crystal clear reflection + ultra-subtle scanlines + edge rim
                half3 finalColor = reflColor.rgb * _TintColor.rgb * _MirrorClarity;
                finalColor += scanline * half3(0.2, 0.8, 1.0);
                finalColor += rim * _RimColor.rgb * 0.3;

                return half4(finalColor, 1.0);
            }
            ENDHLSL
        }
    }
    FallBack "Universal Render Pipeline/Unlit"
}
