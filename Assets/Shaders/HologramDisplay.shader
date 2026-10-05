Shader "XENOASIS/HologramDisplay"
{
    Properties
    {
        _MainColor ("Hologram Color", Color) = (0.0, 0.85, 1.0, 0.7)
        _RimColor ("Rim Color", Color) = (0.3, 1.0, 0.9, 1.0)
        _RimPower ("Rim Power", Range(0.5, 8.0)) = 2.5
        _ScanlineFreq ("Scanline Frequency", Float) = 40.0
        _ScanlineSpeed ("Scanline Speed", Float) = 2.0
        _FlickerSpeed ("Flicker Speed", Float) = 8.0
        _FlickerIntensity ("Flicker Intensity", Range(0.0, 0.5)) = 0.08
        _GlitchAmount ("Glitch Jitter", Range(0.0, 0.05)) = 0.005
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }
        LOD 100

        Blend SrcAlpha One
        ZWrite Off
        Cull Back

        Pass
        {
            Name "HologramPass"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

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
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 viewDirWS : TEXCOORD2;
                float2 uv : TEXCOORD3;
                float fogFactor : TEXCOORD4;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _MainColor;
                half4 _RimColor;
                float _RimPower;
                float _ScanlineFreq;
                float _ScanlineSpeed;
                float _FlickerSpeed;
                float _FlickerIntensity;
                float _GlitchAmount;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;

                // Subtle holographic glitch displacement
                float glitch = sin(_Time.y * _FlickerSpeed * 2.0 + input.positionOS.y * 10.0);
                float3 posOS = input.positionOS.xyz;
                if (frac(glitch * 13.37) > 0.92)
                {
                    posOS.x += sin(_Time.y * 30.0) * _GlitchAmount;
                }

                VertexPositionInputs vertexInput = GetVertexPositionInputs(posOS);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS);

                output.positionCS = vertexInput.positionCS;
                output.positionWS = vertexInput.positionWS;
                output.normalWS = normalInput.normalWS;
                output.viewDirWS = GetWorldSpaceNormalizeViewDir(vertexInput.positionWS);
                output.uv = input.uv;
                output.fogFactor = ComputeFogFactor(vertexInput.positionCS.z);

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float3 N = normalize(input.normalWS);
                float3 V = normalize(input.viewDirWS);

                // 1. Fresnel Rim
                float NdotV = saturate(dot(N, V));
                float rim = pow(1.0 - NdotV, _RimPower);

                // 2. Scanlines
                float scanline = sin(input.positionWS.y * _ScanlineFreq - _Time.y * _ScanlineSpeed);
                scanline = scanline * 0.5 + 0.5;
                scanline = pow(scanline, 2.0);

                // 3. Flicker
                float flicker = 1.0 - (sin(_Time.y * _FlickerSpeed) * 0.5 + 0.5) * _FlickerIntensity;

                // 4. Combine
                half3 col = _MainColor.rgb * (0.35 + scanline * 0.65) * flicker;
                col += _RimColor.rgb * rim * 1.5;

                half alpha = (_MainColor.a * (0.2 + scanline * 0.5) + rim * 0.8) * flicker;
                alpha = saturate(alpha);

                col = MixFog(col, input.fogFactor);
                return half4(col, alpha);
            }
            ENDHLSL
        }
    }
}
