using AdrianMiasik.Components.Base;
using AdrianMiasik.Components.Core;
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
        
        [Header("Dropdowns")]
        [SerializeField] private OptionDigitFormat m_optionDigitFormat;
        [SerializeField] private OptionPomodoroCount m_optionPomodoroCount;
        
        [Header("Toggle Sliders")]
        [SerializeField] private OptionEnableLongBreaks m_optionEnableLongBreak;
        [SerializeField] private OptionMuteAudioOutOfFocus m_optionMuteSoundOutOfFocusToggle;
        [SerializeField] private OptionUnityAnalytics m_optionUnityAnalytics;
        
        private bool isOpen;

        public void Initialize(PomodoroTimer pomodoroTimer, SystemSettings systemSettings)
        {
            base.Initialize(pomodoroTimer, false);
            
            // Overrides
            // m_optionEnableLongBreak.OverrideFalseColor(Timer.GetTheme().GetCurrentColorScheme().m_backgroundHighlight);
            // m_optionEnableLongBreak.OverrideTrueColor(Timer.GetTheme().GetCurrentColorScheme().m_modeOne);
            // m_optionMuteSoundOutOfFocusToggle.OverrideFalseColor(Timer.GetTheme().GetCurrentColorScheme().m_backgroundHighlight);
            // m_optionMuteSoundOutOfFocusToggle.OverrideTrueColor(Timer.GetTheme().GetCurrentColorScheme().m_modeOne);
            // m_optionUnityAnalytics.OverrideFalseColor(Timer.GetTheme().GetCurrentColorScheme().m_backgroundHighlight);
            // m_optionUnityAnalytics.OverrideTrueColor(Timer.GetTheme().GetCurrentColorScheme().m_modeOne);
            
            // Init
            m_optionDigitFormat.Initialize(Timer);
            m_optionPomodoroCount.Initialize(Timer);
            m_optionEnableLongBreak.Initialize(Timer, systemSettings);
#if UNITY_ANDROID
            HideMuteSoundOutOfFocusOption();
#elif UNITY_IOS
            HideMuteSoundOutOfFocusOption();
#else
            m_optionMuteSoundOutOfFocusToggle.Initialize(Timer, systemSettings);
#endif
            
#if ENABLE_CLOUD_SERVICES_ANALYTICS
            m_optionUnityAnalytics.Initialize(Timer, systemSettings);
#else
            // Hide settings option if Unity Analytics not enabled on this platform.
            m_optionUnityAnalytics.gameObject.SetActive(false);
            m_optionUnityAnalytics.m_spacer.gameObject.SetActive(false);
#endif
        }

        public void Refresh()
        {
            // Gets triggered via dropdown on value changed event
            m_optionDigitFormat.SetDropdownValue((int)Timer.GetTimerSettings().m_format);
            m_optionPomodoroCount.SetDropdownValue(Timer.GetTimerSettings().m_pomodoroCount - 1);
            
            m_optionEnableLongBreak.Refresh(Timer.GetTimerSettings().m_longBreaks);
            m_optionMuteSoundOutOfFocusToggle.Refresh(Timer.GetSystemSettings().m_muteSoundWhenOutOfFocus);
            m_optionUnityAnalytics.Refresh(Timer.GetSystemSettings().m_enableUnityAnalytics);
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
                
                m_optionDigitFormat.ColorUpdate(Timer.GetTheme());
                
                m_optionMuteSoundOutOfFocusToggle.ColorUpdate(theme);
                
                m_optionUnityAnalytics.ColorUpdate(theme);
            }
        }

        /// <summary>
        /// Updates the switch digit layout dropdown to use the current timer's digit format.
        /// </summary>
        public void UpdateDropdown()
        {
            m_optionDigitFormat.SetDropdownValue(Timer.GetDigitFormatIndex());
        }

        // TODO: remove / rename
        /// <summary>
        /// Sets the switch digit layout dropdown to the provided digit format index.
        /// (See: <see cref="DigitFormat.SupportedFormats"/>)
        /// </summary>
        /// <param name="value">The index value you want the dropdown to be set at.</param>
        public void SetDropdown(int value)
        {
            m_optionDigitFormat.SetDropdownValue(value);
        }

        /// <summary>
        /// Displays this panel to the user.
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
            m_optionDigitFormat.SetDropdownValue(Timer.GetDigitFormatIndex());
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
            m_optionMuteSoundOutOfFocusToggle.gameObject.SetActive(true);
            m_optionMuteSoundOutOfFocusToggle.m_spacer.gameObject.SetActive(true);
        }

        /// <summary>
        /// Hides the 'sound mute when application is out of focus' option from the user.
        /// <remarks>Intended to be hidden for mobile users, not desktop.</remarks>
        /// </summary>
        public void HideMuteSoundOutOfFocusOption()
        {
            m_optionMuteSoundOutOfFocusToggle.gameObject.SetActive(false);
            m_optionMuteSoundOutOfFocusToggle.m_spacer.gameObject.SetActive(false);
        }
    }
}