using System;
using LeTai.TrueShadow.PluginInterfaces;
using UnityEngine;

namespace LeTai.TrueShadow
{
[ExecuteAlways]
public class ShadowMaterial : MonoBehaviour, ITrueShadowRendererMaterialProvider
{
    public Material material;

    public event Action materialReplaced;
    public event Action materialModified;

    public Material GetTrueShadowRendererMaterial()
    {
        if (!isActiveAndEnabled) // Component Destroyed
            return null;

        return material;
    }

    void OnEnable()
    {
        var ts = GetComponent<TrueShadow>();
        if (ts)
        {
            ts.RefreshPlugins();
        }

        materialReplaced?.Invoke();
    }

    void OnDisable()
    {
        var ts = GetComponent<TrueShadow>();
        if (ts)
        {
            ts.RefreshPlugins();
        }
        materialReplaced?.Invoke();
    }

    void OnValidate()
    {
        materialReplaced?.Invoke();
    }

    public void OnMaterialModified()
    {
        materialModified?.Invoke();
    }
}
}
