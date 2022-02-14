using AdrianMiasik.Components.Core;

namespace AdrianMiasik.Components.Specific.Settings
{
    public class SetMuteAudioOutOfFocus : SettingsOptionToggleSlider
    {
        /// <summary>
        /// Sets the users setting preference to mute the application when out of focus using the provided
        /// <see cref="bool"/>.
        /// <remarks>Intended to be used as a UnityEvent. Otherwise you can directly do this
        /// on the public property in the settings object.</remarks>
        /// </summary>
        /// <param name="state">Do you want to mute this application when it's out of focus?</param>
        public void SetSettingMuteSoundWhenOutOfFocus(bool state = false)
        {
            // Change setting
            Settings.m_muteSoundWhenOutOfFocus = state;
        }
    }
}