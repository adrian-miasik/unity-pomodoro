using System.Collections.Generic;
using AdrianMiasik.Components.Core;
using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AdrianMiasik.Components
{
    public class CreditsBubble : TimerProgress, IPointerEnterHandler, IPointerExitHandler, IColorHook
    {
        [SerializeField] private SVGImage m_background;
        [SerializeField] private CanvasGroup m_backgroundContainer;
        [SerializeField] private UPIcon m_icon;
        [SerializeField] private CanvasGroup m_textContainer;
        [SerializeField] private List<TMP_Text> m_text = new List<TMP_Text>();
        [Tooltip("E.g. 0.5f = fade time of 2 seconds, 2 = fade time of 0.5 seconds.")]
        [SerializeField] private float m_fadeSpeed = 2f;
        
        private float fadeProgress = 1;

        private bool isPointerHovering;
        private bool lockInteraction;

        private PomodoroTimer timer;

        private enum FadeState
        {
            IDLE,
            FADING_OUT,
            FADING_IN
        }

        private FadeState state;

        public void Initialize(PomodoroTimer pomodoroTimer)
        {
            timer = pomodoroTimer;

            // Setup
            Initialize(m_duration);
            Lock();
            fadeProgress = 1; // Starts at one since bubble is visible
            
            // Theme Element
            timer.GetTheme().RegisterColorHook(this);
            m_background.color = timer.GetTheme().GetCurrentColorScheme().m_backgroundHighlight;
            ColorUpdate(timer.GetTheme());
        }

        protected override void OnUpdate(float progress)
        {
            // Nothing
        }

        protected override void OnComplete()
        {
            if (!isPointerHovering && !timer.IsAboutPageOpen())
            {
                FadeOut();
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

        public void FadeOut()
        {
            foreach (TMP_Text text in m_text)
            {
                text.color = timer.GetTheme().GetCurrentColorScheme().m_foreground;
            }

            state = FadeState.FADING_OUT;
        }

        public void FadeIn()
        {
            foreach (TMP_Text text in m_text)
            {
                text.color = timer.GetTheme().GetCurrentColorScheme().m_foreground;
            }

            state = FadeState.FADING_IN;
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

        public void ColorUpdate(Theme theme)
        {
            m_background.color = timer.IsSidebarOpen() ?
                theme.GetCurrentColorScheme().m_background : 
                theme.GetCurrentColorScheme().m_backgroundHighlight;

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