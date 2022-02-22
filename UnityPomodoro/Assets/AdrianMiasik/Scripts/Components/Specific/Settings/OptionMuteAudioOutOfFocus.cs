using AdrianMiasik.Components.Core;
using UnityEngine;

namespace AdrianMiasik.Components.Specific.Settings
{
    public class OptionMuteAudioOutOfFocus : SettingsOptionToggleSlider
    {
        public override void Initialize(PomodoroTimer pomodoroTimer, SystemSettings systemSettings)
        {
            base.Initialize(pomodoroTimer, systemSettings);
            
            m_toggleSlider.m_onSetToTrueClick.AddListener((() =>
            {
                SetSettingMuteSoundWhenOutOfFocus(true);
                ColorUpdate(pomodoroTimer.GetTheme());
            }));
            m_toggleSlider.m_onSetToFalseClick.AddListener((() =>
            {
                SetSettingMuteSoundWhenOutOfFocus();
                ColorUpdate(pomodoroTimer.GetTheme());
            }));
            m_toggleSlider.Initialize(pomodoroTimer, systemSettings.m_muteSoundWhenOutOfFocus);
        }

        /// <summary>
        /// Sets the users setting preference to mute the application when out of focus using the provided
        /// <see cref="bool"/>.
        /// <remarks>Intended to be used as a UnityEvent. Otherwise you can directly do this
        /// on the public property in the settings object.</remarks>
        /// </summary>
        /// <param name="state">Do you want to mute this application when it's out of focus?</param>
        private void SetSettingMuteSoundWhenOutOfFocus(bool state = false)
        {
            // Apply and save
            Timer.GetSystemSettings().m_muteSoundWhenOutOfFocus = state;
            UserSettingsSerializer.SaveSystemSettings(Timer.GetSystemSettings());
        }
    }
}