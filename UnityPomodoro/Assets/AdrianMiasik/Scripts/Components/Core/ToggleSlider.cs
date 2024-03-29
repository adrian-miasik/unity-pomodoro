using AdrianMiasik.Components.Base;
using AdrianMiasik.ScriptableObjects;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace AdrianMiasik.Components.Core
{
    /// <summary>
    /// A <see cref="ThemeElement"/> <see cref="Base.Toggle"/> in the form of a boolean slider.
    /// </summary>
    public class ToggleSlider : Toggle
    {
        // General
        [SerializeField] public SVGImage m_background;
        [SerializeField] public SVGImage m_dot;

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

        public void Initialize(PomodoroTimer timer, bool state)
        {
            base.Initialize(timer);

            isOn = state;
            if (isOn)
            {
                Enable();
            }
            else
            {
                Disable();
            }
        }

        public bool IsOn()
        {
            return isOn;
        }

        public void Refresh(bool state)
        {
            // Early exit if there are no changes to our property
            if (state == isOn)
            {
                return;
            }
            
            isOn = state;
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

        public override void OnPointerClick(PointerEventData eventData)
        { 
            // Flip state
            isOn = !isOn;
            OnStateChanged(true); // User interacted with this, we treating this as a click
            
            m_onClick.Invoke();
        }
        
        private void OnStateChanged(bool invokeEvents = false)
        {
            if (isOn)
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
            isOn = false;
            OnStateChanged();
        }

        /// <summary>
        /// Changes the visibility of the boolean slider to the ON position (right).
        /// Note: OnClick Unity Events won't be triggered.
        /// </summary>
        private void Enable()
        {
            isOn = true;
            OnStateChanged();
        }
        
        public override void ColorUpdate(Theme theme)
        {
            ColorScheme currentColors = theme.GetCurrentColorScheme();
            if (theme.IsDarkMode())
            {
                falseColor = overrideFalseColor ? overridenFalseColor : currentColors.m_foreground;
            }
            else
            {
                falseColor = overrideFalseColor ? overridenFalseColor : currentColors.m_backgroundHighlight;
            }
            
            trueColor = overrideTrueColor ? overridenTrueColor : currentColors.m_modeOne;
            m_background.color = isOn ? trueColor : falseColor;

            Material dotMaterial = new(m_dot.material);
            dotMaterial.SetColor(CircleColor, overrideDotColor ? overridenDotColor : currentColors.m_background);
            m_dot.material = dotMaterial;
        }

        [ContextMenu("Enable (Run-time)")]
        public void SetVisualToEnable()
        {
            m_animation.enabled = false;
            Enable();
            ColorUpdate(Timer.GetTheme());
            m_dot.rectTransform.anchorMin = new Vector2(0.45f, 0);
            m_dot.rectTransform.anchorMax = Vector2.one;
        }
        
        [ContextMenu("Disable (Run-time)")]
        public void SetVisualToDisable()
        {
            m_animation.enabled = false;
            Disable();
            ColorUpdate(Timer.GetTheme());
            m_dot.rectTransform.anchorMin = Vector2.zero;
            m_dot.rectTransform.anchorMax = new Vector2(0.55f, 1); 
        }
        
        [ContextMenu("Enable (Editor)")]
        public void SetEditorVisualToEnable()
        {
            m_background.color = new Color(0.05f, 0.47f, 0.95f);
            m_dot.rectTransform.anchorMin = new Vector2(0.45f, 0);
            m_dot.rectTransform.anchorMax = Vector2.one;
        }
        
        [ContextMenu("Disable (Editor)")]
        public void SetEditorVisualToDisable()
        {
            m_background.color = new Color(0.91f, 0.91f, 0.91f);
            m_dot.rectTransform.anchorMin = Vector2.zero;
            m_dot.rectTransform.anchorMax = new Vector2(0.55f, 1); 
        }

        public void EnableAnimation()
        {
            m_animation.enabled = true;
        }
    }
}