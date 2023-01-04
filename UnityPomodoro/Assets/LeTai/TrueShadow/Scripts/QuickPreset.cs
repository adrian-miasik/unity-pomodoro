using System;
using UnityEngine;

namespace LeTai.TrueShadow
{
[Serializable]
public struct QuickPreset
{
    public string name;

    [Min(0)]
    public float size;

    [SpreadSlider]
    public float spread;

    [Min(0)]
    public float distance;

    [Range(0, 1)]
    public float alpha;

    public QuickPreset(string name, float size, float spread, float distance, float alpha)
    {
        this.name     = name;
        this.size     = size;
        this.spread   = spread;
        this.distance = distance;
        this.alpha    = alpha;
    }

    public void Apply(TrueShadow target)
    {
        target.Size           = size;
        target.Spread         = spread;
        target.OffsetDistance = distance;

        var color = target.Color;
        color.a      = alpha;
        target.Color = color;
    }
}
}
