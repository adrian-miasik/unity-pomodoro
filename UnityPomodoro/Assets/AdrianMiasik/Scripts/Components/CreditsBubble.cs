using AdrianMiasik.Components.Core;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AdrianMiasik.Components
{
    public class CreditsBubble : TimerProgress, IPointerEnterHandler, IPointerExitHandler

    {
        [SerializeField] private SVGImage background;
        [SerializeField] private ClickButton icon;
        [SerializeField] private CanvasGroup textContainer;

        private bool isAnimating;
        private float accumulatedTime;
        private float fadeTime = 0.5f;
        private float fadeProgress;
        private Color startingColor;
        private Color currentColor;
        private Color targetColor;

        private bool isPointerHovering;
        private bool lockInteraction;

        private enum FadeState
        {
            IDLE,
            FADING_OUT,
            FADING_IN
        }

        private FadeState state;

        public void Initialize()
        {
            Initialize(duration);
            Lock();
        }
        
        protected override void OnUpdate(float progress)
        {
            // Nothing
        }

        protected override void OnComplete()
        {
            if (!isPointerHovering)
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

                    currentColor = Color.Lerp(startingColor, targetColor, fadeProgress);

                    // Apply
                    background.color = currentColor;
                    textContainer.alpha = currentColor.a;

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
            state = FadeState.FADING_OUT;
            startingColor = background.color;
            targetColor = background.color;
            targetColor.a = 0;
            accumulatedTime = 0;
            isAnimating = true;
        }

        public void FadeIn()
        {
            state = FadeState.FADING_IN;
            startingColor = background.color;
            targetColor = background.color;
            targetColor.a = 1;
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
    }
}