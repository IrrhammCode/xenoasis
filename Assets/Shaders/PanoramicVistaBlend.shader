Shader "XENOASIS/PanoramicVistaBlend"
{
    Properties
    {
        _MainTex ("Current Vista", 2D) = "white" {}
        _BlendTex ("Target Vista", 2D) = "white" {}
        _BlendFactor ("Blend Factor", Range(0, 1)) = 0.0
        _Exposure ("Exposure", Range(0.1, 4.0)) = 1.0
        _Tint ("Tint Color", Color) = (1, 1, 1, 1)
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "Queue" = "Geometry-10"
            "RenderPipeline" = "UniversalPipeline"
        }
        LOD 100
        Cull Off
        ZWrite On

        Pass
        {
            Name "UnlitVista"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float fogFactor : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_BlendTex);
            SAMPLER(sampler_BlendTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float _BlendFactor;
                float _Exposure;
                half4 _Tint;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = vertexInput.positionCS;
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.fogFactor = ComputeFogFactor(vertexInput.positionCS.z);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                half4 colA = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                half4 colB = SAMPLE_TEXTURE2D(_BlendTex, sampler_BlendTex, input.uv);

                half4 finalColor = lerp(colA, colB, _BlendFactor);
                finalColor.rgb *= _Exposure * _Tint.rgb;

                finalColor.rgb = MixFog(finalColor.rgb, input.fogFactor);
                return half4(finalColor.rgb, 1.0);
            }
            ENDHLSL
        }
    }
    FallBack "Universal Render Pipeline/Unlit"
}
