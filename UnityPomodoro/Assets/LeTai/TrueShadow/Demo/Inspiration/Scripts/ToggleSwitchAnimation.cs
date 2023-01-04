using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.RectTransform.Edge;

namespace LeTai.TrueShadow.Demo
{
[ExecuteAlways]
public class ToggleSwitchAnimation : MonoBehaviour
{
    public Color         onColor  = new Color(0.29f, 1,    0.26f);
    public Color         offColor = new Color(0.93f, 0.3f, 0.23f);
    public Graphic       background;
    public RectTransform handle;
    public float         handleInset        = 1.5f;
    public float         transitionDuration = .1f;

    Coroutine anim;

    void Start()
    {
        var isOn = GetComponent<Toggle>().isOn;
        background.color = isOn ? onColor : offColor;
        handle.SetInsetAndSizeFromParentEdge(isOn ? Right : Left, handleInset, handle.rect.width);
    }

    IEnumerator Animate(float fromX, Color fromColor)
    {
        var toColor  = background.color;
        var position = handle.position;
        var toX      = position.x;
        var y        = position.y;
        var z        = position.z;
        var t        = 0f;
        do
        {
            t = Mathf.Min(t + Time.deltaTime / transitionDuration, 1);

            var tsmoothed = Mathf.SmoothStep(0, 1, t);
            handle.position  = new Vector3(Mathf.Lerp(fromX, toX, tsmoothed), y, z);
            background.color = Color.Lerp(fromColor, toColor, tsmoothed);

            yield return null;
        } while (t < 1);
    }

    public void Toggle(bool isOn)
    {
        if (!background) return;
        if (!handle) return;

        var fromColor = background.color;
        background.color = isOn ? onColor : offColor;
        var fromX = handle.position.x;
        handle.SetInsetAndSizeFromParentEdge(isOn ? Right : Left, handleInset, handle.rect.width);

        if (!Application.isEditor || Application.isPlaying)
        {
            if (anim != null) StopCoroutine(anim);
            anim = StartCoroutine(Animate(fromX, fromColor));
        }
    }
}
}
