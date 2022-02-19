using AdrianMiasik.Components.Base;
using AdrianMiasik.Components.Specific.Settings;
using AdrianMiasik.ScriptableObjects;
using TMPro;
using UnityEngine;

namespace AdrianMiasik.Components.Core
{
    /// <summary>
    /// A <see cref="ThemeElement"/> that has a label with a boolean slider intended to be used
    /// as a settings option on the <see cref="SettingsPanel"/>.
    /// </summary>
    public class SettingsOptionToggleSlider : ThemeElement
    {
        [SerializeField] private TMP_Text m_settingsLabel;
        [SerializeField] public ToggleSlider m_toggleSlider;
        [SerializeField] public RectTransform m_spacer; // Bottom margin

        protected TimerSettings Settings;
        
        public void OverrideFalseColor(Color backgroundHighlight)
        {
            m_toggleSlider.OverrideFalseColor(backgroundHighlight);
        }

        public void OverrideTrueColor(Color modeOne)
        {
            m_toggleSlider.OverrideTrueColor(modeOne);
        }
        
        public virtual void Initialize(PomodoroTimer pomodoroTimer, TimerSettings settingsConfig)
        {
            base.Initialize(pomodoroTimer);
            Settings = settingsConfig;
        }

        public void UpdateToggle(bool state)
        {
            m_toggleSlider.Refresh(state);
        }
        
        public override void ColorUpdate(Theme theme)
        {
            m_settingsLabel.color = theme.GetCurrentColorScheme().m_foreground;
            m_toggleSlider.ColorUpdate(theme);
        }
    }
}