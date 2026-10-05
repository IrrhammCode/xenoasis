Shader "XENOASIS/BiomeSkyboxBlend"
{
    Properties
    {
        _MainTex ("Current Skybox (Deep Space with Stars)", 2D) = "white" {}
        _StarlessTex ("Starless Space Skybox", 2D) = "black" {}
        _BlendTex ("Target Skybox (Earth Horizon)", 2D) = "white" {}
        _BlendFactor ("Blend Factor", Range(0, 1)) = 0.0
        _StarIntensity ("Star Intensity", Range(0, 1)) = 1.0
        _Exposure ("Exposure", Range(0, 8)) = 1.0
        _Tint ("Tint Color", Color) = (1, 1, 1, 1)
        _AtmoColor ("Atmospheric Wash Color", Color) = (0, 0, 0, 0)
        [Gamma] _Rotation ("Rotation", Range(0, 360)) = 0.0
    }
    SubShader
    {
        Tags { "Queue" = "Background" "RenderType" = "Background" "PreviewType" = "Skybox" }
        Cull Off ZWrite Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _StarlessTex;
            sampler2D _BlendTex;
            uniform half _BlendFactor;
            uniform half _StarIntensity;
            uniform half _Exposure;
            uniform half4 _Tint;
            uniform half4 _AtmoColor;
            uniform float _Rotation;

            struct appdata { float4 vertex : POSITION; UNITY_VERTEX_INPUT_INSTANCE_ID };
            struct v2f { float4 vertex : SV_POSITION; float3 viewDir : TEXCOORD0; UNITY_VERTEX_OUTPUT_STEREO };

            float3 RotateAroundYInDegrees(float3 v, float degrees)
            {
                float alpha = degrees * (UNITY_PI / 180.0);
                float sina, cosa;
                sincos(alpha, sina, cosa);
                float2x2 m = float2x2(cosa, -sina, sina, cosa);
                return float3(mul(m, v.xz), v.y).xzy;
            }

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                float3 rotated = RotateAroundYInDegrees(v.vertex.xyz, _Rotation);
                o.vertex = UnityObjectToClipPos(float4(rotated, 1.0));
                o.viewDir = rotated;
                return o;
            }

            inline float2 DirectionToEquirectUV(float3 dir)
            {
                float3 n = normalize(dir);
                float latitude = acos(clamp(n.y, -1.0, 1.0));
                float longitude = atan2(n.z, n.x);
                float2 sphereCoords = float2(longitude, latitude) * float2(0.5 / UNITY_PI, 1.0 / UNITY_PI);
                return float2(0.5, 1.0) - sphereCoords;
            }

            inline float2 FixUVWrap(float2 d)
            {
                d.x = d.x - round(d.x);
                d.y = d.y - round(d.y);
                return d;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                float2 uv = DirectionToEquirectUV(i.viewDir);
                float2 dx = FixUVWrap(ddx(uv));
                float2 dy = FixUVWrap(ddy(uv));

                half4 texSpace = tex2Dgrad(_MainTex, uv, dx, dy);
                half4 texStarless = tex2Dgrad(_StarlessTex, uv, dx, dy);
                half4 texEarth = tex2Dgrad(_BlendTex, uv, dx, dy);

                // Cosmic space with controllable star intensity
                // If starless texture is assigned, lerp cleanly between starless and starry space
                // Fallback: clamp luminance if starless texture is empty/black
                half3 cStarless = texStarless.rgb;
                half starlessLum = dot(cStarless, half3(0.299, 0.587, 0.114));
                if (starlessLum < 0.001) {
                    cStarless = min(texSpace.rgb, half3(0.04, 0.05, 0.09));
                }

                half starFactor = saturate(_StarIntensity);
                half3 cSpace = lerp(cStarless, texSpace.rgb, starFactor);
                // Hard kill-switch: if starFactor == 0, strictly enforce zero star luminance peaks
                if (starFactor <= 0.001) {
                    cSpace = min(cSpace, half3(0.04, 0.05, 0.09));
                }

                // Blend between cosmic space (starless in atmosphere) and Earth daylight skybox
                half3 blended = lerp(cSpace, texEarth.rgb, saturate(_BlendFactor));

                // Layered atmospheric ionization, plasma glow, or tropospheric clouds
                half atmoAlpha = saturate(_AtmoColor.a);
                blended = lerp(blended, _AtmoColor.rgb, atmoAlpha);

                blended *= _Exposure * _Tint.rgb;
                return fixed4(blended, 1.0);
            }
            ENDCG
        }
    }
    Fallback Off
}
