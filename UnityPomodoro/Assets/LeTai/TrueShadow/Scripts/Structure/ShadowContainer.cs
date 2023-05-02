using UnityEngine;

namespace LeTai.TrueShadow
{
class ShadowContainer
{
    public RenderTexture         Texture     { get; }
    public ShadowSettingSnapshot Snapshot    { get; }
    public int                   Padding     { get; }
    public Vector2Int            ImprintSize { get; }

    public int RefCount { get; internal set; }

    public readonly int requestHash;

    internal ShadowContainer(RenderTexture         texture,
                             ShadowSettingSnapshot snapshot,
                             int                   padding,
                             Vector2Int            imprintSize)
    {
        Texture     = texture;
        Snapshot    = snapshot;
        Padding     = padding;
        ImprintSize = imprintSize;
        RefCount    = 1;
        requestHash = snapshot.GetHashCode();
    }
}
}
