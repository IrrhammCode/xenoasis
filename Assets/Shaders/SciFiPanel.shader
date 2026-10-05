Shader "XENOASIS/SciFiPanel"
{
    Properties
    {
        _BaseColor ("Panel Color", Color) = (0.92, 0.94, 0.96, 1.0)
        _Metallic ("Metallic", Range(0, 1)) = 0.2
        _Smoothness ("Smoothness", Range(0, 1)) = 0.85
        _StripeColor ("Glow Strip Color", Color) = (0.0, 0.9, 1.0, 1.0)
        _StripeEmission ("Glow Strip Intensity", Float) = 2.5
        _StripeInterval ("Stripe Interval (Meters)", Float) = 2.0
        _StripeWidth ("Stripe Width (Meters)", Float) = 0.04
        [Toggle] _HorizontalStripes ("Horizontal (vs Vertical)", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
        }
        LOD 200

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile_fog

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
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float2 uv : TEXCOORD2;
                float fogFactor : TEXCOORD3;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float _Metallic;
                float _Smoothness;
                half4 _StripeColor;
                float _StripeEmission;
                float _StripeInterval;
                float _StripeWidth;
                float _HorizontalStripes;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS);

                output.positionCS = vertexInput.positionCS;
                output.positionWS = vertexInput.positionWS;
                output.normalWS = normalInput.normalWS;
                output.uv = input.uv;
                output.fogFactor = ComputeFogFactor(vertexInput.positionCS.z);

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float3 N = normalize(input.normalWS);
                Light mainLight = GetMainLight(TransformWorldToShadowCoord(input.positionWS));

                // Architectural emissive light stripe calculation in world space
                // Horizontal surfaces (floor and ceiling) are 100% clean solid surfaces (ZERO grid lines)
                float isStripe = 0.0;
                if (abs(N.y) > 0.5)
                {
                    isStripe = 0.0; // Strictly zero grid lines on floors and ceilings
                }
                else // Subtle vertical architectural wall accent line if enabled
                {
                    if (_StripeEmission > 0.01 && _StripeWidth > 0.001)
                    {
                        float coord = _HorizontalStripes > 0.5 ? input.positionWS.y : (input.positionWS.x + input.positionWS.z * 0.5);
                        float stripePattern = fmod(abs(coord), max(_StripeInterval, 0.1));
                        isStripe = step(stripePattern, _StripeWidth);
                    }
                }

                // Lighting
                float NdotL = saturate(dot(N, mainLight.direction));
                half3 diffuse = _BaseColor.rgb * (mainLight.color * (NdotL * mainLight.shadowAttenuation) + half3(0.12, 0.15, 0.20));

                // Emissive light strip
                half3 emission = _StripeColor.rgb * (_StripeEmission * isStripe);

                half3 finalColor = diffuse + emission;
                finalColor = MixFog(finalColor, input.fogFactor);

                return half4(finalColor, 1.0);
            }
            ENDHLSL
        }
    }
    FallBack "Universal Render Pipeline/Lit"
}
