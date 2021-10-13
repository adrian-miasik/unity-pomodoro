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
        [SerializeField] private SVGImage background;
        [SerializeField] private CanvasGroup backgroundContainer;
        [SerializeField] private UPIcon icon;
        [SerializeField] private CanvasGroup textContainer;
        [SerializeField] private List<TMP_Text> text = new List<TMP_Text>();
        
        private bool isAnimating;
        private float accumulatedTime;
        private readonly float fadeTime = 0.5f;
        private float fadeProgress;

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

        public void Initialize(PomodoroTimer _timer, Theme _theme)
        {
            timer = _timer;
            theme = _theme;
            
            // Setup
            Initialize(duration);
            Lock();
            
            // Theme
            _theme.RegisterColorHook(this);
            background.color = _theme.GetCurrentColorScheme().backgroundHighlight;
            ColorUpdate(_theme);
        }
        
        protected override void OnUpdate(float _progress)
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
            foreach (TMP_Text _text in text)
            {
                _text.color = theme.GetCurrentColorScheme().foreground;
            }
            
            state = FadeState.FADING_OUT;
            accumulatedTime = 0;
            isAnimating = true;
        }

        public void FadeIn()
        {
            foreach (TMP_Text _text in text)
            {
                _text.color = theme.GetCurrentColorScheme().foreground;
            }
            
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

        public void OnPointerEnter(PointerEventData _eventData)
        {
            isPointerHovering = true;
            
            if (lockInteraction)
            {
                return;
            }
            
            FadeIn();
        }

        public void OnPointerExit(PointerEventData _eventData)
        {
            isPointerHovering = false;
            
            if (lockInteraction)
            {
                return;
            }
            
            FadeOut();
        }

        public void ColorUpdate(Theme _theme)
        {
            background.color = _theme.GetCurrentColorScheme().backgroundHighlight;
            
            foreach (TMP_Text _text in text)
            {
                _text.color = _theme.GetCurrentColorScheme().foreground;
            }
            
            icon.ColorUpdate(_theme);
        }
    }
}