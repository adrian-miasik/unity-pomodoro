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
        [SerializeField] private SVGImage icon;

        [Header("SVGs")]
        [SerializeField] private Sprite setup; 
        [SerializeField] private Sprite running;
        [SerializeField] private Sprite complete;
        [SerializeField] private Sprite breakComplete;
        
        // Unity Events 
        public UnityEvent onClick;
        public UnityEvent playOnClick;
        public UnityEvent pauseOnClick;
        public UnityEvent snoozeOnClick;

        // Cache
        private PomodoroTimer timer;

        public void Initialize(PomodoroTimer _timer)
        {
            timer = _timer;
            _timer.GetTheme().RegisterColorHook(this);
        }
        
        public void OnClick()
        {
            onClick.Invoke();
            
            switch (timer.m_state)
            {
                case PomodoroTimer.States.SETUP:
                    playOnClick.Invoke();
                    break;
                case PomodoroTimer.States.RUNNING:
                    pauseOnClick.Invoke();
                    break;
                case PomodoroTimer.States.PAUSED:
                    playOnClick.Invoke();
                    break;
                case PomodoroTimer.States.COMPLETE:
                    snoozeOnClick.Invoke();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void StateUpdate(PomodoroTimer.States _state, Theme _theme)
        {
            icon.transform.localScale = Vector3.one * 0.42f;
            icon.rectTransform.pivot = new Vector2(0.5f, icon.rectTransform.pivot.y);
            icon.rectTransform.anchoredPosition = Vector2.zero;
            
            switch (_state)
            {
                case PomodoroTimer.States.SETUP:
                    icon.sprite = setup;
                    icon.rectTransform.pivot = new Vector2(0.6f, icon.rectTransform.pivot.y);
                    icon.rectTransform.anchoredPosition = Vector2.zero;
                    break;
                
                case PomodoroTimer.States.RUNNING:
                    icon.sprite = running;
                    break;
                
                case PomodoroTimer.States.PAUSED:
                    icon.sprite = setup;
                    icon.rectTransform.pivot = new Vector2(0.6f, icon.rectTransform.pivot.y);
                    icon.rectTransform.anchoredPosition = Vector2.zero;
                    break;
                
                case PomodoroTimer.States.COMPLETE:
                    icon.transform.localScale = Vector3.one * 0.55f;                    
                    icon.sprite = timer.IsOnBreak() ? breakComplete : complete;
                    break;
            }
            
            ColorUpdate(_theme);
        }

        public void ColorUpdate(Theme _theme)
        {
            switch (timer.m_state)
            {
                case PomodoroTimer.States.SETUP:
                    icon.color = _theme.GetCurrentColorScheme().running;
                    break;
                case PomodoroTimer.States.RUNNING:
                    icon.color = _theme.GetCurrentColorScheme().modeOne;
                    break;
                case PomodoroTimer.States.PAUSED:
                    icon.color = _theme.GetCurrentColorScheme().running;
                    break;
                case PomodoroTimer.States.COMPLETE:
                    icon.color = timer.IsOnBreak() ? _theme.GetCurrentColorScheme().modeOne : _theme.GetCurrentColorScheme().modeTwo;
                    break;
            }
        }
    }
}