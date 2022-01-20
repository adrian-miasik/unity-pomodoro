using System;
using System.Collections.Generic;
using AdrianMiasik.Components.Base;
using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AdrianMiasik.Components.Specific
{
    // TODO: Create Bubble base class? 
    /// <summary>
    /// Used to display the end local time for the current running timer.
    /// (E.g. It's 3:02pm with 3 minutes left on the timer. Thus this will display: "3:05pm".)
    /// </summary>
    public class EndTimestampBubble : ThemeElement, IPointerEnterHandler, IPointerExitHandler, ITimerState
    { 
        [SerializeField] private SVGImage m_background;
        [SerializeField] private CanvasGroup m_backgroundContainer;
        [SerializeField] private SVGImage m_icon;
        [SerializeField] private CanvasGroup m_textContainer;
        [Tooltip("The last text in this list should be the end timer text")]
        [SerializeField] private List<TMP_Text> m_text = new List<TMP_Text>();
        [Tooltip("E.g. 0.5f = fade time of 2 seconds, 2 = fade time of 0.5 seconds.")] [SerializeField]
        private float m_fadeSpeed = 2f;

        [SerializeField] private Sprite workMode;
        [SerializeField] private Sprite breakMode;

        private float fadeProgress = 0;

        private bool isPointerHovering;
        private bool lockInteraction;

        private enum FadeState
        {
            IDLE,
            FADING_OUT,
            FADING_IN
        }

        private FadeState state;

        public override void Initialize(PomodoroTimer pomodoroTimer, bool updateColors = true)
        {
            base.Initialize(pomodoroTimer, updateColors);

            //Lock();
        }

        private void Update()
        {
            if (!IsInitialized())
            {
                return;
            }

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

            // FadeIn();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isPointerHovering = false;

            if (lockInteraction)
            {
                return;
            }

            // FadeOut();
        }

        public void FadeOut()
        {
            foreach (TMP_Text text in m_text)
            {
                text.color = Timer.GetTheme().GetCurrentColorScheme().m_foreground;
            }
            state = FadeState.FADING_OUT;
        }

        public void FadeIn()
        {
            // Set sprite depending what the user will get prompted to do next
            if (Timer.IsOnBreak() || Timer.IsOnLongBreak())
            {
                m_icon.sprite = workMode;
            }
            else
            {
                m_icon.sprite = breakMode;
            }
            
            foreach (TMP_Text text in m_text)
            {
                text.color = Timer.GetTheme().GetCurrentColorScheme().m_foreground;
            }
            state = FadeState.FADING_IN;
        }

        public override void ColorUpdate(Theme theme)
        {
            m_background.color = theme.GetCurrentColorScheme().m_backgroundHighlight;
            
            foreach (TMP_Text text in m_text)
            {
                text.color = theme.GetCurrentColorScheme().m_foreground;
            }
            
            m_icon.color = theme.GetCurrentColorScheme().m_foreground;
        }

        public void StateUpdate(PomodoroTimer.States state, Theme theme)
        {
            if (state == PomodoroTimer.States.RUNNING && Timer.IsMainContentOpen())
            {
                FadeIn();
                CalculateEndTime();
            }
            else
            {
                FadeOut();
            }
        }

        private void CalculateEndTime()
        {
            // Get current current remaining time
            TimeSpan currentTimeSpan = TimeSpan.FromSeconds(Timer.GetCurrentTime());

            // Get system time span
            TimeSpan systemTimeSpan = DateTime.Now.TimeOfDay;
            
            // Add time spans together
            TimeSpan endTime = systemTimeSpan.Add(currentTimeSpan);
            
            // Display end time
            m_text[m_text.Count - 1].text = new DateTime(endTime.Ticks).ToLongTimeString();
        }

        public void EnableFeature(PomodoroTimer timer)
        {
            gameObject.SetActive(true);
        }

        public void DisableFeature()
        {
            throw new NotImplementedException();
        }
    }
}
