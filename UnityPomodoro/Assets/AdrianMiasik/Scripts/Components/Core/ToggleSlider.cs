using AdrianMiasik.Components.Base;
using AdrianMiasik.ScriptableObjects;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AdrianMiasik.Components.Core
{
    //  TODO: Refactor to inherit from Toggle
    /// <summary>
    /// A <see cref="ThemeElement"/> <see cref="Base.Toggle"/> in the form of a boolean slider.
    /// </summary>
    public class ToggleSlider : ThemeElement, IPointerClickHandler
    {
        // General
        [SerializeField] public SVGImage m_background;
        [SerializeField] private Image m_dot;

        // Animation
        [SerializeField] private Animation m_animation;
        [SerializeField] private AnimationClip m_leftToRight;
        [SerializeField] private AnimationClip m_rightToLeft;

        // Unity Events
        public UnityEvent m_onSetToTrueClick; // clicking a disabled boolean
        public UnityEvent m_onSetToFalseClick; // clicking on an enabled boolean
        public UnityEvent m_onClick;
        
        // Shader Property
        private static readonly int CircleColor = Shader.PropertyToID("Color_297012532bf444df807f8743bdb7e4fd");

        // Cache
        private bool state;
        private Color trueColor;
        private Color falseColor;

        // Override
        private bool overrideFalseColor;
        private Color overridenFalseColor;
        private bool overrideTrueColor;
        private Color overridenTrueColor;
        
        // Dot Override
        private bool overrideDotColor;
        private Color overridenDotColor;
        
        /// <summary>
        /// Note: Needs to be invoked before initialize.<see cref="Initialize"/>
        /// </summary>
        /// <param name="color"></param>
        public void OverrideFalseColor(Color color)
        {
            overrideFalseColor = true;
            overridenFalseColor = color;
        }
        
        public void OverrideTrueColor(Color color)
        {
            overrideTrueColor = true;
            overridenTrueColor = color;
        }

        public void OverrideDotColor(Color color)
        {
            overrideDotColor = true;
            overridenDotColor = color;
        }

        public override void Initialize(PomodoroTimer timer, bool isOn)
        {
            base.Initialize(timer);
            
            state = isOn;
            if (state)
            {
                Enable();
            }
            else
            {
                Disable();
            }
        }

        /// <summary>
        /// Changes the visibility of the boolean slider to the ON or OFF position. Position depends on current state.
        /// </summary>
        /// <summary>
        /// Note: Unity Event
        /// </summary>>
        public void Press()
        {
            OnPointerClick(null);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // Flip state
            state = !state;
            OnStateChanged(true); // User interacted with this, we treating this as a click
            
            m_onClick.Invoke();
        }
        
        private void OnStateChanged(bool invokeEvents = false)
        {
            if (state)
            {
                // Set to on position
                m_animation.clip = m_leftToRight;
                m_background.color = trueColor;

                if (invokeEvents)
                {
                    m_onSetToTrueClick.Invoke();
                }
            }
            else
            {
                // Set to off position
                m_animation.clip = m_rightToLeft;
                m_background.color = falseColor;

                if (invokeEvents)
                {
                    m_onSetToFalseClick.Invoke();
                }
            }

            m_animation.Play();
        }

        /// <summary>
        /// Changes the visibility of the boolean slider to the OFF position (left).
        /// Note: OnClick Unity Events won't be triggered.
        /// </summary>
        private void Disable()
        {
            state = false;
            OnStateChanged();
        }

        /// <summary>
        /// Changes the visibility of the boolean slider to the ON position (right).
        /// Note: OnClick Unity Events won't be triggered.
        /// </summary>
        private void Enable()
        {
            state = true;
            OnStateChanged();
        }
        
        public override void ColorUpdate(Theme theme)
        {
            ColorScheme currentColors = theme.GetCurrentColorScheme();
            falseColor = overrideFalseColor ? overridenFalseColor : currentColors.m_modeOne;
            trueColor = overrideTrueColor ? overridenTrueColor : currentColors.m_modeTwo;
            m_background.color = state ? trueColor : falseColor;
            m_dot.material.SetColor(CircleColor, overrideDotColor ? overridenDotColor : currentColors.m_background);
        }
    }
}