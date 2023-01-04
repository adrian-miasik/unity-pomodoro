using UnityEngine;
using UnityEngine.Rendering;

namespace LeTai.Effects
{
public interface IBlurAlgorithm
{
    void Configure(BlurConfig config);

    void Blur(CommandBuffer          cmd,
              RenderTargetIdentifier src,
              Rect                   srcCropRegion,
              RenderTexture          target);
}
}
