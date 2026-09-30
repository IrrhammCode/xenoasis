Shader "XENOASIS/WaterSphere"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0.1, 0.6, 0.8, 0.4)
        _FresnelColor ("Fresnel Color", Color) = (0, 1, 0.82, 0.8)
        _FresnelPower ("Fresnel Power", Range(0.5, 8)) = 3.0
        _Distortion ("Distortion Amount", Range(0, 0.1)) = 0.02
        _WaveSpeed ("Wave Speed", Range(0, 5)) = 1.5
        _WaveScale ("Wave Scale", Range(0.1, 10)) = 3.0
        _Smoothness ("Smoothness", Range(0, 1)) = 0.95
        _EmissionIntensity ("Emission Intensity", Range(0, 5)) = 1.0
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Transparent"
        }
        LOD 200

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Back

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
                float3 viewDirWS : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
                float2 uv : TEXCOORD3;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                half4 _FresnelColor;
                half _FresnelPower;
                half _Distortion;
                half _WaveSpeed;
                half _WaveScale;
                half _Smoothness;
                half _EmissionIntensity;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;

                // Subtle vertex displacement for organic water feel
                float3 worldPos = TransformObjectToWorld(input.positionOS.xyz);
                float wave = sin(worldPos.x * _WaveScale + _Time.y * _WaveSpeed)
                           * cos(worldPos.z * _WaveScale + _Time.y * _WaveSpeed * 0.7)
                           * _Distortion;
                input.positionOS.xyz += input.normalOS * wave;

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = vertexInput.positionCS;
                output.positionWS = vertexInput.positionWS;
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.viewDirWS = GetWorldSpaceNormalizeViewDir(vertexInput.positionWS);
                output.uv = input.uv;

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                // Fresnel rim lighting (critical for glass/water look)
                half NdotV = saturate(dot(normalize(input.normalWS), normalize(input.viewDirWS)));
                half fresnel = pow(1.0 - NdotV, _FresnelPower);

                // Animated internal caustic-like pattern
                float caustic = sin(input.positionWS.x * 12.0 + _Time.y * 2.0)
                              * sin(input.positionWS.y * 12.0 - _Time.y * 1.5)
                              * sin(input.positionWS.z * 12.0 + _Time.y * 1.2);
                caustic = caustic * 0.5 + 0.5;

                // Combine
                half3 baseCol = _BaseColor.rgb;
                half3 fresnelCol = _FresnelColor.rgb * fresnel;
                half3 emission = fresnelCol * _EmissionIntensity + caustic * _FresnelColor.rgb * 0.15;

                half3 finalColor = baseCol + emission;
                half finalAlpha = _BaseColor.a + fresnel * 0.4;

                return half4(finalColor, saturate(finalAlpha));
            }
            ENDHLSL
        }
    }

    FallBack "Universal Render Pipeline/Lit"
}
