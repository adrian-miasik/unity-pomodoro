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
        
        public override void ColorUpdate(Theme theme)
        {
            m_toggle.OverrideDotColor(theme.GetCurrentColorScheme().m_foreground);
            m_toggle.Initialize(Timer, theme.m_darkMode);

            m_toggle.ColorUpdate(theme);
            m_label.color = theme.GetCurrentColorScheme().m_foreground;
            m_label.text = Timer.GetTheme().m_darkMode ? "Light" : "Dark";
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
