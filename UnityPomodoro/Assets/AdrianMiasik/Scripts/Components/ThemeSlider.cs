using AdrianMiasik.Components.Core;
using AdrianMiasik.ScriptableObjects;
using TMPro;
using UnityEngine;

namespace AdrianMiasik.Components
{
    public class ThemeSlider : ThemeElement
    {
        [SerializeField] private BooleanSlider m_toggle;
        [SerializeField] private TMP_Text m_label;
        
        public void Initialize(PomodoroTimer pomodoroTimer, bool state)
        {
            // Register theme element
            base.Initialize(pomodoroTimer);
            
            m_toggle.OverrideDotColor(pomodoroTimer.GetTheme().GetCurrentColorScheme().m_foreground);
            m_toggle.Initialize(pomodoroTimer, state);
        }

        public override void ColorUpdate(Theme theme)
        {
            m_toggle.ColorUpdate(theme);
            m_label.color = theme.GetCurrentColorScheme().m_foreground;
            m_label.text = timer.GetTheme().m_isLightModeOn ? "Dark" : "Light";
            // TODO: Change toggle dot image to a moon for dark mode
        }

        // Piper methods
        public void OverrideFalseColor(Color color)
        {
            m_toggle.OverrideFalseColor(color);
        }

        public void OverrideTrueColor(Color color)
        {
            m_toggle.OverrideTrueColor(color);
        }

        public void Interact()
        {
            m_toggle.OnPointerClick(null);
        }
    }
}
