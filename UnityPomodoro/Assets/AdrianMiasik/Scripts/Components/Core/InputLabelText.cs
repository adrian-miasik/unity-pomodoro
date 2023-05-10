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

        // A bool tracking if a timer state label is overridden (Use custom user text/default fallback?)
        private bool isOverridingDefaultWorkText = false;
        private bool isOverridingDefaultBreakText = false;
        private bool isOverridingDefaultLongBreakText = false;

        // Custom user text for each timer state/context
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

        /// <summary>
        /// Set's the internal string cache based on the current timer state/context.
        /// </summary>
        /// <param name="desiredText"></param>
        /// <param name="isOverridingDefaultText"></param>
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

        /// <summary>
        /// Return's the custom user text for the current timer state/context.
        /// </summary>
        /// <returns></returns>
        private string GetUserStateText()
        {
            if (!timer.IsOnBreak())
            {
                return workText;
            }

            return !timer.IsOnLongBreak() ? breakText : longBreakText;
        }

        public void SetSuffix(string suffix)
        /// <summary>
        /// Adds a suffix to the input label text (Only if the text is custom and not default)
        /// </summary>
        /// <param name="suffix"></param>
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

        /// <summary>
        /// Removes the suffix from the input label text. (Suffix only seen if text is custom)
        /// </summary>
        public void ClearSuffix()
        {
            // If we are using custom labels...
            if (IsOverridingDefaultStateText())
            {
                // Apply basic custom label w/ no preset.
                m_inputText.text = GetUserStateText();
            }
        }

        /// <summary>
        /// Is the current timer state using custom user text?
        /// </summary>
        /// <returns></returns>
        private bool IsOverridingDefaultStateText()
        {
            if (!timer.IsOnBreak())
            {
                return isOverridingDefaultWorkText;
            }

            return !timer.IsOnLongBreak() ? isOverridingDefaultBreakText : isOverridingDefaultLongBreakText;
        }

        /// <summary>
        /// Sets the font color to the provided desired color.
        /// </summary>
        /// <param name="desiredColor"></param>
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