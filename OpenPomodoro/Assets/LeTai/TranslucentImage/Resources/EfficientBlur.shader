Shader "Hidden/EfficientBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }

    CGINCLUDE
    #include "UnityCG.cginc"
    #include "lib.cginc"

    UNITY_DECLARE_SCREENSPACE_TEXTURE(_MainTex);
    half4 _MainTex_TexelSize;

    uniform half _Radius;

    struct v2f
    {
        half4 vertex : SV_POSITION;
        half4 texcoord : TEXCOORD0;
        UNITY_VERTEX_OUTPUT_STEREO
    };


    v2f vert(appdata_img v)
    {
        v2f o;
        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_INITIALIZE_OUTPUT(v2f, o);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
        o.vertex = UnityObjectToClipPos(v.vertex);

        half4 offset = half2(-0.5h, 0.5h).xxyy; //-x, -y, x, y
        offset *= UnityStereoAdjustedTexelSize(_MainTex_TexelSize).xyxy;
        offset *= _Radius;
        o.texcoord = v.texcoord.xyxy + offset;

        return o;
    }

    half4 frag(v2f i) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i)

        //Pray to the compiler god these will MAD
        half4 o =
            SAMPLE_SCREEN_TEX(_MainTex, i.texcoord.xw) / 4.0h;
        o += SAMPLE_SCREEN_TEX(_MainTex, i.texcoord.zw) / 4.0h;
        o += SAMPLE_SCREEN_TEX(_MainTex, i.texcoord.xy) / 4.0h;
        o += SAMPLE_SCREEN_TEX(_MainTex, i.texcoord.zy) / 4.0h;

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

        Pass
        {
            CGPROGRAM
            //Crop before blur
            #pragma vertex vertCrop
            #pragma fragment frag

            half4 _CropRegion;

            half2 getNewUV(half2 oldUV)
            {
                return lerp(_CropRegion.xy, _CropRegion.zw, oldUV);
            }

            v2f vertCrop(appdata_img v)
            {
                v2f o = vert(v);

                o.texcoord.xy = getNewUV(o.texcoord.xy);
                o.texcoord.zw = getNewUV(o.texcoord.zw);

                return o;
            }
            ENDCG
        }
    }

    FallBack Off
}
