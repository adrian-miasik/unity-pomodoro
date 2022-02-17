using AdrianMiasik.UWP;
using UnityEngine;
using UnityEngine.Analytics;

namespace AdrianMiasik.ScriptableObjects
{
    // TODO: Add/move set digit format preference to here - SupportedFormats (since it's timer dependent)
    // TODO: Remove mute sound when out of focus (Since it's a system wide setting similar to dark mode)
    /// <summary>
    /// Responsible for keeping track of the users timer preferences.
    /// </summary>
    [CreateAssetMenu(fileName = "New Settings Configuration", 
        menuName = "Adrian Miasik/Create New Settings Configuration")]
    public class Settings : ScriptableObject
    {
        /// <summary>
        /// Should we mute the sound when the application is not in focus?
        /// If `True`, the application audio will not be played when it's not in focus.
        /// If `False`, the application audio will emit audio regardless of focus state. 
        /// <remarks> We want this to be `True` by default for the Universal Windows Platform since we
        /// pull focus back via UWP notification. (See <see cref="NotificationManager"/>)</remarks>
        /// </summary>
        public bool m_muteSoundWhenOutOfFocus;

        /// <summary>
        /// Enable timer long breaks?
        /// If `True`, the user will be able to accumulate tomatoes/pomodoro & long breaks. (Work/Break/Long Break)
        /// If `False`, the user will not be able to user long breaks. (Work/Break): The way the
        /// timer worked on version 1.6.0. or lower). 
        /// </summary>
        public bool m_longBreaks;

        /// <summary>
        /// Do you want to enable the Unity Analytics service?
        /// If `True`, the Unity Analytics service will track user data.
        /// If `False`, the Unity Analytics service will no longer run and track user data.
        /// </summary>
        public bool m_enableUnityAnalytics;

        private void Awake()
        {
            ApplyPlatformDefaults();
        }

        /// <summary>
        /// Configures our default settings using platform specific define directives. (I.e. Based on current
        /// Operating System)
        /// </summary>
        public void ApplyPlatformDefaults()
        {
            // All platforms have long breaks on by default
            m_longBreaks = true;
            
            // Apply mute out of focus
#if UNITY_STANDALONE_OSX
            m_muteSoundWhenOutOfFocus = false;
#elif UNITY_STANDALONE_LINUX
            m_muteSoundWhenOutOfFocus = false;
#elif UNITY_STANDALONE_WIN
            m_muteSoundWhenOutOfFocus = false;
#elif UNITY_WSA // UWP
            m_muteSoundWhenOutOfFocus = true; // Set to true since our UWP Notification will pull focus back to our app
#elif UNITY_ANDROID
            m_muteSoundWhenOutOfFocus = false; // Doesn't quite matter for mobile
#elif UNITY_IOS
            m_muteSoundWhenOutOfFocus = false; // Doesn't quite matter for mobile.
#endif
            // Enable Unity Analytics by default, user can opt-out:
            // See Settings Panel and UnityAnalyticsSettingsOption.cs
            Analytics.enabled = true;
            Analytics.deviceStatsEnabled = true;
            PerformanceReporting.enabled = true;
            Analytics.ResumeInitialization();
            m_enableUnityAnalytics = true;
        }
    }
}