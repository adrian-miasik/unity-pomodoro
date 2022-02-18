using AdrianMiasik.Components.Base;
using AdrianMiasik.Components.Core.Containers;
using AdrianMiasik.ScriptableObjects;
using TMPro;
using UnityEngine;

namespace AdrianMiasik.Components.Specific.Settings
{
    /// <summary>
    /// A <see cref="ThemeElement"/> page used to display a set of interactable user options. Currently supports
    /// switching between different digit formats and muting our application when it's not in focus.
    /// See: <see cref="DigitFormat.SupportedFormats"/>.
    /// </summary>
    public class SettingsPanel : ThemeElement
    {
        [SerializeField] private TMP_Text m_title;
        
        // Dropdowns
        [SerializeField] private SetDigitFormat m_setDigitFormat;
        [SerializeField] private SetPomodoroCount m_pomodoro;
        
        // Toggle Sliders
        [SerializeField] private SetLongBreaks m_setLongBreakSettingsOption;
        [SerializeField] private SetMuteAudioOutOfFocus m_setMuteSoundOutOfFocusToggle;
        [SerializeField] private UnityAnalyticsSettingsOption m_unityAnalyticsSettingsOption;
        
        private bool isOpen;

        public void Initialize(PomodoroTimer pomodoroTimer, ScriptableObjects.Settings settingsConfig)
        {
            base.Initialize(pomodoroTimer, false);
            
            // Overrides
            m_setMuteSoundOutOfFocusToggle.OverrideFalseColor(Timer.GetTheme().GetCurrentColorScheme().m_backgroundHighlight);
            m_setMuteSoundOutOfFocusToggle.OverrideTrueColor(Timer.GetTheme().GetCurrentColorScheme().m_modeOne);
            m_setLongBreakSettingsOption.OverrideFalseColor(Timer.GetTheme().GetCurrentColorScheme().m_backgroundHighlight);
            m_setLongBreakSettingsOption.OverrideTrueColor(Timer.GetTheme().GetCurrentColorScheme().m_modeOne);
            m_unityAnalyticsSettingsOption.OverrideFalseColor(Timer.GetTheme().GetCurrentColorScheme().m_backgroundHighlight);
            m_unityAnalyticsSettingsOption.OverrideTrueColor(Timer.GetTheme().GetCurrentColorScheme().m_modeOne);
            
            // Init
            m_setDigitFormat.Initialize(Timer);
            m_pomodoro.Initialize(Timer);
            m_setLongBreakSettingsOption.Initialize(Timer, settingsConfig);
            m_unityAnalyticsSettingsOption.Initialize(Timer, settingsConfig);
            // Hide mute option based on platform (Shown by default)
#if UNITY_ANDROID
            HideMuteSoundOutOfFocusOption();
#elif UNITY_IOS
            HideMuteSoundOutOfFocusOption();
#else
            m_setMuteSoundOutOfFocusToggle.Initialize(Timer, settingsConfig);
#endif
        }
        
        /// <summary>
        /// Applies our <see cref="Theme"/> changes to our referenced components when necessary.
        /// </summary>
        /// <param name="theme">The theme to apply on our referenced components.</param>
        public override void ColorUpdate(Theme theme)
        {
            if (isOpen)
            {
                m_title.color = theme.GetCurrentColorScheme().m_foreground;
                
                m_setDigitFormat.ColorUpdate(Timer.GetTheme());
                
                m_setMuteSoundOutOfFocusToggle.ColorUpdate(theme);
                
                // m_unityAnalyticsSettingsOption.ColorUpdate(theme);
            }
        }

        /// <summary>
        /// Updates the switch digit layout dropdown to use the current timer's digit format.
        /// </summary>
        public void UpdateDropdown()
        {
            m_setDigitFormat.SetDropdownValue(Timer.GetDigitFormatIndex());
        }

        /// <summary>
        /// Sets the switch digit layout dropdown to the provided digit format index.
        /// (See: <see cref="DigitFormat.SupportedFormats"/>)
        /// </summary>
        /// <param name="value">The index value you want the dropdown to be set at.</param>
        public void SetDropdown(int value)
        {
            m_setDigitFormat.SetDropdownValue(value);
        }
        
        /// <summary>
        /// Displays this panel to the user.
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
            m_setDigitFormat.SetDropdownValue(Timer.GetDigitFormatIndex());
            isOpen = true;
            ColorUpdate(Timer.GetTheme());
        }

        /// <summary>
        /// Hides this panel away from the user.
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
            isOpen = false;
        }

        /// <summary>
        /// Is this <see cref="SettingsPanel"/> currently open and visible?
        /// </summary>
        /// <returns></returns>
        public bool IsPageOpen()
        {
            return isOpen;
        }
        
        /// <summary>
        /// Shows the 'sound mute when application is out of focus' option to the user.
        /// <remarks>Intended to be shown for desktop users, not mobile.</remarks>
        /// </summary>
        public void ShowMuteSoundOutOfFocusOption()
        {
            m_setMuteSoundOutOfFocusToggle.gameObject.SetActive(true);
        }
        
        /// <summary>
        /// Hides the 'sound mute when application is out of focus' option from the user.
        /// <remarks>Intended to be hidden for mobile users, not desktop.</remarks>
        /// </summary>
        public void HideMuteSoundOutOfFocusOption()
        {
            m_setMuteSoundOutOfFocusToggle.gameObject.SetActive(false);
        }
    }
}