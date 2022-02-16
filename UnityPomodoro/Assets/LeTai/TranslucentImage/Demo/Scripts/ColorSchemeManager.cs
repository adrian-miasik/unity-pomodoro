using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LeTai.Asset.TranslucentImage.Demo
{
public class ColorSchemeManager : MonoBehaviour
{
    public Color lightBackgroudColor = Color.white;
    public Color lightForegroudColor = Color.white;
    public Color lightTextColor      = Color.white;
    public Color darkBackgroudColor  = Color.black;
    public Color darkForegroudColor  = Color.black;
    public Color darkTextColor       = Color.black;

    public Graphic[] foregroudGraphic;
    public Text[]    texts;

    Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    public enum DemoColorScheme
    {
        Light,
        Dark
    }

    public void SetLightScheme(bool on)
    {
        SetColorScheme(on ? DemoColorScheme.Light : DemoColorScheme.Dark);
    }

    public void SetColorScheme(DemoColorScheme scheme)
    {
        Color bg, fg, txt;
        switch (scheme)
        {
            case DemoColorScheme.Light:
                bg  = lightBackgroudColor;
                fg  = lightForegroudColor;
                txt = lightTextColor;
                break;
            case DemoColorScheme.Dark:
                bg  = darkBackgroudColor;
                fg  = darkForegroudColor;
                txt = darkTextColor;
                break;
            default:
                throw new ArgumentOutOfRangeException("scheme", scheme, null);
        }

        cam.backgroundColor = bg;
        foreach (var graphic in foregroudGraphic)
        {
            graphic.color = fg;
        }

        foreach (var text in texts)
        {
            text.color = txt;
        }
    }
}
}
