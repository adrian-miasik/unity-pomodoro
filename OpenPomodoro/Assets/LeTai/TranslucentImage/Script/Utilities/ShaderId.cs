using UnityEngine;

namespace LeTai.Asset.TranslucentImage
{
public static class ShaderId
{
    public static readonly int RADIUS      = Shader.PropertyToID("_Radius");
    public static readonly int CROP_REGION = Shader.PropertyToID("_CropRegion");
}
}
