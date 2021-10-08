using System;
using AdrianMiasik.Interfaces;
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

        public void Initialize(PomodoroTimer timer)
        {
            this.timer = timer;
            timer.GetTheme().RegisterColorHook(this);
        }
        
        public void OnClick()
        {
            onClick.Invoke();
            
            switch (timer.state)
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

        public void StateUpdate(PomodoroTimer.States state, Theme theme)
        {
            icon.transform.localScale = Vector3.one * 0.42f;
            icon.rectTransform.pivot = new Vector2(0.5f, icon.rectTransform.pivot.y);
            icon.rectTransform.anchoredPosition = Vector2.zero;
            
            switch (state)
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
                    if (timer.GetIsOnBreak())
                    {
                        icon.sprite = breakComplete;
                    }
                    else
                    {
                        icon.sprite = complete;
                    }
                    break;
            }
            
            ColorUpdate(theme);
        }

        public void ColorUpdate(Theme theme)
        {
            switch (timer.state)
            {
                case PomodoroTimer.States.SETUP:
                    icon.color = theme.GetCurrentColorScheme().running;
                    break;
                case PomodoroTimer.States.RUNNING:
                    icon.color = theme.GetCurrentColorScheme().modeOne;
                    break;
                case PomodoroTimer.States.PAUSED:
                    icon.color = theme.GetCurrentColorScheme().running;
                    break;
                case PomodoroTimer.States.COMPLETE:
                    icon.color = timer.GetIsOnBreak() ? theme.GetCurrentColorScheme().modeOne : theme.GetCurrentColorScheme().modeTwo;
                    break;
            }
        }
    }
}
