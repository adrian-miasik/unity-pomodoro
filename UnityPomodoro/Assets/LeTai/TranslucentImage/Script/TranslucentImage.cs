using UnityEngine;
using UnityEngine.UI;

namespace LeTai.Asset.TranslucentImage
{
/// <summary>
/// Dynamic blur-behind UI element
/// </summary>
[HelpURL("https://leloctai.com/asset/translucentimage/docs/articles/customize.html#translucent-image")]
public partial class TranslucentImage : Image, IMeshModifier
{
    /// <summary>
    /// Source of blur for this image
    /// </summary>
    public TranslucentImageSource source;

    /// <summary>
    /// (De)Saturate them image, 1 is normal, 0 is grey scale, below zero make the image negative
    /// </summary>
    [Tooltip("(De)Saturate them image, 1 is normal, 0 is black and white, below zero make the image negative")]
    [Range(-1, 3)]
    public float vibrancy = 1;

    /// <summary>
    /// Brighten/darken them image
    /// </summary>
    [Tooltip("Brighten/darken them image")] [Range(-1, 1)]
    public float brightness = 0;

    /// <summary>
    /// Flatten the color behind to help keep contrast on varying background
    /// </summary>
    [Tooltip("Flatten the color behind to help keep contrast on varying background")] [Range(0, 1)]
    public float flatten = .1f;

    static readonly int _vibrancyPropId   = Shader.PropertyToID("_Vibrancy");
    static readonly int _brightnessPropId = Shader.PropertyToID("_Brightness");
    static readonly int _flattenPropId    = Shader.PropertyToID("_Flatten");
    static readonly int _blurTexPropId    = Shader.PropertyToID("_BlurTex");
    static readonly int _cropRegionPropId = Shader.PropertyToID("_CropRegion");

    Material materialForRenderingCached;
    bool     shouldRun;

    protected override void Start()
    {
        base.Start();

        oldVibrancy   = vibrancy;
        oldBrightness = brightness;
        oldFlatten    = flatten;

        AutoAcquireSource();

        if (material)
        {
            //Have to use string comparison as Addressable break object comparision :(
            if (Application.isPlaying && material.shader.name != "UI/TranslucentImage")
            {
                Debug.LogWarning("Translucent Image requires a material using the \"UI/TranslucentImage\" shader");
            }
            else if (source)
            {
                material.SetTexture(_blurTexPropId, source.BlurredScreen);
            }
        }

#if UNITY_5_6_OR_NEWER
        if (canvas)
            canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord1;
#endif
    }

    bool IsInPrefabMode()
    {
#if UNITY_EDITOR
        var stage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
        if (stage != null)
        {
            return true;
        }
#endif
        return false;
    }

    bool sourceAcquiredOnStart = false;

    void AutoAcquireSource()
    {
        if (IsInPrefabMode()) return;
        if (sourceAcquiredOnStart) return;

        source                = source ? source : FindObjectOfType<TranslucentImageSource>();
        sourceAcquiredOnStart = true;
    }

    bool Validate()
    {
        if (!IsActive() || !material)
            return false;

        if (!source)
        {
            if (Application.isPlaying && !IsInPrefabMode())
            {
                Debug.LogWarning("TranslucentImageSource is missing. " +
                                 "Please add the TranslucentImageSource component to your main camera, " +
                                 "then assign it to the Source field of the Translucent Image(s)");
            }

            return false;
        }

        if (!source.BlurredScreen)
            return false;

        return true;
    }

    void LateUpdate()
    {
        if (!shouldRun) return;

        // After re-enabled
        if (!materialForRenderingCached)
        {
            materialForRenderingCached = materialForRendering;
        }

        materialForRenderingCached.SetTexture(_blurTexPropId, source.BlurredScreen);
        materialForRenderingCached.SetVector(_cropRegionPropId, source.BlurRegionNormalizedScreenSpace.ToMinMaxVector());
    }

    void Update()
    {
        shouldRun = Validate();
        if (!shouldRun) return;

        if (_vibrancyPropId == 0 || _brightnessPropId == 0 || _flattenPropId == 0)
            return;

        materialForRenderingCached = materialForRendering;

        SyncMaterialProperty(_vibrancyPropId,   ref vibrancy,   ref oldVibrancy);
        SyncMaterialProperty(_brightnessPropId, ref brightness, ref oldBrightness);
        SyncMaterialProperty(_flattenPropId,    ref flatten,    ref oldFlatten);
    }

    float oldVibrancy, oldBrightness, oldFlatten;

    /// <summary>
    /// Sync material property with instance
    /// </summary>
    /// <param name="propId">material property id</param>
    /// <param name="value"></param>
    /// <param name="oldValue"></param>
    void SyncMaterialProperty(int propId, ref float value, ref float oldValue)
    {
        float matValue = materialForRendering.GetFloat(propId);
        if (Mathf.Abs(matValue - value) > 1e-4)
        {
            if (Mathf.Abs(value - oldValue) > 1e-4)
            {
                if (materialForRenderingCached)
                    materialForRenderingCached.SetFloat(propId, value);

                material.SetFloat(propId, value);
                SetMaterialDirty();
            }
            else
            {
                value = matValue;
            }
        }

        oldValue = value;
    }
}
}
