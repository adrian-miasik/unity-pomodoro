using System;
using AdrianMiasik.UWP;

namespace AdrianMiasik.Components.Core
{
    /// <summary>
    /// Global settings for the application. (Not specific to a timer, but instead the entire application)
    /// </summary>
    [Serializable]
    public class SystemSettings
    {
        public bool m_darkMode = true;        
        
        /// <summary>
        /// Should we mute the sound when the application is not in focus?
        /// If `True`, the application audio will not be played when it's not in focus.
        /// If `False`, the application audio will emit audio regardless of focus state. 
        /// <remarks> We want this to be `True` by default for the Universal Windows Platform since we
        /// pull focus back via UWP notification. (See <see cref="NotificationManager"/>)</remarks>
        /// </summary>
        public bool m_muteSoundWhenOutOfFocus;

        /// <summary>
        /// Do you want to enable the Unity Analytics service?
        /// If `True`, the Unity Analytics service will track user data.
        /// If `False`, the Unity Analytics service will no longer run and track user data.
        /// </summary>
        public bool m_enableUnityAnalytics;
    }
}