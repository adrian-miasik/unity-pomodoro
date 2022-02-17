#include <UnityCG.cginc>

#define SAMPLE_SCREEN_TEX(tex, uv) UNITY_SAMPLE_SCREENSPACE_TEXTURE(tex, UnityStereoTransformScreenSpaceTex(uv))

#if defined(UNITY_SINGLE_PASS_STEREO)
float4 UnityStereoAdjustedTexelSize(float4 texelSize)
{
    texelSize.x = texelSize.x * 2.0; // texelSize.x = 1/w. For a double-wide texture, the true resolution is given by 2/w.
    texelSize.z = texelSize.z * 0.5; // texelSize.z = w. For a double-wide texture, the true size of the eye texture is given by w/2.
    return texelSize;
}
#else
float4 UnityStereoAdjustedTexelSize(float4 texelSize)
{
    return texelSize;
}
#endif

half2 getCroppedCoord(half2 screenCoord, half4 cropRegion)
{
    return (screenCoord - cropRegion.xy) / (cropRegion.zw - cropRegion.xy);
}
