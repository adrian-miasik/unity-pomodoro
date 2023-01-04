using System;
using UnityEngine;
using UnityEngine.Rendering;
#if ENABLE_VR
using UnityEngine.XR;
#endif

namespace LeTai.Asset.TranslucentImage
{
/// <summary>
/// Common source of blur for Translucent Images.
/// </summary>
/// <remarks>
/// It is an Image effect that blur the render target of the Camera it attached to, then save the result to a global read-only  Render Texture
/// </remarks>
[ExecuteAlways]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Tai Le Assets/Translucent Image Source")]
[HelpURL("https://leloctai.com/asset/translucentimage/docs/articles/customize.html#translucent-image-source")]
public partial class TranslucentImageSource : MonoBehaviour
{
#region Public field
    /// <summary>
    /// Maximum number of times to update the blurred image each second
    /// </summary>
    public float maxUpdateRate = float.PositiveInfinity;

    /// <summary>
    /// Render the blurred result to the render target
    /// </summary>
    public bool preview;
#endregion


#region Private Field
    private IBlurAlgorithm blurAlgorithm;

    [SerializeField]
    private BlurAlgorithmType blurAlgorithmSelection = BlurAlgorithmType.ScalableBlur;

    [SerializeField]
    private BlurConfig blurConfig;

    [SerializeField]
    int downsample;

    int lastDownsample;

    [SerializeField]
    Rect blurRegion = new Rect(0, 0, 1, 1);

    Rect lastBlurRegion = new Rect(0, 0, 1, 1);

    //Disable non-sense warning from Unity
#pragma warning disable 0108
    Camera camera;
#pragma warning restore 0108

    Material      previewMaterial;
    RenderTexture blurredScreen;
#endregion


#region Properties
    public BlurAlgorithmType BlurAlgorithmSelection
    {
        get { return blurAlgorithmSelection; }
        set
        {
            if (value == blurAlgorithmSelection)
                return;
            blurAlgorithmSelection = value;
            InitializeBlurAlgorithm();
        }
    }

    public BlurConfig BlurConfig
    {
        get { return blurConfig; }
        set
        {
            blurConfig = value;
            InitializeBlurAlgorithm();
        }
    }

    /// <summary>
    /// Result of the image effect. Translucent Image use this as their content (read-only)
    /// </summary>
    public RenderTexture BlurredScreen
    {
        get { return blurredScreen; }
        set { blurredScreen = value; }
    }

    /// <summary>
    /// The Camera attached to the same GameObject. Cached in field 'camera'
    /// </summary>
    internal Camera Cam
    {
        get { return camera ? camera : camera = GetComponent<Camera>(); }
    }

    /// <summary>
    /// The rendered image will be shrinked by a factor of 2^{{this}} before bluring to reduce processing time
    /// </summary>
    /// <value>
    /// Must be non-negative. Default to 0
    /// </value>
    public int Downsample
    {
        get { return downsample; }
        set { downsample = Mathf.Max(0, value); }
    }

    /// <summary>
    /// Define the rectangular area on screen that will be blurred.
    /// </summary>
    /// <value>
    /// Between 0 and 1
    /// </value>
    public Rect BlurRegion
    {
        get { return blurRegion; }
        set
        {
            Vector2 min = new Vector2(1 / (float)Cam.pixelWidth, 1 / (float)Cam.pixelHeight);
            blurRegion.x      = Mathf.Clamp(value.x,      0,     1 - min.x);
            blurRegion.y      = Mathf.Clamp(value.y,      0,     1 - min.y);
            blurRegion.width  = Mathf.Clamp(value.width,  min.x, 1 - blurRegion.x);
            blurRegion.height = Mathf.Clamp(value.height, min.y, 1 - blurRegion.y);
        }
    }

    public Rect BlurRegionNormalizedScreenSpace
    {
        get
        {
            var camRect = Cam.rect;
            return new Rect(camRect.position + BlurRegion.position * camRect.size,
                            camRect.size * BlurRegion.size);
        }

        set
        {
            var camRect = Cam.rect;
            blurRegion.position = (value.position - camRect.position) / camRect.size;
            blurRegion.size     = value.size / camRect.size;
        }
    }

    /// <summary>
    /// Minimum time in second to wait before refresh the blurred image.
    /// If maxUpdateRate non-positive then just stop updating
    /// </summary>
    float MinUpdateCycle
    {
        get { return (maxUpdateRate > 0) ? (1f / maxUpdateRate) : float.PositiveInfinity; }
    }
#endregion


#if UNITY_EDITOR

