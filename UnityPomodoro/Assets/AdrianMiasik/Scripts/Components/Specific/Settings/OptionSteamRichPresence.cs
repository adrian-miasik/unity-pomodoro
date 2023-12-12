using System;
using AdrianMiasik.Components.Base;
using AdrianMiasik.Components.Core.Containers;
using AdrianMiasik.Components.Core.Items.Pages;
using AdrianMiasik.Components.Core.Settings;
using Steamworks;

namespace AdrianMiasik.Components.Specific.Settings
{
    /// <summary>
    /// A <see cref="SettingsOptionDropdown"/> that toggles our Steam rich presence functionality.
    /// (See <see cref="DigitFormat"/>, also see <seealso cref="SettingsPage"/>)
    /// </summary>
    public class OptionSteamRichPresence : SettingsOptionToggleSlider
    {
        /// <summary>
        /// Sets the dropdown value to the <see cref="PomodoroTimer"/>'s current digit format.
        /// </summary>
        /// <param name="pomodoroTimer"></param>
        public override void Initialize(PomodoroTimer pomodoroTimer)
        {
            base.Initialize(pomodoroTimer);
            
            m_toggleSlider.m_onSetToTrueClick.AddListener(() =>
            {
                SetSteamRichPresence(true);
            });
            
            m_toggleSlider.m_onSetToFalseClick.AddListener(() =>
            {
                SetSteamRichPresence(false);
            });

            m_toggleSlider.Initialize(pomodoroTimer, Timer.GetSystemSettings().m_enableSteamRichPresence);
        }

        private void SetSteamRichPresence(bool enableRichPresence)
        {
            if (!enableRichPresence)
            {
                // Set and apply
                Timer.GetSystemSettings().m_enableSteamRichPresence = false;
                UserSettingsSerializer.SaveSettingsFile(Timer.GetSystemSettings().m_enableSteamRichPresence, "system-settings");
                
                // Clear current state if present
                SteamFriends.ClearRichPresence();
            }
            else
            {
                // Set and apply
                Timer.GetSystemSettings().m_enableSteamRichPresence = true;
                UserSettingsSerializer.SaveSettingsFile(Timer.GetSystemSettings().m_enableSteamRichPresence, "system-settings");

                // Update rich presence state
                Timer.UpdateSteamRichPresence();
            }
        }
    }
}