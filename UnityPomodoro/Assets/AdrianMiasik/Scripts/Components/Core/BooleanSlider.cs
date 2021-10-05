using AdrianMiasik.Interfaces;
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

        public UnityEvent OnSetToTrueClick; // clicking a disabled boolean
        public UnityEvent OnSetToFalseClick; // clicking on an enabled boolean
        public UnityEvent OnClick;
        
        // Shader Property
        private static readonly int CircleColor = Shader.PropertyToID("Color_297012532bf444df807f8743bdb7e4fd");

        // Cache
        private bool state = false;
        private Color trueColor;
        private Color falseColor;

        public void Initialize(bool state, Theme theme)
        {
            this.state = state;
            theme.RegisterColorHook(this);
            ColorUpdate(theme);
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
            
            OnClick.Invoke();
        }
        
        private void OnStateChanged(bool invokeEvents = false)
        {
            if (state)
            {
                // Set to on position
                animation.clip = leftToRight;
                background.color = trueColor;

                if (invokeEvents)
                {
                    OnSetToTrueClick.Invoke();
                }
            }
            else
            {
                // Set to off position
                animation.clip = rightToLeft;
                background.color = falseColor;

                if (invokeEvents)
                {
                    OnSetToFalseClick.Invoke();
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
        
        public void ColorUpdate(Theme theme)
        {
            ColorScheme currentColors = theme.GetCurrentColorScheme();
            falseColor = currentColors.backgroundHighlight;
            trueColor = currentColors.modeTwo;
            background.color = state ? trueColor : falseColor;
            dot.material.SetColor(CircleColor, currentColors.background);
        }
    }
}
