using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting.APIUpdating;
using ShaderIdCommon = LeTai.Asset.TranslucentImage.ShaderId;

namespace LeTai.Asset.TranslucentImage.UniversalRP
{
[MovedFrom("LeTai.Asset.TranslucentImage.LWRP")]
public class ScalableBlur : IBlurAlgorithm
{
    Shader             shader;
    Material           material;
    ScalableBlurConfig config;
    BlitMode           blitMode;

    const int BLUR_PASS      = 0;
    const int CROP_BLUR_PASS = 1;

    Material Material
    {
        get
        {
            if (material == null)
                Material = new Material(Shader.Find("Hidden/EfficientBlur_UniversalRP"));

            return material;
        }
        set => material = value;
    }

    public void Init(BlurConfig config, BlitMode blitMode)
    {
        this.config   = (ScalableBlurConfig)config;
        this.blitMode = blitMode;

        switch (blitMode)
        {
        case BlitMode.Procedural:
            Material.EnableKeyword("PROCEDURAL_QUAD");
            break;
        case BlitMode.Triangle:
            Material.DisableKeyword("PROCEDURAL_QUAD");
            break;
        default:
            throw new ArgumentOutOfRangeException(nameof(blitMode), blitMode, null);
        }
    }

    public void Blur(CommandBuffer          cmd,
                     RenderTargetIdentifier src,
                     Rect                   srcCropRegion,
                     RenderTexture          target)
    {
        float radius = ScaleWithResolution(config.Radius,
                                           target.width * srcCropRegion.width,
                                           target.height * srcCropRegion.height);
        ConfigMaterial(radius, srcCropRegion.ToMinMaxVector());

        int firstDownsampleFactor = config.Iteration > 0 ? 1 : 0;
        int stepCount             = Mathf.Max(config.Iteration * 2 - 1, 1);

        int firstIRT = ShaderId.intermediateRT[0];
        CreateTempRenderTextureFrom(cmd, firstIRT, target, firstDownsampleFactor);
        cmd.BlitCustom(src, firstIRT, Material, CROP_BLUR_PASS, blitMode);


        for (var i = 1; i < stepCount; i++)
        {
            BlurAtDepth(cmd, i, target);
        }

        cmd.BlitCustom(ShaderId.intermediateRT[stepCount - 1],
                       target,
                       Material,
                       BLUR_PASS,
                       blitMode);

        CleanupIntermediateRT(cmd, stepCount);
    }

    void CreateTempRenderTextureFrom(CommandBuffer cmd,
                                     int           nameId,
                                     RenderTexture src,
                                     int           downsampleFactor)
    {
        var desc = src.descriptor;
        desc.width  = src.width >> downsampleFactor; //= width / 2^downsample
        desc.height = src.height >> downsampleFactor;

        cmd.GetTemporaryRT(nameId, desc, FilterMode.Bilinear);
    }

    protected virtual void BlurAtDepth(CommandBuffer cmd, int depth, RenderTexture baseTexture)
    {
        int sizeLevel = Utilities.SimplePingPong(depth, config.Iteration - 1) + 1;
        sizeLevel = Mathf.Min(sizeLevel, config.MaxDepth);
        CreateTempRenderTextureFrom(cmd, ShaderId.intermediateRT[depth], baseTexture, sizeLevel);

        cmd.BlitCustom(ShaderId.intermediateRT[depth - 1],
                       ShaderId.intermediateRT[depth],
                       Material, 0,
                       blitMode);
    }

    private void CleanupIntermediateRT(CommandBuffer cmd, int amount)
    {
        for (var i = 0; i < amount; i++)
        {
            cmd.ReleaseTemporaryRT(ShaderId.intermediateRT[i]);
        }
    }

    ///<summary>
    /// Relative blur size to maintain same look across multiple resolution
    /// </summary>
    float ScaleWithResolution(float baseRadius, float width, float height)
    {
        float scaleFactor = Mathf.Min(width, height) / 1080f;
        scaleFactor = Mathf.Clamp(scaleFactor, .5f, 2f); //too much variation cause artifact
        return baseRadius * scaleFactor;
    }

    protected void ConfigMaterial(float radius, Vector4 cropRegion)
    {
        Material.SetFloat(ShaderIdCommon.RADIUS, radius);
        Material.SetVector(ShaderIdCommon.CROP_REGION, cropRegion);
    }
}
}
