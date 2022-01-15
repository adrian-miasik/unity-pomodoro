using AdrianMiasik.Components.Base;
using AdrianMiasik.Components.Core;
using AdrianMiasik.Components.Core.Containers;
using AdrianMiasik.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace AdrianMiasik.Components.Specific
{
    /// <summary>
    /// A <see cref="ThemeElement"/> page used to display a set of interactable user options. Currently supports
    /// switching between different digit formats and muting our application when it's not in focus.
    /// See: <see cref="DigitFormat.SupportedFormats"/>.
    /// </summary>
    public class SettingsPanel : ThemeElement
    {
        [SerializeField] private TMP_Text m_title;
        [SerializeField] private DigitFormatDropdown m_digitFormatDropdown;
        [SerializeField] private TMP_Text m_muteSoundOutOfFocusLabel;
        [FormerlySerializedAs("m_muteSoundOutOfFocusBoolean")] [SerializeField] private ToggleSlider m_muteSoundOutOfFocusToggle;
        
        private bool isOpen;

        public void Initialize(PomodoroTimer pomodoroTimer)
        {
            base.Initialize(pomodoroTimer, false);
            
            m_muteSoundOutOfFocusToggle.OverrideFalseColor(Timer.GetTheme().GetCurrentColorScheme().m_backgroundHighlight);
            m_muteSoundOutOfFocusToggle.OverrideTrueColor(Timer.GetTheme().GetCurrentColorScheme().m_modeOne);
            
            m_digitFormatDropdown.Initialize(Timer);
            m_muteSoundOutOfFocusToggle.Initialize(pomodoroTimer, pomodoroTimer.MuteSoundWhenOutOfFocus());
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
                m_digitFormatDropdown.ColorUpdate(Timer.GetTheme());
                m_muteSoundOutOfFocusLabel.color = theme.GetCurrentColorScheme().m_foreground;
                m_muteSoundOutOfFocusToggle.ColorUpdate(theme);
            }
        }

        /// <summary>
        /// Updates the switch digit layout dropdown to use the current timer's digit format.
        /// </summary>
        public void UpdateDropdown()
        {
            m_digitFormatDropdown.SetDropdownValue(Timer.GetDigitFormat());
        }

        /// <summary>
        /// Sets the switch digit layout dropdown to the provided digit format index.
        /// (See: <see cref="DigitFormat.SupportedFormats"/>)
        /// </summary>
        /// <param name="value">The index value you want the dropdown to be set at.</param>
        public void SetDropdown(int value)
        {
            m_digitFormatDropdown.SetDropdownValue(value);
        }
        
        /// <summary>
        /// Displays this panel to the user.
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
            m_digitFormatDropdown.SetDropdownValue(Timer.GetDigitFormat());
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
            m_muteSoundOutOfFocusLabel.gameObject.SetActive(true);
            m_muteSoundOutOfFocusToggle.gameObject.SetActive(true);
        }
        
        /// <summary>
        /// Hides the 'sound mute when application is out of focus' option from the user.
        /// <remarks>Intended to be hidden for mobile users, not desktop.</remarks>
        /// </summary>
        public void HideMuteSoundOutOfFocusOption()
        {
            m_muteSoundOutOfFocusLabel.gameObject.SetActive(false);
            m_muteSoundOutOfFocusToggle.gameObject.SetActive(false);
        }
    }
}
