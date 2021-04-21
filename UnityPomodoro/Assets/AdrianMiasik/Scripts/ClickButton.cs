using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AdrianMiasik
{
    public class ClickButton : Image, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        public RectTransform target;

        [Header("Animation")] public float clickedDownScale = 0.75f;

        public AnimationCurve clickReleaseScale;

        // Unity Events
        public UnityEvent OnDown;
        public UnityEvent OnUp;
        public UnityEvent OnClick;
        
        // Cache
        private Vector3 cachedScale;
        private bool isAnimating;
        private float elapsedTime;

        protected override void Start()
        {
            base.Start();
            if (target == null)
            {
                Debug.LogWarning("Target is missing", this);
                return;
            }
            
            cachedScale = target.localScale;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (target == null)
            {
                return;
            }
            
            target.transform.localScale = Vector3.one * clickedDownScale;

            // Release animation
            isAnimating = false;
            elapsedTime = 0f;

            OnDown.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (target == null)
            {
                return;
            }
            
            if (!isAnimating)
            {
                target.transform.localScale = cachedScale;
            }

            OnUp.Invoke();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (target == null)
            {
                return;
            }
            
            // Start animation
            isAnimating = true;
            elapsedTime = 0f;

            OnClick.Invoke();
        }

        private void Update()
        {
            if (isAnimating)
            {
                target.transform.localScale = Vector3.one * clickReleaseScale.Evaluate(elapsedTime);
                elapsedTime += Time.deltaTime;

                // If animation curve is complete...
                if (elapsedTime > clickReleaseScale.keys[clickReleaseScale.length - 1].time)
                {
                    isAnimating = false;
                    target.transform.localScale = cachedScale;
                }
            }
        }
    }
}
