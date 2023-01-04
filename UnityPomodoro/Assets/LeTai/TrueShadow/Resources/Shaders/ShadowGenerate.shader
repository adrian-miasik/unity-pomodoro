Shader "Hidden/TrueShadow/Generate"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }

    CGINCLUDE
    #include "UnityCG.cginc"

    sampler2D _MainTex;
    half4     _MainTex_TexelSize;
    half4     _MainTex_ST;

    uniform half _Radius;

    struct v2f
    {
        half4 vertex : SV_POSITION;
        half4 texcoord : TEXCOORD0;
    };

    v2f vert(appdata_img v)
    {
        v2f o;
        o.vertex = UnityObjectToClipPos(v.vertex);

        half4 offset = half2(-0.5h, 0.5h).xxyy; //-x, -y, x, y
        offset *= _MainTex_TexelSize.xyxy;
        offset *= _Radius;
        o.texcoord = v.texcoord.xyxy + offset;

        return o;
    }

    half4 frag(v2f i) : SV_Target
    {
        //Pray to the compiler god these will MAD
        half4 o =
            tex2D(_MainTex, i.texcoord.xw) / 4.0h;
        o += tex2D(_MainTex, i.texcoord.zw) / 4.0h;
        o += tex2D(_MainTex, i.texcoord.xy) / 4.0h;
        o += tex2D(_MainTex, i.texcoord.zy) / 4.0h;

        return o;
    }
    ENDCG

    SubShader
    {
        Cull Off ZWrite Off ZTest Always Blend Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }

        // +Crop
        Pass
        {
            CGPROGRAM
            #pragma vertex vertCrop
            #pragma fragment frag

            half4 _CropRegion;

            half2 applyCrop(half2 oldUV)
            {
                return lerp(_CropRegion.xy, _CropRegion.zw, oldUV);
            }

            v2f vertCrop(appdata_img v)
            {
                v2f o = vert(v);

                o.texcoord.xy = applyCrop(o.texcoord.xy);
                o.texcoord.zw = applyCrop(o.texcoord.zw);

                return o;
            }
            ENDCG
        }

        // +Dither
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragDither

            sampler2D _BlueNoise;
            half4     _BlueNoise_TexelSize;
            half2     _TargetSize;

            float IGN(float2 pos)
            {
                float3 k = {0.06711056, 0.00583715, 52.9829189};
                return frac(k.z * frac(dot(pos, k.xy)));
            }

            half4 fragDither(v2f i) : SV_Target
            {
                half4 o = frag(i);

                half2 noiseUv = (i.texcoord.xy - .5) * _TargetSize;
                half noise = tex2D(_BlueNoise, noiseUv * _BlueNoise_TexelSize.xy).r;
                noise -= .5;
                noise *= 1. / 256.;

                o += noise;

                return o;
            }
            ENDCG
        }
    }

    FallBack Off
}
