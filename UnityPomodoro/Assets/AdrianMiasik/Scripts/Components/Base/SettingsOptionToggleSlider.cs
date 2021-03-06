using AdrianMiasik.Components.Core;
using AdrianMiasik.Components.Core.Items.Pages;
using AdrianMiasik.Components.Core.Settings;
using AdrianMiasik.Components.Specific;
using AdrianMiasik.Components.Specific.Settings;
using AdrianMiasik.ScriptableObjects;
using TMPro;
using UnityEngine;

namespace AdrianMiasik.Components.Base
{
    /// <summary>
    /// A **base class** <see cref="ThemeElement"/> that has a label with a boolean slider intended to be used
    /// as a settings option on the <see cref="SettingsPage"/>.
    /// </summary>
    public class SettingsOptionToggleSlider : ThemeElement
    {
        [SerializeField] private TMP_Text m_settingsLabel;
        [SerializeField] public ToggleSlider m_toggleSlider;
        [SerializeField] public RectTransform m_spacer; // Bottom margin
        
        public void OverrideFalseColor(Color backgroundHighlight)
        {
            m_toggleSlider.OverrideFalseColor(backgroundHighlight);
        }

        public void OverrideTrueColor(Color modeOne)
        {
            m_toggleSlider.OverrideTrueColor(modeOne);
        }
        
        public virtual void Initialize(PomodoroTimer pomodoroTimer)
        {
            base.Initialize(pomodoroTimer);
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
        
        public void Refresh(bool cachedState)
        {
            // If there is a difference in state...
            if (m_toggleSlider.IsOn() != cachedState)
            {
                m_toggleSlider.Press();
            }
        }
    }
}