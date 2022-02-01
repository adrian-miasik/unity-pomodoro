using System.Collections.Generic;
using AdrianMiasik.Components.Base;
using AdrianMiasik.Components.Core;
using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AdrianMiasik.Components.Specific
{
    // TODO: Create a wrapper class for the (To be created) Bubble base class which inherits TimerProgress?
    /// <summary>
    /// A <see cref="TimerProgress"/> inheritor used for displaying the authors name of the app. Intended to minimize
    /// after a couple seconds via the base class.
    /// </summary>
    public class CreditsBubble : TimerProgress, IPointerEnterHandler, IPointerExitHandler, IColorHook
    {
        [SerializeField] private RectTransform m_self;
        [SerializeField] private SVGImage m_background;
        [SerializeField] private CanvasGroup m_backgroundContainer;
        [SerializeField] private ThemeIcon m_icon;
        [SerializeField] private CanvasGroup m_textContainer;
        [SerializeField] private List<TMP_Text> m_text = new List<TMP_Text>();
        [Tooltip("E.g. 0.5f = fade time of 2 seconds, 2 = fade time of 0.5 seconds.")]
        [SerializeField] private float m_fadeSpeed = 2f;
        
        private float fadeProgress = 1;

        private bool isPointerHovering;
        private bool lockInteraction;

        private float cachedWidthPercentage;
        private float cachedRightOffsetPixels;

        private PomodoroTimer timer;

        private enum FadeState
        {
            IDLE,
            FADING_OUT,
            FADING_IN
        }

        private FadeState state;

        /// <summary>
        /// Sets up our component, and registers the <see cref="IColorHook"/> to the active <see cref="Theme"/>.
        /// </summary>
        /// <param name="pomodoroTimer"></param>
        public void Initialize(PomodoroTimer pomodoroTimer)
        {
            // Cache
            timer = pomodoroTimer;
            cachedWidthPercentage = m_self.anchorMax.x;
            cachedRightOffsetPixels = m_self.offsetMax.x;

            // Setup
            Initialize(m_duration);
            Lock();

            // Theme Element
            timer.GetTheme().Register(this);
            m_background.color = timer.GetTheme().GetCurrentColorScheme().m_backgroundHighlight;
            ColorUpdate(timer.GetTheme());
        }

        protected override void OnStart()
        {
            
        }

        protected override void OnUpdate(float progress)
        {
            
        }
        
        protected override void OnComplete()
        {
            if (!isPointerHovering && !timer.IsAboutPageOpen())
            {
                if (!timer.IsSidebarOpen())
                {
                    FadeOut();
                }
            }

            if (!timer.IsAboutPageOpen())
            {
                Unlock();
            }
        }

        protected override void Update()
        {
            if (state != FadeState.IDLE)
            {
                if (state == FadeState.FADING_IN)
                {
                    fadeProgress += Time.deltaTime * m_fadeSpeed;
                }
                else if (state == FadeState.FADING_OUT)
                {
                    fadeProgress -= Time.deltaTime * m_fadeSpeed;
                }
                
                fadeProgress = Mathf.Clamp01(fadeProgress);
                m_textContainer.alpha = fadeProgress;
                m_backgroundContainer.alpha = fadeProgress;
                
                // If fade is completed...
                if (fadeProgress <= 0 || fadeProgress >= 1)
                {
                    state = FadeState.IDLE;
                }
            }

            base.Update();
        }

        public void FadeOut(bool instantly = false)
        {
            foreach (TMP_Text text in m_text)
            {
                text.color = timer.GetTheme().GetCurrentColorScheme().m_foreground;
            }

            if (instantly)
            {
                state = FadeState.IDLE;
                m_textContainer.alpha = 1;
                m_backgroundContainer.alpha = 1;
            }
            else
            {
                state = FadeState.FADING_OUT;
            }
        }

        public void FadeIn(bool instantly = false)
        {
            foreach (TMP_Text text in m_text)
            {
                text.color = timer.GetTheme().GetCurrentColorScheme().m_foreground;
            }

            if (instantly)
            {
                state = FadeState.IDLE;
                m_textContainer.alpha = 1;
                m_backgroundContainer.alpha = 1;
            }
            else
            {
                state = FadeState.FADING_IN;
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

        public void ColorUpdate(Theme theme)
        {
            m_background.color = theme.GetCurrentColorScheme().m_backgroundHighlight;

            foreach (TMP_Text text in m_text)
            {
                text.color = theme.GetCurrentColorScheme().m_foreground;
            }

            m_icon.ColorUpdate(theme);
        }

        public void OnDestroy()
        {
            timer.GetTheme().Deregister(this);
        }
    }
}