Shader "Hidden/TrueShadow/Cutout"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Offset ("Offset", Vector) = (0,0,0,0)
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Blend Zero OneMinusSrcAlpha, Zero OneMinusSrcAlpha
        BlendOp Add, Add
//        ColorMask a

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            uniform sampler2D _MainTex;
            uniform float2 _Offset;
            uniform float _OverflowAlpha;

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv + _Offset;

                if (any(uv > 1) || any(uv < 0))
                    return _OverflowAlpha;

                half cutout = tex2D(_MainTex, uv).a;
                cutout *= cutout;

                return fixed4(0, 0, 0, cutout);
            }
            ENDCG
        }
    }
}
