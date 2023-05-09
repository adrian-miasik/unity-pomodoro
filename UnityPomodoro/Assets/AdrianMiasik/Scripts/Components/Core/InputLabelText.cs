using System;
using System.Collections;
using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AdrianMiasik.Components.Core
{
    public class InputLabelText : MonoBehaviour, ITimerState
    {
        [SerializeField] private TMP_InputField m_inputText;

        private bool isOverridingDefaultText = false;
        private string userText;

        private PomodoroTimer timer;

        public void Initialize(PomodoroTimer pomodoroTimer)
        {
            timer = pomodoroTimer;

            // Use default text
            SetText(string.Empty);

            // Register event
            m_inputText.onSubmit.AddListener(SetText);
        }

        [ContextMenu("Refresh State Text")]
        public void RefreshStateText()
        {
            // Force update default text
            StateUpdate(timer.m_state, timer.GetTheme());
        }

        private void SetText(string text)
        {
            if (text == String.Empty)
            {
                isOverridingDefaultText = false;
                userText = String.Empty;

                // Use defaults
                RefreshStateText();
            }
            else
            {
                isOverridingDefaultText = true;
                userText = text;
            }
        }

        public void SetSuffix(string suffix)
        {
            if (string.IsNullOrEmpty(userText))
            {
                // Only show suffix
                m_inputText.text = suffix;
            }
            else
            {
                // Show user text + suffix
                m_inputText.text = userText + ": " + suffix;
            }
        }

        public void SetTextColor(Color desiredColor)
        {
            m_inputText.textComponent.color = desiredColor;
        }

        public void ClearSuffix()
        {
            if (isOverridingDefaultText)
            {
                m_inputText.text = userText;
            }
        }

        public void StateUpdate(PomodoroTimer.States state, Theme theme)
        {
            if (isOverridingDefaultText)
            {
                return;
            }

            switch (state)
            {
                case PomodoroTimer.States.SETUP:
                    if (!timer.IsOnBreak())
                    {
                        m_inputText.text = "Set a work time";
                    }
                    else
                    {
                        m_inputText.text = !timer.IsOnLongBreak() ? "Set a break time" : "Set a long break time";
                    }
                    break;
                case PomodoroTimer.States.RUNNING:
                    break;
                case PomodoroTimer.States.PAUSED:
                    break;
                case PomodoroTimer.States.COMPLETE:
                    break;
                default:
                    Debug.LogWarning("This timer state is not currently supported.");
                    break;
            }
        }
    }
}