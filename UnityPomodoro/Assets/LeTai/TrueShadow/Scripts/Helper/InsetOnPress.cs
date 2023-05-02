using UnityEngine;

namespace LeTai.TrueShadow
{
[RequireComponent(typeof(TrueShadow))]
public class InsetOnPress : AnimatedBiStateButton
{
    TrueShadow[] shadows;
    float[]      normalOpacity;
    bool         wasInset;

    void OnEnable()
    {
        shadows       = GetComponents<TrueShadow>();
        normalOpacity = new float[shadows.Length];
    }

    protected override void Animate(float visualPressAmount)
    {
        void SetAllOpacity(float lerpProgress)
        {
            for (var i = 0; i < shadows.Length; i++)
            {
                var color = shadows[i].Color;
                color.a          = Mathf.Lerp(0, normalOpacity[i], lerpProgress);
                shadows[i].Color = color;
            }
        }

        bool shouldInset = visualPressAmount > .5f;

        if (shouldInset != wasInset)
        {
            for (var i = 0; i < shadows.Length; i++)
            {
                shadows[i].Inset = shouldInset;
            }

            wasInset = shouldInset;
        }

        if (shouldInset)
        {
            SetAllOpacity(visualPressAmount * 2f - 1f);
        }
        else
        {
            SetAllOpacity(1 - visualPressAmount * 2f);
        }
    }

    void MemorizeOpacity()
    {
        if (IsAnimating) return;

        for (var i = 0; i < shadows.Length; i++)
        {
            normalOpacity[i] = shadows[i].Color.a;
        }
    }

    protected override void OnWillPress()
    {
        wasInset = shadows[0].Inset;
        MemorizeOpacity();
        base.OnWillPress();
    }
}
}
