using UnityEngine;

namespace LeTai.Asset.TranslucentImage
{
    public enum BlurAlgorithmType
    {
        ScalableBlur
    }

    public interface IBlurAlgorithm
    {
        void Init(BlurConfig    config);
        void Blur(RenderTexture source, Rect sourceCropRegion, ref RenderTexture blurredTexture);
    }
}