using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AdrianMiasik
{
    public class ClickButton : Image, IPointerDownHandler, IPointerClickHandler, IPointerExitHandler
    {
        public bool interactable = true;
        public RectTransform target;

        [Header("Animation")]
        public float clickedDownScale = 0.75f;

        public AnimationCurve clickReleaseScale;

        // Unity Events
        public UnityEvent OnClick;


        // Cache
        private Vector3 cachedScale;
        private bool isAnimating;
        private float elapsedTime;

        protected override void Start()
        {
            base.Start();
            cachedScale = target.localScale;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!interactable)
            {
                return;
            }
            
            cachedScale = target.transform.localScale;
            target.transform.localScale = Vector3.one * clickedDownScale;

            // Release animation
            isAnimating = false;
            elapsedTime = 0f;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!interactable)
            {
                return;
            }
            
            // Start animation
            isAnimating = true;
            elapsedTime = 0f;
            
            OnClick.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!isAnimating)
            {
                target.transform.localScale = cachedScale;
            }
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

        public void Hide()
        {
            interactable = false;
            target.gameObject.SetActive(false);
        }

        public void Show()
        {
            interactable = true;
            target.gameObject.SetActive(true);
        }
    }
}
