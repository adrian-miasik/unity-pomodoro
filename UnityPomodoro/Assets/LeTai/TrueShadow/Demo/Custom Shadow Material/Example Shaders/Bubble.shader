Shader "UI/Bubble"
{
    Properties
    {
        _Scale ("Scale", Float) = 80
        _Bubbling ("Bubbling", Float) = 25
        _Scroll ("Scroll", Vector) = (0, 1, 0, 0)

        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend One OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #include "../../../Resources/Shaders/TrueShadow.cginc"
            #pragma vertex vert
            #pragma fragment frag_custom

            #define mod(x, y) (x - y * floor(x / y))

            // Cellular noise ("Worley noise") in 3D in GLSL.
            // Copyright (c) Stefan Gustavson 2011-04-19. All rights reserved.
            // This code is released under the conditions of the MIT license.
            // See LICENSE file for details.

            // Permutation polynomial: (34x^2 + x) mod 289
            half4 permute(half4 x)
            {
                return mod((34.0 * x + 1.0) * x, 289.0);
            }

            half3 permute(half3 x)
            {
                return mod((34.0 * x + 1.0) * x, 289.0);
            }

            // Cellular noise, returning F1 and F2 in a half2.
            // Speeded up by using 2x2x2 search window instead of 3x3x3,
            // at the expense of some pattern artifacts.
            // F2 is often wrong and has sharp discontinuities.
            // If you need a good F2, use the slower 3x3x3 version.
            half2 cellular2x2x2(half3 P)
            {
                #define K 0.142857142857 // 1/7
                #define Ko 0.428571428571 // 1/2-K/2
                #define K2 0.020408163265306 // 1/(7*7)
                #define Kz 0.166666666667 // 1/6
                #define Kzo 0.416666666667 // 1/2-1/6*2
                #define jitter 0.8 // smaller jitter gives less errors in F2
                half3 Pi = mod(floor(P), 289.0);
                half3 Pf = frac(P);
                half4 Pfx = Pf.x + half4(0.0, -1.0, 0.0, -1.0);
                half4 Pfy = Pf.y + half4(0.0, 0.0, -1.0, -1.0);
                half4 p = permute(Pi.x + half4(0.0, 1.0, 0.0, 1.0));
                p = permute(p + Pi.y + half4(0.0, 0.0, 1.0, 1.0));
                half4 p1 = permute(p + Pi.z); // z+0
                half4 p2 = permute(p + Pi.z + 1.0); // z+1
                half4 ox1 = frac(p1 * K) - Ko;
                half4 oy1 = mod(floor(p1 * K), 7.0) * K - Ko;
                half4 oz1 = floor(p1 * K2) * Kz - Kzo; // p1 < 289 guaranteed
                half4 ox2 = frac(p2 * K) - Ko;
                half4 oy2 = mod(floor(p2 * K), 7.0) * K - Ko;
                half4 oz2 = floor(p2 * K2) * Kz - Kzo;
                half4 dx1 = Pfx + jitter * ox1;
                half4 dy1 = Pfy + jitter * oy1;
                half4 dz1 = Pf.z + jitter * oz1;
                half4 dx2 = Pfx + jitter * ox2;
                half4 dy2 = Pfy + jitter * oy2;
                half4 dz2 = Pf.z - 1.0 + jitter * oz2;
                half4 d1 = dx1 * dx1 + dy1 * dy1 + dz1 * dz1; // z+0
                half4 d2 = dx2 * dx2 + dy2 * dy2 + dz2 * dz2; // z+1

                // Sort out the two smallest distances (F1, F2)
                #if 1
                    // Cheat and sort out only F1
                    d1 = min(d1, d2);
                    d1.xy = min(d1.xy, d1.wz);
                    d1.x = min(d1.x, d1.y);
                    return sqrt(d1.xx);
                #else
	                // Do it right and sort out both F1 and F2
	                half4 d = min(d1,d2); // F1 is now in d
	                d2 = max(d1,d2); // Make sure we keep all candidates for F2
	                d.xy = (d.x < d.y) ? d.xy : d.yx; // Swap smallest to d.x
	                d.xz = (d.x < d.z) ? d.xz : d.zx;
	                d.xw = (d.x < d.w) ? d.xw : d.wx; // F1 is now in d.x
	                d.yzw = min(d.yzw, d2.yzw); // F2 now not in d2.yzw
	                d.y = min(d.y, d.z); // nor in d.z
	                d.y = min(d.y, d.w); // nor in d.w
	                d.y = min(d.y, d2.x); // F2 is now in d.y
	                return sqrt(d.xy); // F1 and F2
                #endif
            }

            half _Scale;
            half _Bubbling;
            half2 _Scroll;

            fixed4 frag_custom(v2f IN) : SV_Target
            {
                half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd);

                half2 resInvariantUV = IN.vertex.xy / ((_ScreenParams.x + _ScreenParams.y) / 2);
                half2 cell = cellular2x2x2(half3(resInvariantUV * _Scale + _Scroll * _Time.y, _Time.y * _Bubbling));
                half a2 = smoothstep(.2, .0, color.a);
                half n = smoothstep(0.5, 0.3, cell.x + a2);
                color.rgb = color.rgb / max(color.a, 1e-9) * IN.color.rgb * n;
                color.a = IN.color.a * n;

                #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif

                return color;
            }
            ENDCG
        }
    }
}
