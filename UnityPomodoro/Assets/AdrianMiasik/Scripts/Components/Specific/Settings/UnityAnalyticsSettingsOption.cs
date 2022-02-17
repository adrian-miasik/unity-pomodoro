using AdrianMiasik.Components.Core;
using UnityEditor.CrashReporting;
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
            SetSettingUnityAnalytics(settingsConfig.m_enableUnityAnalytics);
            m_toggleSlider.Initialize(pomodoroTimer, settingsConfig.m_enableUnityAnalytics);
        }

        public void SetSettingUnityAnalytics(bool state)
        {
            Analytics.enabled = state;
            Analytics.deviceStatsEnabled = state;
            PerformanceReporting.enabled = state;
            CrashReportingSettings.enabled = state;
            Settings.m_enableUnityAnalytics = state;

            Debug.Log("ua: " + Analytics.enabled);
            
            if (state)
            {
                Debug.Log("Enabled Unity Analytics");
            }
            else
            {
                Debug.Log("Disabled Unity Analytics");
            }
        }
    }
}
