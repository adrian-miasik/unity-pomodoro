using System;
using AdrianMiasik.Components.Base;
using AdrianMiasik.Components.Core;
using AdrianMiasik.Components.Core.Containers;

namespace AdrianMiasik.Components.Specific.Settings
{
    /// <summary>
    /// A <see cref="ThemeElement"/> dropdown with a label.
    /// Intended to be used for 'Set digit format' settings option. (See <see cref="SettingsPanel"/>)
    /// </summary>
    public class SetDigitFormat : SettingsOptionDropdown
    {
        /// <summary>
        /// Sets the dropdown value to the <see cref="PomodoroTimer"/>'s current digit format.
        /// </summary>
        /// <param name="pomodoroTimer"></param>
        public void Initialize(PomodoroTimer pomodoroTimer)
        {
            base.Initialize(pomodoroTimer);

            m_dropdown.onValueChanged.AddListener(TryChangeFormat);

            SetDropdownValue(Timer.GetDigitFormatIndex());
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