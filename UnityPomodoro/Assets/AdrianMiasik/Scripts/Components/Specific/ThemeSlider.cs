using AdrianMiasik.Components.Base;
using AdrianMiasik.Components.Core;
using AdrianMiasik.ScriptableObjects;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AdrianMiasik.Components.Specific
{
    /// <summary>
    /// A <see cref="ThemeElement"/> boolean slider with a custom icon. Intended for toggling between our light / dark
    /// mode themes.
    /// </summary>
    public class ThemeSlider : ThemeElement
    {
        [SerializeField] private ToggleSlider m_toggle;

        [SerializeField] private Sprite m_moonSprite;
        [SerializeField] private Material m_sliderDotCircle;

        private readonly Vector2 cachedOffsetMin = new Vector2(3, 1.5f); 
        private readonly Vector2 cachedOffsetMax = new Vector2(1.5f, -1.5f);

        public override void Initialize(PomodoroTimer pomodoroTimer, bool updateColors = true)
        {
            base.Initialize(pomodoroTimer, updateColors);

            // Theme Slider
            m_toggle.m_onSetToTrueClick.AddListener(() => { pomodoroTimer.GetTheme().SetToDarkMode(); });
            m_toggle.m_onSetToFalseClick.AddListener(() => { pomodoroTimer.GetTheme().SetToLightMode(); });

            m_toggle.OverrideDotColor(Timer.GetTheme().GetCurrentColorScheme().m_foreground);
            m_toggle.Initialize(Timer, Timer.GetSystemSettings().m_darkMode);
        }

        /// <summary>
        /// Applies our <see cref="Theme"/> changes to our referenced components when necessary.
        /// And changes our text label from depending on the current <see cref="Theme"/>.
        /// </summary>
        /// <param name="theme">The theme to apply on our referenced components.</param>
        public override void ColorUpdate(Theme theme)
        {
            // Regular boolean
            if (Timer.GetSystemSettings().m_darkMode)
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
        
        public void OverrideDotColor(Color color)
        {
            m_toggle.OverrideDotColor(color);
        }

        /// <summary>
        /// Presses our theme slider toggle.
        /// </summary>
        public void Interact()
        {
            m_toggle.OnPointerClick(null);
        }

        public void SetVisualToEnable()
        {
            m_toggle.SetVisualToEnable();
        }

        public void SetVisualToDisable()
        {
            m_toggle.SetVisualToDisable();
        }

        public void Refresh()
        {
            if (m_toggle.IsOn() != Timer.GetSystemSettings().m_darkMode)
            {
                m_toggle.Press();
            }
        }

        public void EnableAnimation()
        {
            m_toggle.EnableAnimation();
        }
    }
}