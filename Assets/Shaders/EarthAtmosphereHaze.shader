Shader "XENOASIS/EarthAtmosphereHaze"
{
    Properties
    {
        _GlowColor ("Atmosphere Glow Color", Color) = (0.2, 0.7, 1.0, 1.0)
        _SunDir ("Sun Direction", Vector) = (0.6, 0.4, -0.7, 0)
        _Falloff ("Atmosphere Falloff Power", Range(1.0, 10.0)) = 3.5
        _Intensity ("Glow Intensity", Range(0.5, 6.0)) = 3.2
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent+10" "RenderPipeline"="UniversalPipeline" }
        LOD 100

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }
            Blend One One
            ZWrite Off
            Cull Front

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
                float3 positionWS   : TEXCOORD0;
                float3 normalWS     : TEXCOORD1;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _GlowColor;
                float4 _SunDir;
                float _Falloff;
                float _Intensity;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                VertexPositionInputs posInputs = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normInputs = GetVertexNormalInputs(input.normalOS);

                output.positionCS = posInputs.positionCS;
                output.positionWS = posInputs.positionWS;
                output.normalWS = normalize(normInputs.normalWS);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                // Inverted normal since we render Cull Front (looking at backfaces of the shell)
                float3 N = -normalize(input.normalWS);
                float3 V = normalize(_WorldSpaceCameraPos - input.positionWS);
                float3 L = normalize(_SunDir.xyz);

                // Inverted fresnel: bright at edges, zero in center
                float NdotV = saturate(dot(N, V));
                float fresnel = pow(1.0 - NdotV, _Falloff);

                float NdotL = saturate(dot(N, L) * 0.6 + 0.4);
                float3 glow = _GlowColor.rgb * fresnel * _Intensity * NdotL;

                return half4(glow, 1.0);
            }
            ENDHLSL
        }
    }
}
