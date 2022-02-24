#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace LeTai.Asset.TranslucentImage
{
[ExecuteAlways]
[AddComponentMenu("UI/Translucent Image", 2)]
public partial class TranslucentImage
{
    protected override void Reset()
    {
        base.Reset();
        color = Color.white;

        material   = FindDefaultMaterial();
        vibrancy   = material.GetFloat(_vibrancyPropId);
        brightness = material.GetFloat(_brightnessPropId);
        flatten    = material.GetFloat(_flattenPropId);

        source = source ? source : FindObjectOfType<TranslucentImageSource>();
    }

    static Material FindDefaultMaterial()
    {
        var guid = AssetDatabase.FindAssets("Default-Translucent t:Material l:TranslucentImageResource");

        if (guid.Length == 0)
            Debug.LogError("Can't find Default-Translucent Material");

        var path = AssetDatabase.GUIDToAssetPath(guid[0]);

        return AssetDatabase.LoadAssetAtPath<Material>(path);
    }

    protected override void OnValidate()
    {
        base.OnValidate();

        SetVerticesDirty();

        Update();
    }
}
}
#endif
