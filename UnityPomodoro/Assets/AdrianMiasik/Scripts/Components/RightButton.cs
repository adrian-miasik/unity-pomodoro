using System;
using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.Events;

namespace AdrianMiasik.Components
{
    public class RightButton : MonoBehaviour, ITimerState, IColorHook
    {
        [Header("References")] 
        [SerializeField] private SVGImage m_icon;
        
        [Header("SVGs")]
        [SerializeField] private Sprite m_setup; 
        [SerializeField] private Sprite m_running;
        [SerializeField] private Sprite m_complete;
        [SerializeField] private Sprite m_breakComplete;
        
        // Unity Events 
        public UnityEvent m_onClick;
        public UnityEvent m_playOnClick;
        public UnityEvent m_pauseOnClick;
        public UnityEvent m_snoozeOnClick;

        // Cache
        private PomodoroTimer timer;

        public void Initialize(PomodoroTimer pomodoroTimer)
        {
            timer = pomodoroTimer;
            pomodoroTimer.GetTheme().RegisterColorHook(this);
        }
        
        public void OnClick()
        {
            m_onClick.Invoke();
            
            switch (timer.m_state)
            {
                case PomodoroTimer.States.SETUP:
                    m_playOnClick.Invoke();
                    break;
                case PomodoroTimer.States.RUNNING:
                    m_pauseOnClick.Invoke();
                    break;
                case PomodoroTimer.States.PAUSED:
                    m_playOnClick.Invoke();
                    break;
                case PomodoroTimer.States.COMPLETE:
                    m_snoozeOnClick.Invoke();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void StateUpdate(PomodoroTimer.States state, Theme theme)
        {
            m_icon.transform.localScale = Vector3.one * 0.42f;
            m_icon.rectTransform.pivot = new Vector2(0.5f, m_icon.rectTransform.pivot.y);
            m_icon.rectTransform.anchoredPosition = Vector2.zero;
            
            switch (state)
            {
                case PomodoroTimer.States.SETUP:
                    m_icon.sprite = m_setup;
                    m_icon.rectTransform.pivot = new Vector2(0.6f, m_icon.rectTransform.pivot.y);
                    m_icon.rectTransform.anchoredPosition = Vector2.zero;
                    break;
                
                case PomodoroTimer.States.RUNNING:
                    m_icon.sprite = m_running;
                    break;
                
                case PomodoroTimer.States.PAUSED:
                    m_icon.sprite = m_setup;
                    m_icon.rectTransform.pivot = new Vector2(0.6f, m_icon.rectTransform.pivot.y);
                    m_icon.rectTransform.anchoredPosition = Vector2.zero;
                    break;
                
                case PomodoroTimer.States.COMPLETE:
                    m_icon.transform.localScale = Vector3.one * 0.55f;                    
                    m_icon.sprite = timer.IsOnBreak() ? m_breakComplete : m_complete;
                    break;
            }
            
            ColorUpdate(theme);
        }

        public void ColorUpdate(Theme theme)
        {
            switch (timer.m_state)
            {
                case PomodoroTimer.States.SETUP:
                    m_icon.color = theme.GetCurrentColorScheme().m_running;
                    break;
                case PomodoroTimer.States.RUNNING:
                    m_icon.color = theme.GetCurrentColorScheme().m_modeOne;
                    break;
                case PomodoroTimer.States.PAUSED:
                    m_icon.color = theme.GetCurrentColorScheme().m_running;
                    break;
                case PomodoroTimer.States.COMPLETE:
                    m_icon.color = timer.IsOnBreak() ? theme.GetCurrentColorScheme().m_modeOne : theme.GetCurrentColorScheme().m_modeTwo;
                    break;
            }
        }
    }
}
