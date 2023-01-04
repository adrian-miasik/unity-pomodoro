Shader "Hidden/TrueShadow/PostProcess"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Offset ("Offset", Vector) = (0,0,0,0)
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

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

            uniform sampler2D _ShadowTex;
            uniform sampler2D _MainTex;
            uniform float2 _Offset;
            uniform float _OverflowAlpha;
            uniform float _AlphaMultiplier;

            fixed4 frag(v2f i) : SV_Target
            {
                half4 shadow = tex2D(_ShadowTex, i.uv);

                half alpha = shadow.a;
                half alphaBoosted = saturate(alpha * _AlphaMultiplier);
                shadow.a = alphaBoosted;
                shadow.rgb = shadow.rgb / alpha * alphaBoosted;

                float cutOut;
                float2 cutoutUv = i.uv + _Offset;
                if (any(cutoutUv > 1) || any(cutoutUv < 0))
                    cutOut = _OverflowAlpha;
                else
                    cutOut = tex2D(_MainTex, cutoutUv).a;

                shadow *= saturate(1 - cutOut*cutOut);

                return shadow;
            }
            ENDCG
        }
    }
}
