// =============================================================================
// JokerGames/RouletteWheel
//
// URP-compatible lit shader that rotates its base map UVs by an arbitrary
// angle (degrees) around a pivot. Drive _Rotation from C# via a Material-
// PropertyBlock to make the wheel face spin without rotating the mesh — the
// physical disc transform can stay still while the visible numbers rotate.
//
// Properties
//   _BaseMap     : The wheel face texture (numbers + colored pockets).
//   _BaseColor   : Tint multiplied with the texture sample.
//   _Rotation    : Degrees of UV rotation (positive = CW when looking down +Y).
//   _PivotU/_V   : UV-space pivot for the rotation, default centre (0.5, 0.5).
//   _Smoothness  : Cosmetic only in this simple lit pipeline; affects nothing
//                  beyond the inspector field — included for consistency with
//                  the URP/Lit shader in case you want to extend later.
// =============================================================================
Shader "JokerGames/RouletteWheel"
{
    Properties
    {
        _BaseMap   ("Base Map", 2D) = "white" {}
        _BaseColor ("Tint", Color) = (1, 1, 1, 1)
        _Rotation  ("Rotation (degrees)", Float) = 0.0
        _PivotU    ("Pivot U", Range(0, 1)) = 0.5
        _PivotV    ("Pivot V", Range(0, 1)) = 0.5
        _Smoothness("Smoothness", Range(0, 1)) = 0.4
    }

    SubShader
    {
        Tags
        {
            "RenderType"     = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "Queue"          = "Geometry"
        }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile_fragment _ _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                half4  _BaseColor;
                float  _Rotation;
                float  _PivotU;
                float  _PivotV;
                float  _Smoothness;
            CBUFFER_END

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS   : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float2 uv         : TEXCOORD2;
            };

            // Rotate a UV around 'pivot' by 'angleRad' radians.
            float2 RotateUV(float2 uv, float angleRad, float2 pivot)
            {
                float c = cos(angleRad);
                float s = sin(angleRad);
                uv -= pivot;
                uv = float2(uv.x * c - uv.y * s,
                            uv.x * s + uv.y * c);
                return uv + pivot;
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT = (Varyings)0;
                VertexPositionInputs vp = GetVertexPositionInputs(IN.positionOS.xyz);
                VertexNormalInputs   vn = GetVertexNormalInputs(IN.normalOS);

                OUT.positionCS = vp.positionCS;
                OUT.positionWS = vp.positionWS;
                OUT.normalWS   = vn.normalWS;
                OUT.uv         = TRANSFORM_TEX(IN.uv, _BaseMap);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // Degrees → radians, then rotate UV around the pivot.
                float angleRad = _Rotation * 0.017453293; // π / 180
                float2 uv = RotateUV(IN.uv, angleRad, float2(_PivotU, _PivotV));

                half4 baseSample = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv);
                half3 albedo    = baseSample.rgb * _BaseColor.rgb;

                // Simple lambert-ish shading: main directional light + spherical
                // harmonics ambient. No specular — the wheel face material
                // doesn't need it for the deliverable, and it keeps the shader
                // mobile-friendly.
                Light mainLight = GetMainLight();
                half3 N = normalize(IN.normalWS);
                half  NdotL = saturate(dot(N, mainLight.direction));
                half3 lit = albedo * mainLight.color * NdotL;
                half3 amb = albedo * SampleSH(N);

                return half4(lit + amb, baseSample.a * _BaseColor.a);
            }
            ENDHLSL
        }
    }

    Fallback "Universal Render Pipeline/Unlit"
}