    protected virtual void OnEnable()
    {
        if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Start();
        }
    }

    protected virtual void OnGUI()
    {
        if (!preview) return;
        if (UnityEditor.Selection.activeGameObject != gameObject) return;

        var curBlurRegionNSS = BlurRegionNormalizedScreenSpace;
        var newBlurRegionNSS = ResizableScreenRect.Draw(curBlurRegionNSS);

        if (newBlurRegionNSS != curBlurRegionNSS)
        {
            UnityEditor.Undo.RecordObject(this, "Change Blur Region");
            BlurRegionNormalizedScreenSpace = newBlurRegionNSS;
        }

        if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
    }
#endif

    protected virtual void Start()
    {
        previewMaterial = new Material(Shader.Find("Hidden/FillCrop"));

        InitializeBlurAlgorithm();
        CreateNewBlurredScreen();

        lastDownsample = Downsample;
    }

    void OnDestroy()
    {
        // RT are not released automatically
        if (BlurredScreen)
            BlurredScreen.Release();
    }

    void InitializeBlurAlgorithm()
    {
        switch (BlurAlgorithmSelection)
        {
        case BlurAlgorithmType.ScalableBlur:
            blurAlgorithm = new ScalableBlur();
            break;
        default:
            throw new ArgumentOutOfRangeException(nameof(BlurAlgorithmSelection));
        }

        blurAlgorithm.Init(BlurConfig);
    }

    protected virtual void CreateNewBlurredScreen()
    {
        if (BlurredScreen)
            BlurredScreen.Release();

#if ENABLE_VR
        if (XRSettings.enabled)
        {
            BlurredScreen = new RenderTexture(XRSettings.eyeTextureDesc);
            BlurredScreen.width = Mathf.RoundToInt(BlurredScreen.width * BlurRegion.width) >> Downsample;
            BlurredScreen.height = Mathf.RoundToInt(BlurredScreen.height * BlurRegion.height) >> Downsample;
            BlurredScreen.depth = 0;
        }
        else
#endif
        {
            BlurredScreen = new RenderTexture(Mathf.RoundToInt(Cam.pixelWidth * BlurRegion.width) >> Downsample,
                                              Mathf.RoundToInt(Cam.pixelHeight * BlurRegion.height) >> Downsample, 0);
        }

        BlurredScreen.antiAliasing = 1;
        BlurredScreen.useMipMap    = false;

        BlurredScreen.name       = $"{gameObject.name} Translucent Image Source";
        BlurredScreen.filterMode = FilterMode.Bilinear;

        BlurredScreen.Create();
    }

    TextureDimension lastEyeTexDim;

    public void OnBeforeBlur()
    {
        if (
            BlurredScreen == null
         || !BlurredScreen.IsCreated()
         || Downsample != lastDownsample
         || !BlurRegion.Approximately(lastBlurRegion)
#if ENABLE_VR
         || XRSettings.deviceEyeTextureDimension != lastEyeTexDim
#endif
        )
        {
            CreateNewBlurredScreen();
            lastDownsample = Downsample;
            lastBlurRegion = BlurRegion;
#if ENABLE_VR
            lastEyeTexDim = XRSettings.deviceEyeTextureDimension;
#endif
        }
    }

    protected virtual void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (blurAlgorithm == null || BlurConfig == null)
            goto draw_unmodified;

        if (shouldUpdateBlur())
        {
            OnBeforeBlur();
            blurAlgorithm.Blur(source, BlurRegion, ref blurredScreen);
        }

        if (preview)
        {
            previewMaterial.SetVector(ShaderId.CROP_REGION, BlurRegion.ToMinMaxVector());
            Graphics.Blit(BlurredScreen, destination, previewMaterial);
            return;
        }

        draw_unmodified:
        Graphics.Blit(source, destination);
    }

    float lastUpdate;

    public bool shouldUpdateBlur()
    {
        if (!enabled)
            return false;

        float now    = GetTrueCurrentTime();
        bool  should = now - lastUpdate >= MinUpdateCycle;

        if (should)
            lastUpdate = GetTrueCurrentTime();

        return should;
    }

    private static float GetTrueCurrentTime()
    {
#if UNITY_EDITOR
        return (float)UnityEditor.EditorApplication.timeSinceStartup;
#else
            return Time.unscaledTime;
#endif
    }
}
}
