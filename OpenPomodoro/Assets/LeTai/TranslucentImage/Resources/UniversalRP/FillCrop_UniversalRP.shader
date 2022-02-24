Shader "Hidden/FillCrop_UniversalRP"
{
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM
            #pragma target 3.0
            //HLSLcc is not used by default on gles
            #pragma prefer_hlslcc gles
            //SRP don't support dx9
            #pragma exclude_renderers d3d11_9x

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "./lib.hlsl"

            minimalVertexOutput vert(minimalVertexInput v)
            {
                minimalVertexOutput o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                half4 pos;
                half2 uv;

#if PROCEDURAL_QUAD
                GetProceduralQuad(v.vertexID, pos, uv);
                o.position.zw = half2(0, 1);
#else
                pos = v.position;
                uv = VertexToUV(v.position.xy);
#endif

                o.position = pos;
                o.texcoord = uv;
                return o;
            }

            TEXTURE2D_X(_MainTex);
            float4 _CropRegion;

            half2 getCroppedCoord(half2 screenCoord)
            {
                return (screenCoord - _CropRegion.xy) / (_CropRegion.zw - _CropRegion.xy);
            }

            half4 frag(minimalVertexOutput v) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(v)
                return SAMPLE_SCREEN_TEX(_MainTex, getCroppedCoord(v.texcoord));
            }
            ENDHLSL
        }
    }
}
