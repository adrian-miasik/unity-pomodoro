using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace AdrianMiasik
{
    public class BooleanSlider : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private SVGImage background;
        [SerializeField] private new Animation animation;

        [SerializeField] private AnimationClip leftToRight;
        [SerializeField] private AnimationClip rightToLeft;

        public UnityEvent OnSetToTrueClick; // clicking a disabled boolean
        public UnityEvent OnSetToFalseClick; // clicking on an enabled boolean

        // Cache
        private bool state = false;
        private Color offColor;
        private Color onColor;

        public void Initialize(bool state, Color offColor, Color onColor)
        {
            this.state = state;
            this.offColor = offColor;
            this.onColor = onColor;

            // Set background color to match state
            background.color = state ? onColor : offColor;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // Flip state
            state = !state;
            OnStateChanged(true); // User interacted with this, we treating this as a click
        }

        /// <summary>
        /// Changes the visibility of the boolean slider to the ON or OFF position. Position depends on current state.
        /// Note: OnClick Unity Events won't be triggered.
        /// </summary>
        public void FlipState()
        {
            state = !state;
            OnStateChanged();
        }

        private void OnStateChanged(bool invokeEvents = false)
        {
            if (state)
            {
                // Set to on position
                animation.clip = leftToRight;
                background.color = onColor;

                if (invokeEvents)
                {
                    OnSetToTrueClick.Invoke();
                }
            }
            else
            {
                // Set to off position
                animation.clip = rightToLeft;
                background.color = offColor;

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
    }
}
