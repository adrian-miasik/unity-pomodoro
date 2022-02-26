using System.Collections.Generic;
using AdrianMiasik.Components.Core;
using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;

namespace AdrianMiasik.Components.Base
{
    /// <summary>
    /// A **base** class that is intended to be used on any component/class that needs fade in and out
    /// <see cref="ThemeIcon"/>'s and text.
    /// </summary>
    public class Ghost : ThemeElement, ITimerState
    {
        [SerializeField] private SVGImage m_background;
        [SerializeField] private CanvasGroup m_backgroundContainer;
        [SerializeField] protected ThemeIcon m_icon;
        [SerializeField] private CanvasGroup m_textContainer;
        [SerializeField] protected List<TMP_Text> m_text = new List<TMP_Text>();
        [SerializeField] private float m_fadeProgress = 1;
        [Tooltip("E.g. 0.5f = fade time of 2 seconds, 2 = fade time of 0.5 seconds.")]
        [SerializeField] private float m_fadeSpeed = 2f;

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
            
            m_icon.Initialize(pomodoroTimer);
        }

        public void StateUpdate(PomodoroTimer.States state, Theme theme)
        {
            if (state == PomodoroTimer.States.RUNNING && Timer.IsMainContentOpen())
            {
                FadeIn();
            }
            else
            {
                FadeOut();
            }
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
                    m_fadeProgress += Time.deltaTime * m_fadeSpeed;
                }
                else if (state == FadeState.FADING_OUT)
                {
                    m_fadeProgress -= Time.deltaTime * m_fadeSpeed;
                }

                m_fadeProgress = Mathf.Clamp01(m_fadeProgress);
                m_textContainer.alpha = m_fadeProgress;
                m_backgroundContainer.alpha = m_fadeProgress;

                // If fade is completed...
                if (m_fadeProgress <= 0 || m_fadeProgress >= 1)
                {
                    state = FadeState.IDLE;
                }
            }
        }

        public virtual void FadeIn(bool instantly = false)
        {
            foreach (TMP_Text text in m_text)
            {
                text.color = Timer.GetTheme().GetCurrentColorScheme().m_foreground;
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
        
        public virtual void FadeOut(bool instantly = false)
        {
            foreach (TMP_Text text in m_text)
            {
                text.color = Timer.GetTheme().GetCurrentColorScheme().m_foreground;
            }
            
            if (instantly)
            {
                state = FadeState.IDLE;
                m_textContainer.alpha = 0;
                m_backgroundContainer.alpha = 0;
            }
            else
            {
                state = FadeState.FADING_OUT;
            }
        }
        
        public override void ColorUpdate(Theme theme)
        {
            m_background.color = theme.GetCurrentColorScheme().m_backgroundHighlight;

            foreach (TMP_Text text in m_text)
            {
                text.color = theme.GetCurrentColorScheme().m_foreground;
            }
        }
    }
}