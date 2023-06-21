using System.Collections;
using System.Collections.Generic;
using AdrianMiasik.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace AdrianMiasik.Components.Core.Todo
{
    /// <summary>
    /// This manager changes the Color Scheme's based on user Hex code input
    /// </summary>
public class CustomizationManager : MonoBehaviour
{
    public ColorScheme lightScheme;
    public ColorScheme darkScheme;
    public Button saveButton;
    public TMP_InputField backgroundInput;
    public TMP_InputField foregroundInput;
    public TMP_InputField mode1Input;
    public TMP_InputField runningInput;
    public TMP_InputField completeInput;
    public TMP_InputField mode2Input;
    public TMP_InputField closeInput;

    public Theme theme;

    void Update()
    {
        saveButton.onClick.AddListener(ChangeColors);
    }

    private void ChangeColors()
    {
        Color Color;
        if (ColorUtility.TryParseHtmlString("#" + backgroundInput.text, out Color))
            {
                lightScheme.m_background = Color;
                darkScheme.m_background = Color;
            }

        if (ColorUtility.TryParseHtmlString("#" + foregroundInput.text, out Color))
            {
                lightScheme.m_foreground = Color;
                darkScheme.m_foreground = Color;
            }

        if (ColorUtility.TryParseHtmlString("#" + mode1Input.text, out Color))
            {
                lightScheme.m_modeOne = Color;
                darkScheme.m_modeOne = Color;
            }

        if (ColorUtility.TryParseHtmlString("#" + runningInput.text, out Color))
            {
                lightScheme.m_running = Color;
                darkScheme.m_running = Color;
            }

        if (ColorUtility.TryParseHtmlString("#" + completeInput.text, out Color))
            {
                lightScheme.m_complete = Color;
                darkScheme.m_complete = Color;
            }

        if (ColorUtility.TryParseHtmlString("#" + mode2Input.text, out Color))
            {
                lightScheme.m_modeTwo = Color;
                darkScheme.m_modeTwo = Color;
            }

        if (ColorUtility.TryParseHtmlString("#" + closeInput.text, out Color))
            {
                lightScheme.m_close = Color;
                darkScheme.m_close = Color;
            }

        theme.ApplyColorChanges();
    }
}
}
