Shader "Hidden/TrueShadow/ImprintPostProcess"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_local __ BLEACH
            #pragma multi_compile_local __ INSET

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

            sampler2D _MainTex;

            half4 frag(v2f i) : SV_Target
            {
                #if BLEACH
                    half4 color = half4(1,1,1, tex2D(_MainTex, i.uv).a);
                #else
                    half4 color = tex2D(_MainTex, i.uv);
                #endif

                #if INSET
                    color.a = 1 - color.a;
                #endif

                color.rgb*=color.a;
                return color;
            }
            ENDCG
        }
    }
}
