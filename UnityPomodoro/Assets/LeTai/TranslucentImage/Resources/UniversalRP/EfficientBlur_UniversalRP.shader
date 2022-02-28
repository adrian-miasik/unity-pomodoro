Shader "Hidden/EfficientBlur_UniversalRP"
{
    HLSLINCLUDE
    #pragma target 3.0
    //HLSLcc is not used by default on gles
    #pragma prefer_hlslcc gles
    //SRP don't support dx9
    #pragma exclude_renderers d3d11_9x
    #pragma multi_compile_local _ PROCEDURAL_QUAD

    #ifdef SHADER_API_GLES
    #undef PROCEDURAL_QUAD
    #endif

    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "./lib.hlsl"

    TEXTURE2D_X(_MainTex);
    SAMPLER(sampler_MainTex);
    uniform half4 _MainTex_TexelSize;
    uniform half  _Radius;

    struct v2f
    {
        half4 vertex : SV_POSITION;
        half4 texcoord : TEXCOORD0;
        UNITY_VERTEX_OUTPUT_STEREO
    };

    v2f vert(minimalVertexInput v)
    {
        v2f o;
        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
        half4 pos;
        half2 uv;

#if PROCEDURAL_QUAD
        GetProceduralQuad(v.vertexID, pos, uv);
#else
        pos = v.position;
        uv = VertexToUV(v.position.xy);
#endif

        o.vertex = half4(pos.xy, 0.0, 1.0);

        half4 offset = half2(-0.5h, 0.5h).xxyy; //-x, -y, x, y
        offset *= UnityStereoAdjustedTexelSize(_MainTex_TexelSize).xyxy;
        offset *= _Radius;
        o.texcoord = uv.xyxy + offset;

        return o;
    }

    half4 frag(v2f i) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

        //Pray to the compiler god these will MAD
        half4 o =
            SAMPLE_SCREEN_TEX(_MainTex, i.texcoord.xw) / 4.0h;
        o += SAMPLE_SCREEN_TEX(_MainTex, i.texcoord.zw) / 4.0h;
        o += SAMPLE_SCREEN_TEX(_MainTex, i.texcoord.xy) / 4.0h;
        o += SAMPLE_SCREEN_TEX(_MainTex, i.texcoord.zy) / 4.0h;
        return o;
    }
    ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always Blend Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDHLSL
        }

        Pass
        {
            HLSLPROGRAM
            //Crop before blur
            #pragma vertex vertCrop
            #pragma fragment frag

            half4 _CropRegion;

            half2 getNewUV(half2 oldUV)
            {
                return lerp(_CropRegion.xy, _CropRegion.zw, oldUV);
            }

            v2f vertCrop(minimalVertexInput v)
            {
                v2f o = vert(v);

                o.texcoord.xy = getNewUV(o.texcoord.xy);
                o.texcoord.zw = getNewUV(o.texcoord.zw);

                return o;
            }
            ENDHLSL
        }
    }

    FallBack Off
}
