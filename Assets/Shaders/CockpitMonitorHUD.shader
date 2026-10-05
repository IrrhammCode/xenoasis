Shader "XENOASIS/CockpitMonitorHUD"
{
    Properties
    {
        _MainTex ("HUD Display Texture", 2D) = "black" {}
        _EmissionColor ("Screen Emission Tint", Color) = (1.0, 1.0, 1.0, 1.0)
        _EmissionGain ("Emission Gain / Brightness", Range(0.8, 3.0)) = 1.35
        _ScanlineIntensity ("Scanline Intensity", Range(0.0, 0.2)) = 0.04
        _ScanlineFreq ("Scanline Frequency", Float) = 280.0
        _ScanlineSpeed ("Scanline Speed", Float) = 2.0
        _FlickerAmount ("Screen Subtle Pulse", Range(0.0, 0.1)) = 0.025
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
            "RenderPipeline" = "UniversalPipeline"
        }
        LOD 100

        Pass
        {
            Name "CockpitMonitorForward"
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
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                half4 _EmissionColor;
                half _EmissionGain;
                half _ScanlineIntensity;
                float _ScanlineFreq;
                float _ScanlineSpeed;
                half _FlickerAmount;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                VertexPositionInputs posInputs = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = posInputs.positionCS;
                output.uv = input.uv;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                // Sample main HUD graphic
                half4 baseCol = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);

                // Very fine, high-tech holographic scanline
                float scan = sin(input.uv.y * _ScanlineFreq - _Time.y * _ScanlineSpeed) * 0.5 + 0.5;
                float scanMult = 1.0 - (scan * _ScanlineIntensity);

                // Subtle organic electrical pulse
                float pulse = 1.0 + sin(_Time.y * 3.5) * _FlickerAmount;

                // Emissive composite
                half3 finalColor = baseCol.rgb * _EmissionColor.rgb * _EmissionGain * scanMult * pulse;

                return half4(finalColor, 1.0);
            }
            ENDHLSL
        }
    }
    FallBack "Universal Render Pipeline/Unlit"
}
