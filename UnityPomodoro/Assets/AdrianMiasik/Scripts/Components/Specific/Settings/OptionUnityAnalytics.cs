using System.Collections.Generic;
using AdrianMiasik.Components.Base;
using AdrianMiasik.Components.Core;
using AdrianMiasik.Components.Core.Settings;
using AdrianMiasik.Components.Specific.Pages;
using Unity.Services.Analytics;
using UnityEngine;

namespace AdrianMiasik.Components.Specific.Settings
{
    /// <summary>
    /// A <see cref="SettingsOptionToggleSlider"/> intended be used for 'Enable Unity Analytics' settings option. (See <see cref="SettingsPage"/>)
    /// <remarks>On by default, but users can opt-out of telemetry / turn off the service at run-time by turning this boolean off (To the left position).</remarks>
    /// </summary>
    public class OptionUnityAnalytics : SettingsOptionToggleSlider
    {
        public override void Initialize(PomodoroTimer pomodoroTimer)
        {
            base.Initialize(pomodoroTimer);

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
            m_toggleSlider.Initialize(pomodoroTimer, Timer.GetSystemSettings().m_enableUnityAnalytics);
        }

        private void SetSettingUnityAnalytics(bool state)
        {
            if (!state)
            {
                // Disable analytics
                Timer.ToggleUnityAnalytics(false, false);
            }
            else
            {
                // Enable analytics
                Timer.ToggleUnityAnalytics(true, false);
            }
        }

#if UNITY_ANDROID
        /// <summary>
        /// Restarts our android application and re-opens the process.
        /// </summary>
        private static void RestartAndroidProcess()
        {
            using AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");

            const int kIntent_FLAG_ACTIVITY_CLEAR_TASK = 0x00008000;
            const int kIntent_FLAG_ACTIVITY_NEW_TASK = 0x10000000;

            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject packageManager = currentActivity.Call<AndroidJavaObject>("getPackageManager");
            AndroidJavaObject intent = packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage",
                Application.identifier);

            intent.Call<AndroidJavaObject>("setFlags", 
                kIntent_FLAG_ACTIVITY_NEW_TASK | kIntent_FLAG_ACTIVITY_CLEAR_TASK);
            currentActivity.Call("startActivity", intent);
            currentActivity.Call("finish");
            AndroidJavaClass process = new AndroidJavaClass("android.os.Process");
            int pid = process.CallStatic<int>("myPid");
            process.CallStatic("killProcess", pid);
        }
#endif
    }
}