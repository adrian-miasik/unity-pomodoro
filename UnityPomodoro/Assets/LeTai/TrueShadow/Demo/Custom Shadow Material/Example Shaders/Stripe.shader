Shader "UI/Stripe"
{
    Properties
    {
        _Angle ("Angle", Float) = 45
        _StripeCount ("Stripe Count", Float) = 60
        _Speed ("Speed", Float) = -.1
        _ColorA ("ColorA", Color) = (0,0,0,1)
        _ColorB ("ColorB", Color) = (1,1,1,1)

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

            inline half2 rot2d(half2 dir, half angle)
            {
                half s, c;
                sincos(angle, s, c);
                return half2(c * dir.x - s * dir.y,
                              s * dir.x + c * dir.y);
            }

            half _Angle;
            half _StripeCount;
            half _Speed;

            half stripe(half2 p)
            {
                p.y += _Speed * _Time.y;
                p = rot2d(p, radians(_Angle));
                half stripe = frac(p.y * _StripeCount / 2);
                return stripe < .7 ? smoothstep(stripe, stripe + .05, .5) : 1. - smoothstep(stripe, stripe + .05, .95);
            }

            half4 _ColorA;
            half4 _ColorB;

            fixed4 frag_custom(v2f IN) : SV_Target
            {
                half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;

                half2 resInvariantUV = IN.vertex.xy / ((_ScreenParams.x + _ScreenParams.y) / 2);
                color *= lerp(_ColorA, _ColorB, stripe(resInvariantUV));

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
