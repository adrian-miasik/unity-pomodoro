using AdrianMiasik.Components.Base;
using AdrianMiasik.Components.Core;
using AdrianMiasik.ScriptableObjects;
using TMPro;
using UnityEngine;

namespace AdrianMiasik.Components.Specific
{
    /// <summary>
    /// A <see cref="ThemeElement"/> boolean slider with a label. Intended for the theme toggle (light / dark).
    /// </summary>
    public class ThemeSlider : ThemeElement
    {
        [SerializeField] private ToggleSlider m_toggle;
        [SerializeField] private TMP_Text m_label;
        
        /// <summary>
        /// Applies our <see cref="Theme"/> changes to our referenced components when necessary.
        /// And changes our text label from depending on the current <see cref="Theme"/>.
        /// </summary>
        /// <param name="theme">The theme to apply on our referenced components.</param>
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
        /// <summary>
        /// What color should this slider be when `False`?
        /// </summary>
        /// <param name="color">The color you want the boolean slider background to be.</param>
        public void OverrideFalseColor(Color color)
        {
            m_toggle.OverrideFalseColor(color);
        }

        /// <summary>
        /// What color should this slider be when `True`?
        /// </summary>
        /// <param name="color"></param>
        public void OverrideTrueColor(Color color)
        {
            m_toggle.OverrideTrueColor(color);
        }

        /// <summary>
        /// Presses our theme slider toggle.
        /// </summary>
        public void Interact()
        {
            m_toggle.OnPointerClick(null);
        }
    }
}
