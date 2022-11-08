using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using static LeTai.TrueShadow.Math;

namespace LeTai.TrueShadow.Demo
{
[Serializable]
public class KnobValueChangedEvent : UnityEvent<float> { }

[ExecuteAlways]
public class DirectionalKnob : UIBehaviour, IDragHandler
{
    public Transform knobGraphic;
    public float     min   = 0;
    public float     max   = 1;
    public float     value = .5f;

    public KnobValueChangedEvent knobValueChanged;

    RectTransform rectTransform;
    Vector2       zeroVector;


    protected override void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();

        value = Mathf.Clamp(value, min, max);
        SetValue(value);
    }
#endif

    public void SetValue(float newValue)
    {
        value = newValue;

        knobGraphic.localRotation = Quaternion.Euler(0, 0, 1 - value * 360);

        knobValueChanged.Invoke(value);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out var point
        ))
        {
            var angle = Angle360(Vector2.down, point);
            SetValue(Mathf.InverseLerp(min, max, 1 - angle / 360f));
        }
    }
}
}
