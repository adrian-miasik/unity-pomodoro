using System.Collections.Generic;
using AdrianMiasik.Components.Core;
using AdrianMiasik.Interfaces;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AdrianMiasik.Components
{
    public class CreditsBubble : TimerProgress, IPointerEnterHandler, IPointerExitHandler, IColorHook
    {
        [SerializeField] private SVGImage background;
        [SerializeField] private CanvasGroup backgroundContainer;
        [SerializeField] private UPIcon icon;
        [SerializeField] private CanvasGroup textContainer;
        [SerializeField] private List<TMP_Text> text = new List<TMP_Text>();
        
        private bool isAnimating;
        private float accumulatedTime;
        private float fadeTime = 0.5f;
        private float fadeProgress;
        private float targetAlpha;

        private bool isPointerHovering;
        private bool lockInteraction;

        private PomodoroTimer timer;
        private Theme theme;

        private enum FadeState
        {
            IDLE,
            FADING_OUT,
            FADING_IN
        }

        private FadeState state;

        public void Initialize(PomodoroTimer timer, Theme theme)
        {
            this.timer = timer;
            this.theme = theme;
            
            // Setup
            Initialize(duration);
            Lock();
            
            // Theme
            theme.RegisterColorHook(this);
            background.color = theme.GetCurrentColorScheme().backgroundHighlight;
            ColorUpdate(theme);
        }
        
        protected override void OnUpdate(float progress)
        {
            // Nothing
        }

        protected override void OnComplete()
        {
            if (!isPointerHovering && !timer.IsInfoPageOpen())
            {
                FadeOut();
            }
            
            Unlock();
        }

        protected override void Update()
        {
            if (isAnimating)
            {
                if (state == FadeState.FADING_OUT || state == FadeState.FADING_IN)
                {
                    accumulatedTime += Time.deltaTime;
                    fadeProgress = accumulatedTime / fadeTime;
                    
                    if (state == FadeState.FADING_IN)
                    {
                        backgroundContainer.alpha = fadeProgress;
                        textContainer.alpha = fadeProgress;
                    }
                    else
                    {
                        textContainer.alpha = fadeProgress * -1 + 1;
                        backgroundContainer.alpha = fadeProgress * -1 + 1;
                    }

                    if (accumulatedTime >= fadeTime)
                    {
                        state = FadeState.IDLE;
                        isAnimating = false;
                    }
                }
            }

            base.Update();
        }

        public void FadeOut()
        {
            foreach (TMP_Text text in text)
            {
                text.color = theme.GetCurrentColorScheme().foreground;
            }
            
            targetAlpha = 0;
            
            state = FadeState.FADING_OUT;
            accumulatedTime = 0;
            isAnimating = true;
        }

        public void FadeIn()
        {
            foreach (TMP_Text text in text)
            {
                text.color = theme.GetCurrentColorScheme().foreground;
            }

            targetAlpha = 1;
            
            state = FadeState.FADING_IN;
            accumulatedTime = 0;
            isAnimating = true;
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
            background.color = theme.GetCurrentColorScheme().backgroundHighlight;
            
            foreach (TMP_Text text in text)
            {
                text.color = theme.GetCurrentColorScheme().foreground;
            }
            
            icon.ColorUpdate(theme);
        }
    }
}