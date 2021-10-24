using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AdrianMiasik.Components.Core
{
    public class BooleanSlider : MonoBehaviour, IPointerClickHandler, IColorHook
    {
        [SerializeField] public SVGImage background;
        [SerializeField] private Image dot;
        [SerializeField] private new Animation animation;

        [SerializeField] private AnimationClip leftToRight;
        [SerializeField] private AnimationClip rightToLeft;

        public UnityEvent onSetToTrueClick; // clicking a disabled boolean
        public UnityEvent onSetToFalseClick; // clicking on an enabled boolean
        public UnityEvent onClick;
        
        // Shader Property
        private static readonly int CircleColor = Shader.PropertyToID("Color_297012532bf444df807f8743bdb7e4fd");

        // Cache
        private bool state;
        private Color trueColor;
        private Color falseColor;

        private bool overrideFalseColor;
        private Color overridenFalseColor;
        
        /// <summary>
        /// Note: Needs to be invoked before initialize
        /// </summary>
        /// <param name="_color"></param>
        public void OverrideFalseColor(Color _color)
        {
            overrideFalseColor = true;
            overridenFalseColor = _color;
        }

        public void Initialize(PomodoroTimer _timer, bool _state)
        {
            state = _state;
            _timer.GetTheme().RegisterColorHook(this);
            ColorUpdate(_timer.GetTheme());
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

        public void OnPointerClick(PointerEventData _eventData)
        {
            // Flip state
            state = !state;
            OnStateChanged(true); // User interacted with this, we treating this as a click
            
            onClick.Invoke();
        }
        
        private void OnStateChanged(bool _invokeEvents = false)
        {
            if (state)
            {
                // Set to on position
                animation.clip = leftToRight;
                background.color = trueColor;

                if (_invokeEvents)
                {
                    onSetToTrueClick.Invoke();
                }
            }
            else
            {
                // Set to off position
                animation.clip = rightToLeft;
                background.color = falseColor;

                if (_invokeEvents)
                {
                    onSetToFalseClick.Invoke();
                }
            }

            animation.Play();
        }

        /// <summary>
        /// Changes the visibility of the boolean slider to the OFF position (left).
        /// Note: OnClick Unity Events won't be triggered.
        /// </summary>
        public void Disable()
        {
            state = false;
            OnStateChanged();
        }

        /// <summary>
        /// Changes the visibility of the boolean slider to the ON position (right).
        /// Note: OnClick Unity Events won't be triggered.
        /// </summary>
        public void Enable()
        {
            state = true;
            OnStateChanged();
        }
        
        public void ColorUpdate(Theme _theme)
        {
            ColorScheme _currentColors = _theme.GetCurrentColorScheme();
            falseColor = overrideFalseColor ? overridenFalseColor : _currentColors.modeOne;
            trueColor = _currentColors.modeTwo;
            background.color = state ? trueColor : falseColor;
            dot.material.SetColor(CircleColor, _currentColors.background);
        }
    }
}