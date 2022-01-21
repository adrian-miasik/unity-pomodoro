using AdrianMiasik.Components;
using AdrianMiasik.Components.Base;
using AdrianMiasik.Components.Core;
using AdrianMiasik.ScriptableObjects;
using TMPro;
using UnityEngine;

public class LongBreakSetting : ThemeElement
{
    [SerializeField] private TMP_Text m_longBreaksLabel;
    [SerializeField] public ToggleSlider m_longBreakToggle;

    public void Initialize(PomodoroTimer pomodoroTimer, Settings settingsConfig)
    {
        base.Initialize(pomodoroTimer);
        if (settingsConfig.m_longBreaks)
        {
            m_longBreakToggle.Initialize(pomodoroTimer, settingsConfig.m_longBreaks);
        }
    }
    public override void ColorUpdate(Theme theme)
    {
        m_longBreaksLabel.color = theme.GetCurrentColorScheme().m_foreground;
        m_longBreakToggle.ColorUpdate(theme);
    }
    
    /// <summary>
    /// <remarks>Used as a UnityEvent by toggle.</remarks>
    /// </summary>
    /// <param name="state"></param>
    public void SetSettingLongBreak(bool state)
    {
        if (state == false)
        {
            if (Timer.HasTomatoProgression())
            {
                // Prompt for user permission first
                Timer.SpawnConfirmationDialog(() =>
                {
                    Timer.SetSettingLongBreaks(false);
                }, (() =>
                {
                    // Cancel visuals if they don't agree, similar how we do the work/break slider
                    m_longBreakToggle.Initialize(Timer, true);
                }), "This action will delete your current pomodoro/tomato progress.");
            }
            else
            {
                // Set immediately
                Timer.SetSettingLongBreaks(false);
            }
        }
        else
        {
            // Set immediately
            Timer.SetSettingLongBreaks(true);
        }
    }
}
