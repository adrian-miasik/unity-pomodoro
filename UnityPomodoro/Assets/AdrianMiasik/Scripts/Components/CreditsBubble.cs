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
        [Tooltip("E.g. 0.5f = fade time of 2 seconds, 2 = fade time of 0.5 seconds.")]
        [SerializeField] private float fadeSpeed = 2f;
        
        private float fadeProgress = 1;

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

        public void Initialize(PomodoroTimer _timer)
        {
            timer = _timer;
            theme = timer.GetTheme();
            
            // Setup
            Initialize(duration);
            Lock();
            fadeProgress = 1; // Starts at one since bubble is visible
            
            // Theme
            theme.RegisterColorHook(this);
            background.color = theme.GetCurrentColorScheme().backgroundHighlight;
            ColorUpdate(theme);
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

            if (!timer.IsInfoPageOpen())
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
                    fadeProgress += Time.deltaTime * fadeSpeed;
                }
                else if (state == FadeState.FADING_OUT)
                {
                    fadeProgress -= Time.deltaTime * fadeSpeed;
                }
                
                fadeProgress = Mathf.Clamp01(fadeProgress);
                textContainer.alpha = fadeProgress;
                backgroundContainer.alpha = fadeProgress;
                
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
            foreach (TMP_Text _text in text)
            {
                _text.color = theme.GetCurrentColorScheme().foreground;
            }

            state = FadeState.FADING_OUT;
        }

        public void FadeIn()
        {
            foreach (TMP_Text _text in text)
            {
                _text.color = theme.GetCurrentColorScheme().foreground;
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