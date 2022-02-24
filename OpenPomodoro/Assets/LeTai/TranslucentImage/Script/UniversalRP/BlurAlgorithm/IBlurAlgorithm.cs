using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting.APIUpdating;

namespace LeTai.Asset.TranslucentImage.UniversalRP
{
[MovedFrom("LeTai.Asset.TranslucentImage.LWRP")]
public enum BlurAlgorithmType
{
    ScalableBlur
}

[MovedFrom("LeTai.Asset.TranslucentImage.LWRP")]
public interface IBlurAlgorithm
{
    void Init(BlurConfig config, BlitMode blitMode);

    void Blur(CommandBuffer          cmd,
              RenderTargetIdentifier src,
              Rect                   srcCropRegion,
              RenderTexture          target);
}
}
