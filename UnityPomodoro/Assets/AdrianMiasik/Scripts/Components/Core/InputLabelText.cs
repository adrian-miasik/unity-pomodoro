using System;
using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
using TMPro;
using UnityEngine;

namespace AdrianMiasik.Components.Core
{
    public class InputLabelText : MonoBehaviour, ITimerState
    {
        [SerializeField] private TMP_InputField m_inputText;

        private bool isOverridingDefaultWorkText = false;
        private bool isOverridingDefaultBreakText = false;
        private bool isOverridingDefaultLongBreakText = false;

        private string workText;
        private string breakText;
        private string longBreakText;

        private PomodoroTimer timer;

        public void Initialize(PomodoroTimer pomodoroTimer)
        {
            timer = pomodoroTimer;

            // Use default text
            SetText(string.Empty);

            // Register event
            m_inputText.onSubmit.AddListener(SetText);
        }

        private void SetText(string text)
        {
            if (text == String.Empty)
            {
                SetUserStateText(string.Empty, false);

                // Use defaults
                StateUpdate(timer.m_state, timer.GetTheme());
            }
            else
            {
                SetUserStateText(text, true);
            }
        }

        private void SetUserStateText(string desiredText, bool isOverridingDefaultText)
        {
            if (!timer.IsOnBreak())
            {
                isOverridingDefaultWorkText = isOverridingDefaultText;
                workText = desiredText;
            }
            else
            {
                if (!timer.IsOnLongBreak())
                {
                    isOverridingDefaultBreakText = isOverridingDefaultText;
                    breakText = desiredText;
                }
                else
                {
                    isOverridingDefaultLongBreakText = isOverridingDefaultText;
                    longBreakText = desiredText;
                }
            }
        }

        private string GetUserStateText()
        {
            if (!timer.IsOnBreak())
            {
                return workText;
            }

            return !timer.IsOnLongBreak() ? breakText : longBreakText;
        }

        public void SetSuffix(string suffix)
        {
            if (string.IsNullOrEmpty(GetUserStateText()))
            {
                // Only show suffix
                m_inputText.text = suffix;
            }
            else
            {
                // Show user text + suffix
                m_inputText.text = GetUserStateText() + ": " + suffix;
            }
        }

        public void ClearSuffix()
        {
            // If we are using custom labels...
            if (IsOverridingDefaultStateText())
            {
                // Apply basic custom label w/ no preset.
                m_inputText.text = GetUserStateText();
            }
        }

        private bool IsOverridingDefaultStateText()
        {
            if (!timer.IsOnBreak())
            {
                return isOverridingDefaultWorkText;
            }

            return !timer.IsOnLongBreak() ? isOverridingDefaultBreakText : isOverridingDefaultLongBreakText;
        }

        public void SetTextColor(Color desiredColor)
        {
            m_inputText.textComponent.color = desiredColor;
        }

        public void StateUpdate(PomodoroTimer.States state, Theme theme)
        {
            // If we are using default labels...
            if (IsOverridingDefaultStateText())
            {
                // Early exit!
                return;
            }

            // Use pre-defined labels
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