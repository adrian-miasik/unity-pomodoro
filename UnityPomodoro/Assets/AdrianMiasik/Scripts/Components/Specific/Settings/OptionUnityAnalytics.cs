using AdrianMiasik.Components.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AdrianMiasik.Components.Specific.Settings
{
    /// <summary>
    /// A <see cref="SettingsOptionToggleSlider"/> intended be used for 'Enable Unity Analytics' settings option. (See <see cref="SettingsPanel"/>)
    /// <remarks>On by default, but users can opt-out of telemetry / turn off the service at run-time by turning this boolean off (To the left position).</remarks>
    /// </summary>
    public class OptionUnityAnalytics : SettingsOptionToggleSlider
    {
        public override void Initialize(PomodoroTimer pomodoroTimer, TimerSettings settingsConfig)
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

            if (!state)
            {
                Debug.LogWarning("Unity Analytics - Restarting Application with Disabled Analytics.");
                
#if UNITY_EDITOR
                UnityEditor.EditorApplication.ExitPlaymode();
                // UnityEditor.EditorApplication.EnterPlaymode();
#else
                System.Diagnostics.Process.Start(Application.dataPath.Replace("_Data", ".exe"));
                Application.Quit();
#endif
            }
        }
    }
}