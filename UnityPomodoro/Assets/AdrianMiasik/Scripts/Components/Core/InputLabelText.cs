using System;
using AdrianMiasik.Components.Base;
using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
using TMPro;
using UnityEngine;

namespace AdrianMiasik.Components.Core
{
    public class InputLabelText : ThemeElement, ITimerState
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

        public override void Initialize(PomodoroTimer pomodoroTimer, bool updateColors = true)
        {
            base.Initialize(pomodoroTimer, updateColors);

            // Register deselect event -> Submit text on deselection
            m_inputText.onDeselect.AddListener(OnDeselect);
            
            // Use default text
            SetUserText(string.Empty);

            // Register event
            m_inputText.onSubmit.AddListener(SetUserText);

            // Set default themes
            SetTextColor(Timer.GetTheme().GetCurrentColorScheme().m_backgroundHighlight);
            m_inputText.placeholder.color = Timer.GetTheme().GetCurrentColorScheme().m_backgroundHighlight;
        }

        private void OnDeselect(string inputFieldString)
        {
            // Submit on deselection
            m_inputText.onSubmit.Invoke(m_inputText.text);

            // Debug.Log("The input label text has been deselected: " + inputFieldString);
        }

        /// <summary>
        /// Sets the label based on the current timer state.
        /// </summary>
        /// <param name="text"></param>
        private void SetUserText(string text)
        {
            if (text == String.Empty ||
                text == "Set a work time" ||
                text == "Set a break time" ||
                text == "Set a long break time")
            {
                SetUserStateText(string.Empty, false);
            }
            else
            {
                SetUserStateText(text, true);
                SetTextColor(Timer.GetTheme().GetCurrentColorScheme().m_foreground);
            }

            // Apply state context
            StateUpdate(Timer.m_state, Timer.GetTheme());
        }

        /// <summary>
        /// Set's the internal string cache based on the current timer state/context.
        /// </summary>
        /// <param name="desiredText"></param>
        /// <param name="isOverridingDefaultText"></param>
        private void SetUserStateText(string desiredText, bool isOverridingDefaultText)
        {
            if (!Timer.IsOnBreak())
            {
                isOverridingDefaultWorkText = isOverridingDefaultText;
                workText = desiredText;
            }
            else
            {
                if (!Timer.IsOnLongBreak())
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
            if (!Timer.IsOnBreak())
            {
                return workText;
            }

            return !Timer.IsOnLongBreak() ? breakText : longBreakText;
        }

        /// <summary>
        /// Adds a suffix to the input label text (Only if the text is custom and not default)
        /// </summary>
        /// <param name="suffix"></param>
        private void SetSuffix(string suffix)
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
        /// Is the current timer state using custom user text?
        /// </summary>
        /// <returns></returns>
        private bool IsOverridingDefaultStateText()
        {
            if (!Timer.IsOnBreak())
            {
                return isOverridingDefaultWorkText;
            }

            return !Timer.IsOnLongBreak() ? isOverridingDefaultBreakText : isOverridingDefaultLongBreakText;
        }

        /// <summary>
        /// Sets the font color to the provided desired color.
        /// </summary>
        /// <param name="desiredColor"></param>
        public void SetTextColor(Color desiredColor)
        {
            m_inputText.textComponent.color = desiredColor;
        }

        /// <summary>
        /// Interface: Invoked everytime the timer changes state + when submitting text
        /// </summary>
        public void StateUpdate(PomodoroTimer.States state, Theme theme)
        {
            switch (state)
            {
                case PomodoroTimer.States.SETUP:
                    // If we are using defaults...Use pre-defined text
                    if (!IsOverridingDefaultStateText())
                    {
                        if (!Timer.IsOnBreak())
                        {
                            m_inputText.text = "Set a work time";
                        }
                        else
                        {
                            m_inputText.text = !Timer.IsOnLongBreak() ? "Set a break time" : "Set a long break time";
                        }

                        // Use subtle text coloring
                        SetTextColor(theme.GetCurrentColorScheme().m_backgroundHighlight);
                    }
                    // Otherwise, using custom user text
                    else
                    {
                        // Use user custom text
                        m_inputText.text = GetUserStateText();

                        // Set color to foreground indicating custom label in-use.
                        SetTextColor(theme.GetCurrentColorScheme().m_foreground);
                    }
                    m_inputText.interactable = true;
                    break;

                case PomodoroTimer.States.RUNNING:
                    SetSuffix("Running");
                    m_inputText.interactable = false;
                    break;

                case PomodoroTimer.States.PAUSED:
                    SetSuffix("Paused");
                    m_inputText.interactable = false;
                    break;

                case PomodoroTimer.States.COMPLETE:
                    break;

                default:
                    Debug.LogWarning("This timer state is not currently supported.");
                    break;
            }
        }

        public override void ColorUpdate(Theme theme)
        {
            if (IsOverridingDefaultStateText())
            {
                // Set color to foreground indicating custom label in-use.
                SetTextColor(theme.GetCurrentColorScheme().m_foreground);
            }
            else
            {
                // Use subtle text coloring
                SetTextColor(theme.GetCurrentColorScheme().m_backgroundHighlight);
            }

            m_inputText.placeholder.color = Timer.GetTheme().GetCurrentColorScheme().m_backgroundHighlight;
        }
    }
}