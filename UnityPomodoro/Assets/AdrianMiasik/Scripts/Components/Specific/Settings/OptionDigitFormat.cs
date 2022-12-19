using System;
using AdrianMiasik.Components.Base;
using AdrianMiasik.Components.Core.Containers;
using AdrianMiasik.Components.Core.Items.Pages;

namespace AdrianMiasik.Components.Specific.Settings
{
    /// <summary>
    /// A <see cref="SettingsOptionDropdown"/> that changes our current timers digit format.
    /// (See <see cref="DigitFormat"/>, also see <seealso cref="SettingsPage"/>)
    /// </summary>
    public class OptionDigitFormat : SettingsOptionDropdown
    {
        /// <summary>
        /// Sets the dropdown value to the <see cref="PomodoroTimer"/>'s current digit format.
        /// </summary>
        /// <param name="pomodoroTimer"></param>
        public void Initialize(PomodoroTimer pomodoroTimer)
        {
            base.Initialize(pomodoroTimer);

            m_dropdown.onValueChanged.AddListener(TryChangeFormat);

            if (Timer.HaveComponentsBeenInitialized())
            {
                SetDropdownValue(Timer.GetDigitFormatIndex());
            }
        }

        /// <summary>
        /// Attempts to change the digit format using enum index, will prompt user with confirmation dialog
        /// if necessary. See <see cref="DigitFormat.SupportedFormats"/>.
        /// </summary>
        /// <param name="i"></param>
        private void TryChangeFormat(Int32 i)
        {
            Timer.TryChangeFormat((DigitFormat.SupportedFormats)i);
        }
    }
}