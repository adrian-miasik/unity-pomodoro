using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AdrianMiasik
{
    public class ClickButton : Image, IPointerDownHandler, IPointerClickHandler, IPointerExitHandler, IPointerEnterHandler
    {
        public RectTransform target;
        
        [Header("Animation")]
        public float clickedDownScale = 0.75f;
        public AnimationCurve clickReleaseScale;

        // Unity Events
        public UnityEvent OnClick;
        
        // Cache
        private Vector3 cachedScale;
        private bool isReleasing = false;
        private float elapsedTime;

        public void OnPointerEnter(PointerEventData eventData)
        {
            cachedScale = target.transform.localScale;
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            target.transform.localScale = Vector3.one * clickedDownScale;
            
            // Reset release animation
            isReleasing = false;
            elapsedTime = 0f;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            target.transform.localScale = cachedScale;

            elapsedTime = 0f;
            isReleasing = true;
            
            OnClick.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            target.transform.localScale = cachedScale;
        }

        private void Update()
        {
            if (isReleasing)
            {
                target.transform.localScale = Vector3.one * clickReleaseScale.Evaluate(elapsedTime);
                elapsedTime += Time.deltaTime;

                // If animation curve is complete...
                if (elapsedTime > clickReleaseScale.keys[clickReleaseScale.length - 1].time)
                {
                    isReleasing = false;
                }
            }
        }
    }
}
