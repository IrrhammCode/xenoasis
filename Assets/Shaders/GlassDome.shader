Shader "XENOASIS/GlassDome"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0.1, 0.15, 0.2, 0.15)
        _FresnelPower ("Fresnel Power", Float) = 3.0
        _FresnelColor ("Fresnel Color", Color) = (0, 1, 0.85, 1)
        _GridScale ("Hex Grid Scale", Float) = 8.0
        _GridLineWidth ("Grid Line Width", Float) = 0.05
        _GridEmission ("Grid Emission Intensity", Float) = 2.0
        _GridColor ("Grid Color", Color) = (0, 1, 0.85, 1)
        [KeywordEnum(WorldXZ, ObjectXZ, Spherical)] _GridMode ("Grid Coordinates", Float) = 0
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
        Cull Off

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma multi_compile_local _GRIDMODE_WORLDXZ _GRIDMODE_OBJECTXZ _GRIDMODE_SPHERICAL

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 viewDirWS : TEXCOORD2;
                float3 positionOS : TEXCOORD3;
                float2 uv : TEXCOORD4;
                float fogFactor : TEXCOORD5;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float _FresnelPower;
                half4 _FresnelColor;
                float _GridScale;
                float _GridLineWidth;
                float _GridEmission;
                half4 _GridColor;
                float _GridMode;
            CBUFFER_END

            // Evaluates hexagonal distance to edge
            // A regular hexagon centered at the origin has its boundary at d = 0.5.
            // Returns distance to closest edge: >0 inside the hex, <=0 outside/at the edge.
            float HexDistanceToEdge(float2 p)
            {
                const float2 r = float2(1.0, 1.7320508);
                const float2 h = r * 0.5;

                // Symmetrical wrapping across grid cells
                float2 a = (p - r * floor(p / r)) - h;
                float2 b = ((p - h) - r * floor((p - h) / r)) - h;

                // Pick closest center
                float2 gv = dot(a, a) < dot(b, b) ? a : b;

                // Compute distance to edge
                float2 absGv = abs(gv);
                float d = max(absGv.x, dot(absGv, float2(0.5, 0.8660254)));
                return 0.5 - d;
            }

            Varyings vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS);

                output.positionCS = vertexInput.positionCS;
                output.positionWS = vertexInput.positionWS;
                output.positionOS = input.positionOS.xyz;
                output.normalWS = normalInput.normalWS;
                output.viewDirWS = GetWorldSpaceNormalizeViewDir(vertexInput.positionWS);
                output.uv = input.uv;
                output.fogFactor = ComputeFogFactor(vertexInput.positionCS.z);

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                // 1. Fresnel effect based on view-normal angle: pow(1 - abs(dot(N, V)), _FresnelPower)
                float3 N = normalize(input.normalWS);
                float3 V = normalize(input.viewDirWS);
                float NdotV = abs(dot(N, V));
                float fresnel = pow(1.0 - NdotV, max(_FresnelPower, 0.001));

                half3 fresnelRim = _FresnelColor.rgb * fresnel;
                half fresnelAlpha = _FresnelColor.a * fresnel;

                // 2. Crystal Clear Architectural Glass (Zero wireframe lines)
                half3 gridEmission = half3(0, 0, 0);
                half gridAlpha = 0.0;

                // 3. Final color = base transparent + fresnel rim + grid emission
                half3 finalColor = _BaseColor.rgb + fresnelRim * 0.5 + gridEmission;
                half finalAlpha = saturate(_BaseColor.a + fresnelAlpha * 0.25 + gridAlpha);

                // Apply fog
                finalColor = MixFog(finalColor, input.fogFactor);

                return half4(finalColor, finalAlpha);
            }
            ENDHLSL
        }
    }

    FallBack "Universal Render Pipeline/Lit"
}
