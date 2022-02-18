using System.Collections.Generic;
using AdrianMiasik.UWP;
using UnityEngine;

namespace AdrianMiasik.ScriptableObjects
{
    // TODO: Add set digit format preference to here (since it's timer dependent)
    // TODO: Add pomodoro count preference to here (since it's timer dependant)
    // TODO: Remove mute sound when out of focus (Since it's a system wide setting similar to dark mode)
    /// <summary>
    /// Responsible for keeping track of the users timer preferences for the current timer.
    /// </summary>
    [CreateAssetMenu(fileName = "New Settings Configuration", 
        menuName = "Adrian Miasik/Create New Settings Configuration")]
    public class Settings : ScriptableObject
    {
        /// <summary>
        /// A list of timers that are currently using this settings configuration.
        /// <remarks>Intended to be used for propagating editor changes down to each timers settings page. (I.e
        /// when we edit a scriptable object setting via editor / inspector, we want all the timers to update visually
        /// without having to re-load the settings page for each timer to see their new state. It's a similar
        /// concept to our <see cref="Theme"/>'s where we force a refresh on editor validate.)</remarks>
        /// </summary>
        private List<PomodoroTimer> timers = new List<PomodoroTimer>();
        
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

        private void OnValidate()
        {
            foreach (PomodoroTimer timer in timers)
            {
                timer.UpdateSettings(this);
            }
        }

        private void OnEnable()
        {
            timers.Clear();
        }
        
        private void Awake()
        {
            ApplyDefaults();
        }

        /// <summary>
        /// Configures our default settings using platform specific define directives. (I.e. Based on current
        /// Operating System)
        /// </summary>
        [ContextMenu("Reset to Defaults")]
        private void ApplyDefaults()
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

            // All platforms have analytics on by default. (User can opt-out though)
            m_enableUnityAnalytics = true;
            
            // Propagate changes down
            OnValidate();
        }

        public void RegisterTimer(PomodoroTimer timer)
        {
            if (!timers.Contains(timer))
            {
                timers.Add(timer);
            }
        }
        
        /// <summary>
        /// Displays into the console all the associated PomodoroTimers (<see cref="PomodoroTimer"/>) objects that have
        /// been registered to this settings config.
        /// </summary>
        [ContextMenu("List Timers")]
        public void ListTimers()
        {
            foreach (PomodoroTimer timer in timers)
            {
                Debug.Log(timer.ToString(), timer.gameObject);    
            }
        }
    }
}