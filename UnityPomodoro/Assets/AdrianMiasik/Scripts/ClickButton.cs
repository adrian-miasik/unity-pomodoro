using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AdrianMiasik
{
    public class ClickButton : Image, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        public RectTransform target;
        public float clickedDownScale = 0.75f;  // What scale do you want the target to scale to on press?
        public AnimationCurve clickReleaseScale; // What scale do you want the target to scale after click
        public AnimationCurve holdRamp;
        
        // Unity Events
        public UnityEvent OnDown;
        public UnityEvent OnUp;
        public UnityEvent OnClick;
        
        // Press and Release
        private Vector3 cachedScale;
        private bool isAnimatingRelease;
        private float accumulatedReleaseTime;
        
        // Hold
        private bool isUserHolding; // Is the user pressing down?
        private float userHoldTime; // How long has the user been holding down for?
        private float holdActivationTime = 0.5f; // How long does the user have to hold to activate our hold click logic?
        private float accumlatedHoldTime; // How long has the hold logic been running for?
        
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
            isUserHolding = true;
            userHoldTime = 0f;
            accumlatedHoldTime = 0f;

            // Release animation
            isAnimatingRelease = false;
            accumulatedReleaseTime = 0f;

            OnDown.Invoke();
            
            if (target == null)
            {
                return;
            }
            
            // Set target scale to clicked down scale
            target.transform.localScale = Vector3.one * clickedDownScale;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            CancelHold();
            OnUp.Invoke();

            if (target == null)
            {
                return;
            }
            
            // Return target to starting scale
            if (!isAnimatingRelease)
            {
                target.transform.localScale = cachedScale;
            }            
        }

        public bool IsHolding()
        {
            return isUserHolding;
        }
        
        public void CancelHold()
        {
            isUserHolding = false;
            userHoldTime = 0f;
            accumlatedHoldTime = 0f;
            
            if (target != null)
            {
                target.transform.localScale = cachedScale;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick.Invoke();
            
            if (target == null)
            {
                return;
            }
            
            // Start click release animation
            isAnimatingRelease = true;
            accumulatedReleaseTime = 0f;
        }

        private void Update()
        {
            // If user is holding this button down...
            if (isUserHolding)
            {
                userHoldTime += Time.deltaTime;

                if (userHoldTime > holdActivationTime)
                {
                    accumlatedHoldTime += Time.deltaTime;

                    // Calculate how long to wait for before triggering next on click...
                    if (accumlatedHoldTime > holdRamp.Evaluate(userHoldTime))
                    {
                        accumlatedHoldTime = 0f;
                        OnClick.Invoke();
                    }
                }
            }
            
            // If button release is animating...
            if (isAnimatingRelease)
            {
                target.transform.localScale = Vector3.one * clickReleaseScale.Evaluate(accumulatedReleaseTime);
                accumulatedReleaseTime += Time.deltaTime;
                
                // If animation curve is complete...
                if (accumulatedReleaseTime > clickReleaseScale.keys[clickReleaseScale.length - 1].time)
                {
                    isAnimatingRelease = false;
                    target.transform.localScale = cachedScale;
                }
            }
            
        }
    }
}