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

        [SerializeField] private Sprite m_moonSprite;
        [SerializeField] private Material m_sliderDotCircle;

        private Vector2 cachedOffsetMin = new Vector2(3, 0); 
        private Vector2 cachedOffsetMax = new Vector2(1.5f, 1.5f); 

        private void Start()
        {
            cachedOffsetMin = m_toggle.GetDotOffsetMin();
            cachedOffsetMax = m_toggle.GetDotOffsetMax();
        }

        /// <summary>
        /// Applies our <see cref="Theme"/> changes to our referenced components when necessary.
        /// And changes our text label from depending on the current <see cref="Theme"/>.
        /// </summary>
        /// <param name="theme">The theme to apply on our referenced components.</param>
        public override void ColorUpdate(Theme theme)
        {
            m_toggle.OverrideDotColor(theme.GetCurrentColorScheme().m_foreground);
            m_toggle.Initialize(Timer, theme.m_darkMode);
            
            m_label.color = theme.GetCurrentColorScheme().m_foreground;
            m_label.text = Timer.GetTheme().m_darkMode ? "Light" : "Dark";
            
            if (theme.m_darkMode)
            {
                m_toggle.SetDotSprite(null);
                m_toggle.SetDotMaterial(m_sliderDotCircle);
                m_toggle.SetDotOffsetMin(Vector2.zero);
                m_toggle.SetDotOffsetMax(Vector2.zero);
            }
            else
            {
                // m_toggle.SetDotColor(theme.GetCurrentColorScheme().bac)
                m_toggle.SetDotSprite(m_moonSprite);
                m_toggle.SetDotMaterial(null);
                m_toggle.SetDotOffsetMin(cachedOffsetMin);
                m_toggle.SetDotOffsetMax(cachedOffsetMax);
            }

            m_toggle.ColorUpdate(theme);
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
