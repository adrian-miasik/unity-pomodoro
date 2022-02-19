using System;

namespace AdrianMiasik.Components.Core
{
    [Serializable]
    public class TimerSettings
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
    }
}