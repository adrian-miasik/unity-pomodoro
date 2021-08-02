using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AdrianMiasik.Components.Core
{
    public class ClickButton : Image, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        // Inspector References
        public RectTransform containerTarget; // The rect transform that will scale
        public Transform visibilityTarget;
        public AudioSource clickSound;
        public float clickHoldScale = 0.75f;  // What scale do you want the target to scale to on press?
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
        private float accumlatedHoldTime; // How long has the hold logic been running for? Not to be confused with userHoldTime.
        
        // Click sound pitch variation
        public bool isPitchVariationOn = true;
        public float lowestPitch = 0.95f;
        public float highestPitch = 1.05f;

        protected override void Start()
        {
            base.Start();
            if (containerTarget == null)
            {
                Debug.LogWarning("Target is missing", this);
                return;
            }
            
            cachedScale = containerTarget.localScale;
        }
        
        public void Hide()
        {
            Release();

            raycastTarget = false;
            visibilityTarget.gameObject.SetActive(false);
        }

        public void Show()
        {
            raycastTarget = true;
            visibilityTarget.gameObject.SetActive(true);
        }

        public void Hold()
        {
            OnPointerDown(null);
        }
        
        public void Release()
        {
            if (IsHolding())
            {
                CancelHold();
            }
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
            
            if (containerTarget == null)
            {
                return;
            }
            
            // Set target scale to clicked down scale
            containerTarget.transform.localScale = Vector3.one * clickHoldScale;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            CancelHold();
            OnUp.Invoke();

            if (containerTarget == null)
            {
                return;
            }
            
            // Return target to starting scale
            if (!isAnimatingRelease)
            {
                containerTarget.transform.localScale = cachedScale;
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
            
            if (containerTarget != null)
            {
                containerTarget.transform.localScale = cachedScale;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick.Invoke();
            PlayClickSound();
            
            if (containerTarget == null)
            {
                return;
            }
            
            // Start click release animation
            isAnimatingRelease = true;
            accumulatedReleaseTime = 0f;
        }

        private void PlayClickSound()
        {
            if (isPitchVariationOn)
            {
                clickSound.pitch = Random.Range(lowestPitch, highestPitch);
            }
            else
            {
                // Reset pitch back to default
                clickSound.pitch = 1;
            }

            clickSound.Play();
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
                        PlayClickSound();
                    }
                }
            }
            
            // If button release is animating...
            if (isAnimatingRelease)
            {
                containerTarget.transform.localScale = Vector3.one * clickReleaseScale.Evaluate(accumulatedReleaseTime);
                accumulatedReleaseTime += Time.deltaTime;
                
                // If animation curve is complete...
                if (accumulatedReleaseTime > clickReleaseScale.keys[clickReleaseScale.length - 1].time)
                {
                    isAnimatingRelease = false;
                    containerTarget.transform.localScale = cachedScale;
                }
            }
            
        }

        public void OpenURL(string url)
        {
            Application.OpenURL(url);
        }
    }
}