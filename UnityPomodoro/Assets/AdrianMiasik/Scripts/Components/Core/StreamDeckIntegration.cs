using System;
using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
using F10.StreamDeckIntegration;
using UnityEngine;
using F10.StreamDeckIntegration.Attributes;

namespace AdrianMiasik.Components.Core
{
    public class StreamDeckIntegration: MonoBehaviour, ITimerState
    {
        private PomodoroTimer timer;
        
        public void Initialize(PomodoroTimer pomodoroTimer)
        {
            timer = pomodoroTimer;
            
            StreamDeck.Add(this);
            
            UpdateTimerSwitchTitle();
            UpdateThemeTitle();
        }

        [StreamDeckButton]
        private void SwitchTimer()
        {
            timer.TriggerTimerSwitch();
            UpdateTimerSwitchTitle();
        }

        private void UpdateTimerSwitchTitle()
        {
            StreamDeckSettings.SetButtonTitle(timer.IsOnBreak() ? "Work" : "Break",
                "SwitchTimer");
        }

        [StreamDeckButton]
        private void RightButtonInteract()
        {
            switch (timer.m_state)
            {
                case PomodoroTimer.States.COMPLETE:
                    timer.TriggerTimerSwitch();
                    return;
                default:
                    timer.TriggerPlayPause();
                    break;
            }
        }

        [StreamDeckButton]
        private void Restart()
        {
            timer.TriggerTimerRestart();
        }

        [StreamDeckButton]
        private void Skip()
        {
            if (timer.m_state == PomodoroTimer.States.RUNNING || timer.m_state == PomodoroTimer.States.PAUSED)
            {
                timer.TriggerSkip();
            }
        }

        [StreamDeckButton]
        private void ToggleTheme()
        {
            timer.TriggerThemeSwitch();
            UpdateThemeTitle();
        }

        private void UpdateThemeTitle()
        {
            StreamDeckSettings.SetButtonTitle(timer.GetTheme().IsDarkMode() ? "Light" : "Dark", "ToggleTheme");
        }

        [StreamDeckButton]
        private void SelectDigit(int index)
        {
            timer.SelectDigit(index);
        }

        [StreamDeckButton]
        private void IncrementDigit(int index)
        {
            timer.IncrementDigit(index);
        }
        
        [StreamDeckButton]
        private void DecrementDigit(int index)
        {
            timer.DecrementDigit(index);
        }

        [StreamDeckButton]
        private void IncrementSelectedDigits()
        {
            timer.IncrementSelectedDigits();
        }
        
        [StreamDeckButton]
        private void DecrementSelectedDigits()
        {
            timer.DecrementSelectedDigits();
        }

        [StreamDeckButton]
        private void SelectAllDigits()
        {
            timer.SelectAll();
        }

        [StreamDeckButton]
        private void DeselectAllDigits()
        {
            timer.ClearSelection();
        }

        [StreamDeckButton]
        private void ToggleAllDigits()
        {
            timer.ToggleAllDigits();
        }

        [StreamDeckButton]
        private void ClearAll()
        {
            timer.ClearAll();
        }

        [StreamDeckButton]
        private void TrashTomatoes()
        {
            timer.TrashTomatoes();
        }

        [StreamDeckButton]
        private void Submit()
        {
            timer.GetConfirmDialogManager().TriggerSubmit();
        }
        
        [StreamDeckButton]
        private void Cancel()
        {
            timer.GetConfirmDialogManager().TriggerCancel();
        }

        /// <summary>
        /// Updates all our Stream buttons accordingly
        /// </summary>
        /// <param name="state"></param>
        /// <param name="theme"></param>
        public void StateUpdate(PomodoroTimer.States state, Theme theme)
        {
            UpdateTimerSwitchTitle();
            
            switch (timer.m_state)
            {
                case PomodoroTimer.States.SETUP:
                    StreamDeckSettings.SetButtonTitle("-", "Skip");
                    StreamDeckSettings.SetButtonTitle("Play", "RightButtonInteract");
                    break;
                case PomodoroTimer.States.RUNNING:
                    StreamDeckSettings.SetButtonTitle("Skip", "Skip");
                    StreamDeckSettings.SetButtonTitle("Pause", "RightButtonInteract");
                    break;
                case PomodoroTimer.States.PAUSED:
                    StreamDeckSettings.SetButtonTitle("Skip", "Skip");
                    StreamDeckSettings.SetButtonTitle("Play", "RightButtonInteract");
                    break;
                case PomodoroTimer.States.COMPLETE:
                    StreamDeckSettings.SetButtonTitle("-", "Skip");
                    StreamDeckSettings.SetButtonTitle(timer.IsOnBreak() ? "Work" : "Break",
                        "RightButtonInteract");
                    break;
            }
        }

        private void OnDestroy()
        {
            StreamDeck.Remove(this);
        }
    }
}