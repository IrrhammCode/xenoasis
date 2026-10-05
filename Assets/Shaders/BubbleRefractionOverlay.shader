Shader "XENOASIS/BubbleRefractionOverlay"
{
    Properties
    {
        _TintColor ("Water Tint Color", Color) = (0.05, 0.45, 0.65, 0.4)
        _Distortion ("Distortion Strength", Range(0, 2)) = 0.0
        _Opacity ("Overlay Opacity", Range(0, 1)) = 0.0
        _VignetteStrength ("Vignette Rim", Range(0, 2)) = 1.2
        _WaveFreq ("Wave Frequency", Range(1, 20)) = 8.0
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Overlay"
        }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        ZTest Always
        Cull Off

        Pass
        {
            Name "BubbleOverlayPass"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _TintColor;
                half _Distortion;
                half _Opacity;
                half _VignetteStrength;
                half _WaveFreq;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float2 uv = input.uv;
                float2 center = float2(0.5, 0.5);
                float2 d = uv - center;
                float r = length(d);

                // Spherical Fisheye Bubble Warping
                float distortionFactor = pow(r * 1.4, 2.0) * _Distortion;
                float2 distortedUV = center + d * (1.0 + distortionFactor);

                // Ripple rings
                float ripple = sin(r * _WaveFreq - _Time.y * 4.0) * 0.05 * _Distortion;

                // Vignette at edges
                float vignette = smoothstep(0.2, 0.7, r) * _VignetteStrength;

                half4 finalColor = _TintColor;
                finalColor.rgb += ripple * half3(0.1, 0.8, 0.9);
                finalColor.a = saturate((vignette + _TintColor.a) * _Opacity);

                return finalColor;
            }
            ENDHLSL
        }
    }
    FallBack Off
}
