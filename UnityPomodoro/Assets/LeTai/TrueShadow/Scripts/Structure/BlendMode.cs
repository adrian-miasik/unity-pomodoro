using System;
using UnityEngine;

namespace LeTai.TrueShadow
{
public enum BlendMode
{
    Normal,
    Additive,
    Screen,
    Multiply,
}

public static class BlendModeExtensions
{
    static Material matNormal;
    static Material materialAdditive;
    static Material matScreen;
    static Material matMultiply;

    public static Material GetMaterial(this BlendMode blendMode)
    {
        switch (blendMode)
        {
        case BlendMode.Normal:
            if (!matNormal) matNormal = new Material(Shader.Find("UI/TrueShadow-Normal"));
            return matNormal;
        case BlendMode.Additive:
            if (!materialAdditive) materialAdditive = new Material(Shader.Find("UI/TrueShadow-Additive"));
            return materialAdditive;
        case BlendMode.Screen:
            if (!matScreen) matScreen = new Material(Shader.Find("UI/TrueShadow-Screen"));
            return matScreen;
        case BlendMode.Multiply:
            if (!matMultiply) matMultiply = new Material(Shader.Find("UI/TrueShadow-Multiply"));
            return matMultiply;
        default:
            throw new ArgumentOutOfRangeException();
        }
    }
}
}
