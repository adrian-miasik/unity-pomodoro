using AdrianMiasik.Components.Base;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AdrianMiasik.Components.Specific
{
    // TODO: Rename all instances and references of 'CreditsBubble' to 'CreditsGhost'
    /// <summary>
    /// A <see cref="TimerProgress"/> inheritor used for displaying the authors name of the app. Intended to minimize
    /// after a couple seconds via the base class.
    /// </summary>
    // public class CreditsBubble : TimerProgress, IPointerEnterHandler, IPointerExitHandler, IColorHook
    public class CreditsGhost : Ghost, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private RectTransform m_self;
        [SerializeField] private SimpleTimer m_timer;

        private bool isPointerHovering;
        private bool lockInteraction;

        private float cachedWidthPercentage;
        private float cachedRightOffsetPixels;

        public override void Initialize(PomodoroTimer pomodoroTimer, bool updateColors = true)
        {
            base.Initialize(pomodoroTimer, updateColors);
            
            // Cache
            cachedWidthPercentage = m_self.anchorMax.x;
            cachedRightOffsetPixels = m_self.offsetMax.x;

            // Setup
            m_timer.m_onComplete.AddListener(OnTimerComplete);
            m_timer.Initialize(3);
            Lock();
        }

        public void Restart()
        {
            m_timer.Initialize(3);
            Lock();
        }

        private void OnTimerComplete()
        {
            if (!isPointerHovering && !Timer.IsAboutPageOpen())
            {
                if (!Timer.IsSidebarOpen())
                {
                    FadeOut();
                }
            }
            
            if (!Timer.IsAboutPageOpen())
            {
                Unlock();
            }
        }

        public void Lock()
        {
            lockInteraction = true;
        }

        public void Unlock()
        {
            lockInteraction = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isPointerHovering = true;
            
            if (lockInteraction)
            {
                return;
            }
            
            FadeIn();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isPointerHovering = false;
            
            if (lockInteraction)
            {
                return;
            }
            
            FadeOut();
        }

        public void SetWidth(float desiredWidthPercentage)
        {
            m_self.anchorMax = new Vector2(desiredWidthPercentage, m_self.anchorMax.y);
        }

        public void SetRightOffset(float rightOffsetInPixels)
        {
            m_self.offsetMax = new Vector2(rightOffsetInPixels, m_self.offsetMax.y);
        }

        public void ResetWidth()
        {
            m_self.anchorMax = new Vector2(cachedWidthPercentage, m_self.anchorMax.y);
        }

        public void ResetRightOffset()
        {
            m_self.offsetMax = new Vector2(cachedRightOffsetPixels, m_self.offsetMax.y);
        }
    }
}