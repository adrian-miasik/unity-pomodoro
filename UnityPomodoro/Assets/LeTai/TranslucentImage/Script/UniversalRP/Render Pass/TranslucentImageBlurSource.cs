using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    ScriptableRenderer       renderer;
    Func<RenderTargetHandle> getBackBufferDelegate;

    public void CacheRenderer(ScriptableRenderer renderer)
    {
#if URP12_OR_NEWER
        if (this.renderer == renderer) return;

        this.renderer = renderer;

        if (renderer is UniversalRenderer ur)
        {
            var cbs = ur.GetType()
                        .GetField("m_ColorBufferSystem",
                                  BindingFlags.NonPublic | BindingFlags.Instance)
                        .GetValue(ur);
            var gbb = cbs.GetType()
                         .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                         .First(m => m.Name == "GetBackBuffer"
                                  && m.GetParameters().Length == 0);
            getBackBufferDelegate = (Func<RenderTargetHandle>)gbb.CreateDelegate(typeof(Func<RenderTargetHandle>), cbs);
        }
#endif
    }

    public RenderTargetHandle GetBackBuffer()
    {
        Debug.Assert(getBackBufferDelegate != null);

        // var sw = Stopwatch.StartNew();
        var r = getBackBufferDelegate.Invoke();
        // sw.Stop();
        // Debug.Log($"{sw.Elapsed.TotalMilliseconds}");
        return r;
    }
}

[MovedFrom("LeTai.Asset.TranslucentImage.LWRP")]
public class TranslucentImageBlurSource : ScriptableRendererFeature
{
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
        pass = new TranslucentImageBlurRenderPass(universalRendererInternal) {
#if URP12_OR_NEWER
            renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing
#else
            renderPassEvent = RenderPassEvent.AfterRendering
#endif
        };

        tisCache.Clear();
    }

    public override void AddRenderPasses(ScriptableRenderer renderer,
                                         ref RenderingData  renderingData)
    {
        var tis = GetTIS(renderingData.cameraData.camera);

        if (tis == null || !tis.shouldUpdateBlur())
            return;

        tis.OnBeforeBlur();
        var activeBlitMode = GetActiveBlitMode();
        blurAlgorithm.Init(tis.BlurConfig, activeBlitMode);
        RendererType rendererType = RendererType.Universal;

        if (renderer is UniversalRenderer)
        {
            universalRendererInternal.CacheRenderer(renderer);
        }
        else
        {
            rendererType = RendererType.Renderer2D;
        }

        var passData = new TISPassData {
            rendererType      = rendererType,
            cameraColorTarget = renderer.cameraColorTarget,
            blurAlgorithm     = blurAlgorithm,
            blitMode          = activeBlitMode,
            blurSource        = tis,
            isPreviewing      = tis.preview,
        };

        pass.Setup(passData);

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
