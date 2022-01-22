using AdrianMiasik.Components.Base;
using AdrianMiasik.Components.Core;
using AdrianMiasik.ScriptableObjects;
using UnityEngine;

namespace AdrianMiasik.Components.Specific
{
    /// <summary>
    /// A <see cref="ThemeElement"/> boolean slider with custom icon. Intended for the theme toggle (light / dark).
    /// </summary>
    public class ThemeSlider : ThemeElement
    {
        [SerializeField] private ToggleSlider m_toggle;

        [SerializeField] private Sprite m_moonSprite;
        [SerializeField] private Material m_sliderDotCircle;

        private Vector2 cachedOffsetMin = new Vector2(3, 0); 
        private Vector2 cachedOffsetMax = new Vector2(1.5f, 1.5f); 

        private void Start()
        {
            cachedOffsetMin = m_toggle.m_dot.rectTransform.offsetMin;
            cachedOffsetMax = m_toggle.m_dot.rectTransform.offsetMax;
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
            
            // Regular boolean
            if (theme.m_darkMode)
            {
                m_toggle.m_dot.rectTransform.pivot = new Vector2(0.5f, m_toggle.m_dot.rectTransform.pivot.y);
                m_toggle.m_dot.sprite = null;
                m_toggle.m_dot.material = m_sliderDotCircle;
                m_toggle.m_dot.rectTransform.offsetMin = Vector2.zero;
                m_toggle.m_dot.rectTransform.offsetMax = Vector2.zero;
            }
            // Moon dark boolean
            else
            {
                m_toggle.m_dot.rectTransform.pivot = new Vector2(0f, m_toggle.m_dot.rectTransform.pivot.y);
                m_toggle.m_dot.sprite = m_moonSprite;
                m_toggle.m_dot.material = null;
                m_toggle.m_dot.rectTransform.offsetMin = cachedOffsetMin;
                m_toggle.m_dot.rectTransform.offsetMax = cachedOffsetMax;
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