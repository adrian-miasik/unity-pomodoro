using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Scripting.APIUpdating;
using Debug = UnityEngine.Debug;


namespace LeTai.Asset.TranslucentImage.UniversalRP
{
class UniversalRendererInternal
{
    ScriptableRenderer renderer;
#if UNITY_2022_1_OR_NEWER
    Func<RTHandle> getBackBufferDelegate;
#else
    Func<RenderTargetHandle> getBackBufferDelegate;
#endif

    public void CacheRenderer(ScriptableRenderer renderer)
    {
#if URP12_OR_NEWER
        if (this.renderer == renderer) return;

        this.renderer = renderer;
#if UNITY_2022_1_OR_NEWER
        const string backBufferMethodName = "PeekBackBuffer";
#else
        const string backBufferMethodName = "GetBackBuffer";
#endif

        if (renderer is UniversalRenderer ur)
        {
            var cbs = ur.GetType()
                        .GetField("m_ColorBufferSystem",
                                  BindingFlags.NonPublic | BindingFlags.Instance)
                        .GetValue(ur);
            var gbb = cbs.GetType()
                         .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                         .First(m => m.Name == backBufferMethodName
                                  && m.GetParameters().Length == 0);
#if UNITY_2022_1_OR_NEWER
            getBackBufferDelegate = (Func<RTHandle>)gbb.CreateDelegate(typeof(Func<RTHandle>), cbs);
#else
            getBackBufferDelegate = (Func<RenderTargetHandle>)gbb.CreateDelegate(typeof(Func<RenderTargetHandle>), cbs);
#endif
        }
#endif
    }

    public RenderTargetIdentifier GetBackBuffer()
    {
        Debug.Assert(getBackBufferDelegate != null);

        // var sw = Stopwatch.StartNew();
        var r = getBackBufferDelegate.Invoke();
        // sw.Stop();
        // Debug.Log($"{sw.Elapsed.TotalMilliseconds}");
#if UNITY_2022_1_OR_NEWER
        return r.nameID;
#else
        return r.Identifier();
#endif
    }
}

public enum RenderOrder
{
    AfterPostProcessing,
    BeforePostProcessing,
}

[MovedFrom("LeTai.Asset.TranslucentImage.LWRP")]
public class TranslucentImageBlurSource : ScriptableRendererFeature
{
#if URP12_OR_NEWER
    public RenderOrder renderOrder = RenderOrder.AfterPostProcessing;
#endif
    public BlitMode blitMode = BlitMode.Procedural;

    readonly Dictionary<Camera, TranslucentImageSource> tisCache = new Dictionary<Camera, TranslucentImageSource>();

    UniversalRendererInternal      universalRendererInternal;
    TranslucentImageBlurRenderPass pass;
    IBlurAlgorithm                 blurAlgorithm;

    BlitMode GetActiveBlitMode()
    {
        if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES2)
            return BlitMode.Triangle;

        return blitMode;
    }

    /// <summary>
    /// When adding new Translucent Image Source to existing Camera at run time, the new Source must be registered here
    /// </summary>
    /// <param name="source"></param>
    public void RegisterSource(TranslucentImageSource source)
    {
        tisCache[source.GetComponent<Camera>()] = source;
    }

    public override void Create()
    {
        ShaderId.Init(32); //hack for now

        blurAlgorithm = new ScalableBlur();

        universalRendererInternal = new UniversalRendererInternal();

        // ReSharper disable once JoinDeclarationAndInitializer
        RenderPassEvent renderPassEvent;
#if URP12_OR_NEWER
        renderPassEvent = renderOrder == RenderOrder.BeforePostProcessing
                              ? RenderPassEvent.BeforeRenderingPostProcessing
                              : RenderPassEvent.AfterRenderingPostProcessing;
#else
        renderPassEvent = RenderPassEvent.AfterRendering;
#endif
        pass = new TranslucentImageBlurRenderPass(universalRendererInternal) {
            renderPassEvent = renderPassEvent
        };

        tisCache.Clear();
    }

    void Setup(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        var cameraData = renderingData.cameraData;
        var tis        = GetTIS(cameraData.camera);

        if (tis == null)
            return;

        RendererType rendererType = RendererType.Universal;

        if (renderer is UniversalRenderer)
        {
            universalRendererInternal.CacheRenderer(renderer);
        }
        else
        {
            rendererType = RendererType.Renderer2D;
        }
#if UNITY_BUGGED_HAS_PASSES_AFTER_POSTPROCESS
        bool applyFinalPostProcessing = renderingData.postProcessingEnabled
                                     && cameraData.resolveFinalTarget
                                     && (cameraData.antialiasing == AntialiasingMode.FastApproximateAntialiasing
                                        );
        pass.renderPassEvent = applyFinalPostProcessing ? RenderPassEvent.AfterRenderingPostProcessing : RenderPassEvent.AfterRendering;
#endif

        var passData = new TISPassData {
            rendererType = rendererType,
#if UNITY_2022_1_OR_NEWER
            cameraColorTarget = renderer.cameraColorTargetHandle,
#else
            cameraColorTarget = renderer.cameraColorTarget,
#endif
            blurAlgorithm = blurAlgorithm,
#if URP12_OR_NEWER
            renderOrder   = renderOrder,
#endif
            blitMode      = GetActiveBlitMode(),
            blurSource    = tis,
            isPreviewing  = tis.preview,
        };

        pass.Setup(passData);
    }

#if UNITY_2022_1_OR_NEWER
    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        Setup(renderer, renderingData);
    }
#endif

    public override void AddRenderPasses(ScriptableRenderer renderer,
                                         ref RenderingData  renderingData)
    {
#if !UNITY_2022_1_OR_NEWER
        Setup(renderer, renderingData);
#endif

        if(!(renderer is UniversalRenderer))
        {
            Debug.LogError("Only Forward/Universal Renderer is supported in URP");
            return;
        }

        var tis = GetTIS(renderingData.cameraData.camera);

        if (tis == null || !tis.shouldUpdateBlur())
            return;

        tis.OnBeforeBlur();
        blurAlgorithm.Init(tis.BlurConfig, GetActiveBlitMode());

        renderer.EnqueuePass(pass);
    }

    TranslucentImageSource GetTIS(Camera camera)
    {
        if (!tisCache.ContainsKey(camera))
        {
            tisCache.Add(camera, camera.GetComponent<TranslucentImageSource>());
        }

        return tisCache[camera];
    }
}
}
