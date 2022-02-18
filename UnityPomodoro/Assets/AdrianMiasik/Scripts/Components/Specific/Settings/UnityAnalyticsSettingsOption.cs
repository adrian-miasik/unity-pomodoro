using AdrianMiasik.Components.Core;
using UnityEngine;
using UnityEngine.Analytics;

namespace AdrianMiasik.Components.Specific.Settings
{
    /// <summary>
    /// A settings option intended be used for 'Enable Unity Analytics' settings option. (See <see cref="SettingsPanel"/>)
    /// <remarks>On by default, but users can opt-out of telemetry / turn off the service at run-time by turning this boolean off (To the left position).</remarks>
    /// </summary>
    public class UnityAnalyticsSettingsOption : SettingsOptionToggleSlider
    {
        public override void Initialize(PomodoroTimer pomodoroTimer, ScriptableObjects.Settings settingsConfig)
        {
            base.Initialize(pomodoroTimer, settingsConfig);

            m_toggleSlider.m_onSetToTrueClick.AddListener(() =>
            {
                SetSettingUnityAnalytics(true);
            });
            m_toggleSlider.m_onSetToFalseClick.AddListener(() =>
            {
                SetSettingUnityAnalytics(false);
            });
            m_toggleSlider.Initialize(pomodoroTimer, settingsConfig.m_enableUnityAnalytics);
        }

        public void SetSettingUnityAnalytics(bool state)
        {
            Settings.m_enableUnityAnalytics = state;
            
            if (state)
            {
                Timer.StartServices();
                Analytics.ResumeInitialization();
                Debug.Log("Enabled Unity Analytics");
            }
            else
            {
                Analytics.FlushEvents();
                Timer.RestartApplication();
                Debug.Log("Disabled Unity Analytics");
            }
            
            Analytics.enabled = state;
            Analytics.deviceStatsEnabled = state;
            PerformanceReporting.enabled = state;
        }
    }
}
