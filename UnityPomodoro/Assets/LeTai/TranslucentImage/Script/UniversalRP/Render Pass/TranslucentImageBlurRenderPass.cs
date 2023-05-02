using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Scripting.APIUpdating;
using ShaderIdCommon = LeTai.Asset.TranslucentImage.ShaderId;

namespace LeTai.Asset.TranslucentImage.UniversalRP
{
enum RendererType
{
    Universal,
    Renderer2D
}

[MovedFrom("LeTai.Asset.TranslucentImage.LWRP")]
struct TISPassData
{
    public RendererType           rendererType;
    public RenderTargetIdentifier cameraColorTarget;
    public TranslucentImageSource blurSource;
    public IBlurAlgorithm         blurAlgorithm;
    public RenderOrder            renderOrder;
    public BlitMode               blitMode;
    public bool                   isPreviewing;
}

[MovedFrom("LeTai.Asset.TranslucentImage.LWRP")]
public class TranslucentImageBlurRenderPass : ScriptableRenderPass
{
    private const string PROFILER_TAG = "Translucent Image Source";

    readonly UniversalRendererInternal universalRendererInternal;
    readonly RenderTargetIdentifier    afterPostprocessTexture;

    TISPassData currentPassData;
    Material    previewMaterial;

    public Material PreviewMaterial
    {
        get
        {
            if (!previewMaterial)
                previewMaterial = CoreUtils.CreateEngineMaterial("Hidden/FillCrop_UniversalRP");

            return previewMaterial;
        }
    }

    internal TranslucentImageBlurRenderPass(UniversalRendererInternal universalRendererInternal)
    {
        this.universalRendererInternal = universalRendererInternal;
        afterPostprocessTexture = new RenderTargetIdentifier(Shader.PropertyToID("_AfterPostProcessTexture"),
                                                             0, CubemapFace.Unknown, -1);
    }

    ~TranslucentImageBlurRenderPass()
    {
        CoreUtils.Destroy(previewMaterial);
    }

    internal void Setup(TISPassData passData)
    {
        currentPassData = passData;
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        var                    cmd = CommandBufferPool.Get(PROFILER_TAG);
        RenderTargetIdentifier source;
#if URP12_OR_NEWER
        if (currentPassData.rendererType == RendererType.Universal)
        {
            source = universalRendererInternal.GetBackBuffer();
        }
        else
        {
#endif
        bool useAfterPostTex = renderingData.cameraData.postProcessEnabled;
#if URP12_OR_NEWER
            useAfterPostTex &= currentPassData.renderOrder == RenderOrder.AfterPostProcessing;
#endif
        source = useAfterPostTex
                     ? afterPostprocessTexture
                     : currentPassData.cameraColorTarget;
#if URP12_OR_NEWER
        }
#endif

        currentPassData.blurAlgorithm.Blur(cmd,
                                           source,
                                           currentPassData.blurSource.BlurRegion,
                                           currentPassData.blurSource.BlurredScreen);

        if (currentPassData.isPreviewing)
        {
            PreviewMaterial.SetVector(ShaderIdCommon.CROP_REGION,
                                      currentPassData.blurSource.BlurRegion.ToMinMaxVector());
            cmd.BlitCustom(currentPassData.blurSource.BlurredScreen,
                           source,
                           PreviewMaterial,
                           0,
                           BlitMode.Triangle);
        }

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }
}
}
