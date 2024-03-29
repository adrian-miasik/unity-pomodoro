using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AdrianMiasik.Components.Base
{
    /// <summary>
    /// A **generic** button that is used to interact with the software and trigger events based on user input.
    /// Supports animations, click hold curves, unity events, and sound FX (with pitch variation).
    /// <remarks>Intended to be used on all our application buttons.</remarks>
    /// </summary>
    public class ClickButton : Image, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IPointerExitHandler
    {
        // Inspector References
        public RectTransform m_containerTarget; // The rect transform that will scale
        public bool m_enableClickSound = true;
        public AudioSource m_clickSound;
        public bool m_isPitchVariationOn = true;
        public float m_lowestPitch = 0.9f;
        public float m_highestPitch = 1.1f;
        
        /// <summary>
        /// What transform scale do you want the target to scale to on button press?
        /// </summary>
        public float m_clickHoldScale = 0.75f;
        
        /// <summary>
        /// What transform scale (animation over time) do you want the target to scale to on button release/after click?
        /// Used as a release animation.
        /// </summary>
        public AnimationCurve m_clickReleaseScale;
        
        /// <summary>
        /// How fast do you want to press/click the button when user is holding down the button? Initially set to be
        /// somewhat exponentially faster. This way the longer the user holds, the faster we invoke the next click.
        /// Ideally scenario is incrementing / decrementing our timer digits.
        /// </summary>
        public AnimationCurve m_holdRamp;

        /// <summary>
        /// Can the user invoke this button more than once by holding it down?
        /// </summary>
        public bool m_isHoldable = true;

        // Unity Events
        public UnityEvent m_onDown;
        public UnityEvent m_onUp;
        public UnityEvent m_onClick;
        
        // Press and Release
        private Vector3 cachedScale = Vector3.one;
        private bool isAnimatingRelease;
        private float accumulatedReleaseTime;
        
        // Hold
        private bool isUserHolding; // Is the user pressing down?
        private float userHoldTime; // How long has the user been holding down for?
        private float holdActivationTime = 0.25f; // How long does the user have to hold to activate our hold click logic?
        private float accumulatedHoldTime; // How long has the hold logic been running for? Not to be confused with userHoldTime.
        
        protected override void Start()
        {
            base.Start();
            if (m_containerTarget == null)
            {
                Debug.LogWarning("Target is missing", this);
                return;
            }
            
            cachedScale = m_containerTarget.localScale;
        }
        
        public virtual void Hide()
        {
            Release();

            raycastTarget = false;
        }

        public virtual void Show()
        {
            raycastTarget = true;
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
            accumulatedHoldTime = 0f;

            // Release animation
            isAnimatingRelease = false;
            accumulatedReleaseTime = 0f;

            m_onDown.Invoke();
            
            if (m_containerTarget == null)
            {
                return;
            }
            
            // Set target scale to clicked down scale
            m_containerTarget.transform.localScale = Vector3.one * m_clickHoldScale;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            CancelHold();
            m_onUp.Invoke();

            if (m_containerTarget == null)
            {
                return;
            }
            
            // Return target to starting scale
            if (!isAnimatingRelease)
            {
                m_containerTarget.transform.localScale = cachedScale;
            }            
        }

        private bool IsHolding()
        {
            return isUserHolding;
        }
        
        public void CancelHold()
        {
            isUserHolding = false;
            userHoldTime = 0f;
            accumulatedHoldTime = 0f;

            if (m_containerTarget != null)
            {
                m_containerTarget.transform.localScale = cachedScale;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            m_onClick.Invoke();

            if (m_enableClickSound)
            {
                PlayClickSound();
            }

            if (m_containerTarget == null)
            {
                return;
            }
            
            // Start click release animation
            isAnimatingRelease = true;
            accumulatedReleaseTime = 0f;
        }

        private void PlayClickSound()
        {
            if (m_isPitchVariationOn)
            {
                m_clickSound.pitch = Random.Range(m_lowestPitch, m_highestPitch);
            }
            else
            {
                // Reset pitch back to default
                m_clickSound.pitch = 1;
            }

            m_clickSound.Play();
        }

        private void Update()
        {
            // If user is holding this button down...
            if (isUserHolding)
            {
                userHoldTime += Time.deltaTime;

                if (userHoldTime > holdActivationTime)
                {
                    accumulatedHoldTime += Time.deltaTime;

                    // Calculate how long to wait for before triggering next on click...
                    if (accumulatedHoldTime > m_holdRamp.Evaluate(userHoldTime))
                    {
                        accumulatedHoldTime = 0f;
                        if (m_enableClickSound)
                        {
                            PlayClickSound();
                        }
                        m_onClick.Invoke();

                        if (!m_isHoldable)
                        {
                            CancelHold();
                        }
                    }
                }
            }
            
            // If button release is animating...
            if (isAnimatingRelease)
            {
                m_containerTarget.transform.localScale = Vector3.one * m_clickReleaseScale.Evaluate(accumulatedReleaseTime);
                accumulatedReleaseTime += Time.deltaTime;
                
                // If animation curve is complete...
                if (accumulatedReleaseTime > m_clickReleaseScale.keys[m_clickReleaseScale.length - 1].time)
                {
                    isAnimatingRelease = false;
                    m_containerTarget.transform.localScale = cachedScale;
                }
            }
            
        }

        public void OpenURL(string url)
        {
#if ENABLE_WINMD_SUPPORT
            UnityEngine.WSA.Launcher.LaunchUri(url, true);
#else
            Application.OpenURL(url);
#endif
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isUserHolding = false;
        }
    }
}