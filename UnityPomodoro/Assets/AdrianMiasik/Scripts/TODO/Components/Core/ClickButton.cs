using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace AdrianMiasik.Components.Core
{
    public class ClickButton : Image, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IPointerExitHandler
    {
        // Inspector References
        [FormerlySerializedAs("containerTarget")] public RectTransform m_containerTarget; // The rect transform that will scale
        [FormerlySerializedAs("enableClickSound")] public bool m_enableClickSound = true;
        [FormerlySerializedAs("clickSound")] public AudioSource m_clickSound;
        [FormerlySerializedAs("clickHoldScale")] public float m_clickHoldScale = 0.75f;  // What scale do you want the target to scale to on press?
        [FormerlySerializedAs("clickReleaseScale")] public AnimationCurve m_clickReleaseScale; // What scale do you want the target to scale after click
        [FormerlySerializedAs("holdRamp")] public AnimationCurve m_holdRamp;

        // Unity Events
        [FormerlySerializedAs("OnDown")] public UnityEvent m_onDown;
        [FormerlySerializedAs("OnUp")] public UnityEvent m_onUp;
        [FormerlySerializedAs("OnClick")] public UnityEvent m_onClick;
        
        // Press and Release
        private Vector3 cachedScale;
        private bool isAnimatingRelease;
        private float accumulatedReleaseTime;
        
        // Hold
        private bool isUserHolding; // Is the user pressing down?
        private float userHoldTime; // How long has the user been holding down for?
        private float holdActivationTime = 0.25f; // How long does the user have to hold to activate our hold click logic?
        private float accumulatedHoldTime; // How long has the hold logic been running for? Not to be confused with userHoldTime.
        
        // Click sound pitch variation
        [FormerlySerializedAs("isPitchVariationOn")] public bool m_isPitchVariationOn = true;
        [FormerlySerializedAs("lowestPitch")] public float m_lowestPitch = 0.9f;
        [FormerlySerializedAs("highestPitch")] public float m_highestPitch = 1.1f;

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
            Application.OpenURL(_url);
#endif
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isUserHolding = false;
        }
    }
}