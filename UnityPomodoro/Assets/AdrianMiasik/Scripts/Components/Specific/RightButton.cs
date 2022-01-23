using System;
using AdrianMiasik.Components.Base;
using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.Events;

namespace AdrianMiasik.Components.Specific
{
    /// <summary>
    /// A <see cref="ThemeElement"/> button used to play/pause the timer. Implements <see cref="ITimerState"/>
    /// to change graphics depending on the <see cref="PomodoroTimer"/>'s current state.
    /// (See <see cref="PomodoroTimer.States"/>)
    /// <remarks>Explicitly not a <see cref="ClickButton"/> since this component relies on the pomodoro timer state.
    /// But intended to be used in conjunction with a <see cref="ClickButton"/> core component.</remarks>
    /// </summary>
    public class RightButton : ThemeElement, ITimerState
    {
        [Header("References")] 
        [SerializeField] private SVGImage m_icon;
        
        [Header("SVGs")]
        [SerializeField] private Sprite m_setup; 
        [SerializeField] private Sprite m_running;
        [SerializeField] private Sprite m_complete;
        [SerializeField] private Sprite m_breakComplete;
        
        /// <summary>
        /// Invoked when this button is clicked.
        /// </summary>
        public UnityEvent m_onClick;
        
        /// <summary>
        /// Invoked when the user is presses the play button / is resuming from a paused state.
        /// </summary>
        public UnityEvent m_playOnClick;
        
        /// <summary>
        /// Invoked when the user presses the pause button.
        /// </summary>
        public UnityEvent m_pauseOnClick;
        
        /// <summary>
        /// Invoked when the user presses the switch timer button. (work / break)
        /// </summary>
        public UnityEvent m_snoozeOnClick;

        /// <summary>
        /// Sets up this component. (ThemeElement register)
        /// </summary>
        /// <param name="pomodoroTimer"></param>
        public void Initialize(PomodoroTimer pomodoroTimer)
        {
            base.Initialize(pomodoroTimer, false);
        }
        
        /// <summary>
        /// Invoked when the user presses this right button.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void OnClick()
        {
            m_onClick.Invoke();
            
            switch (Timer.m_state)
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

        /// <summary>
        /// Positions the sprites to a specific offset and scale. (Asset specific!)
        /// </summary>
        /// <param name="state"></param>
        /// <param name="theme"></param>
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
                    m_icon.sprite = Timer.IsOnBreak() ? m_breakComplete : m_complete;
                    break;
            }
            
            ColorUpdate(theme);
        }

        /// <summary>
        /// Applies our <see cref="Theme"/> changes to our referenced components when necessary.
        /// </summary>
        /// <param name="theme">The theme to apply on our referenced components.</param>
        public override void ColorUpdate(Theme theme)
        {
            switch (Timer.m_state)
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
                    m_icon.color = Timer.IsOnBreak() ? theme.GetCurrentColorScheme().m_modeOne : theme.GetCurrentColorScheme().m_modeTwo;
                    break;
            }
        }
    }
}