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
        }
        
        [StreamDeckButton]
        private void PlayPauseToggle()
        {
            switch (timer.m_state)
            {
                case PomodoroTimer.States.COMPLETE:
                    return;
                case PomodoroTimer.States.RUNNING:
                    timer.Pause();
                    break;
                default:
                    timer.Play();
                    break;
            }
        }

        [StreamDeckButton]
        private void Skip()
        {
            if (timer.m_state == PomodoroTimer.States.RUNNING || timer.m_state == PomodoroTimer.States.PAUSED)
            {
                timer.Skip();
            }
        }

        /// <summary>
        /// Updates all our Stream buttons accordingly
        /// </summary>
        /// <param name="state"></param>
        /// <param name="theme"></param>
        public void StateUpdate(PomodoroTimer.States state, Theme theme)
        {
            Debug.Log("state update!");
            
            switch (timer.m_state)
            {
                case PomodoroTimer.States.SETUP:
                    StreamDeckSettings.SetButtonTitle("-", "Skip");
                    StreamDeckSettings.SetButtonTitle("Play", "PlayPauseToggle");
                    break;
                case PomodoroTimer.States.RUNNING:
                    StreamDeckSettings.SetButtonTitle("Skip", "Skip");
                    StreamDeckSettings.SetButtonTitle("Pause", "PlayPauseToggle");
                    break;
                case PomodoroTimer.States.PAUSED:
                    StreamDeckSettings.SetButtonTitle("Skip", "Skip");
                    StreamDeckSettings.SetButtonTitle("Play", "PlayPauseToggle");
                    break;
                case PomodoroTimer.States.COMPLETE:
                    StreamDeckSettings.SetButtonTitle("-", "Skip");
                    StreamDeckSettings.SetButtonTitle("-", "PlayPauseToggle");
                    break;
            }
        }

        private void OnDestroy()
        {
            StreamDeck.Remove(this);
        }
    }
}