using LeTai.TrueShadow.PluginInterfaces;
using UnityEngine;

namespace LeTai.TrueShadow
{
[ExecuteAlways]
[RequireComponent(typeof(TrueShadow))]
public class DisableShadowCache : MonoBehaviour, ITrueShadowCustomHashProvider
{
    TrueShadow shadow;

    void OnEnable()
    {
        shadow            = GetComponent<TrueShadow>();
        shadow.CustomHash = shadow.GetInstanceID();
        shadow.SetTextureDirty();
    }

    void OnDisable()
    {
        shadow.CustomHash = 0;
        shadow.SetTextureDirty();
    }
}
}
