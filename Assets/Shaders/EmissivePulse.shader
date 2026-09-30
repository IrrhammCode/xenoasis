Shader "XENOASIS/EmissivePulse"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (0.02, 0.03, 0.05, 1)
        _EmissionColor ("Emission Color", Color) = (0, 1, 0.82, 1)
        _EmissionIntensity ("Emission Intensity", Range(0, 10)) = 1.5
        _PulseSpeed ("Pulse Speed", Range(0, 10)) = 2.0
        _PulseAmplitude ("Pulse Amplitude", Range(0, 1)) = 0.3
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
            #pragma multi_compile_fog

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
                float3 normalWS : TEXCOORD1;
                float fogFactor : TEXCOORD2;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                half4 _BaseColor;
                half4 _EmissionColor;
                half _EmissionIntensity;
                half _PulseSpeed;
                half _PulseAmplitude;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = vertexInput.positionCS;
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.fogFactor = ComputeFogFactor(vertexInput.positionCS.z);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                half4 baseTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                half4 baseColor = baseTex * _BaseColor;

                // Pulsing emission
                half pulse = 1.0 + sin(_Time.y * _PulseSpeed) * _PulseAmplitude;
                half3 emission = _EmissionColor.rgb * _EmissionIntensity * pulse;

                half4 finalColor = half4(baseColor.rgb + emission, baseColor.a);
                finalColor.rgb = MixFog(finalColor.rgb, input.fogFactor);

                return finalColor;
            }
            ENDHLSL
        }
    }

    FallBack "Universal Render Pipeline/Lit"
}
