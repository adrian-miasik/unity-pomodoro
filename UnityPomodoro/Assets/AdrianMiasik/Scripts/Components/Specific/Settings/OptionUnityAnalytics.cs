using AdrianMiasik.Components.Core;

namespace AdrianMiasik.Components.Specific.Settings
{
    /// <summary>
    /// A <see cref="SettingsOptionToggleSlider"/> intended be used for 'Enable Unity Analytics' settings option. (See <see cref="SettingsPanel"/>)
    /// <remarks>On by default, but users can opt-out of telemetry / turn off the service at run-time by turning this boolean off (To the left position).</remarks>
    /// </summary>
    public class OptionUnityAnalytics : SettingsOptionToggleSlider
    {
        public override void Initialize(PomodoroTimer pomodoroTimer, ScriptableObjects.Settings settingsConfig)
        {
            base.Initialize(pomodoroTimer, settingsConfig);

            // Triggering true / false will invoke our analytics logic enable / disable
            m_toggleSlider.m_onSetToTrueClick.AddListener(() =>
            {
                SetSettingUnityAnalytics(true);
            });
            m_toggleSlider.m_onSetToFalseClick.AddListener(() =>
            {
                SetSettingUnityAnalytics(false);
            });
            
            // Setup toggle slider visuals to match user setting
            m_toggleSlider.Initialize(pomodoroTimer, settingsConfig.m_enableUnityAnalytics);
        }

        private void SetSettingUnityAnalytics(bool state)
        {
            Timer.ToggleUnityAnalytics(state);
        }
    }
}