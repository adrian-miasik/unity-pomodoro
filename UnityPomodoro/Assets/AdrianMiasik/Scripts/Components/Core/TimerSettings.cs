﻿using System;
using AdrianMiasik.Components.Core.Containers;

namespace AdrianMiasik.Components.Core
{
    /// <summary>
    /// Settings that are to be applied / correspond to a PomodoroTimer.
    /// </summary>
    [Serializable]
    public class TimerSettings
    {
        /// <summary>
        /// The timers current time format.
        /// </summary>
        public DigitFormat.SupportedFormats m_format = DigitFormat.SupportedFormats.HH_MM_SS;

        /// <summary>
        /// Enable timer long breaks?
        /// If `True`, the user will be able to accumulate tomatoes/pomodoro & long breaks. (Work/Break/Long Break)
        /// If `False`, the user will not be able to user long breaks. (Work/Break): The way the
        /// timer worked on version 1.6.0. or lower). 
        /// </summary>
        public bool m_longBreaks = true;

        /// <summary>
        /// How many pomodoro / tomatoes does the user need to unlock the long break?
        /// <remarks>According to the Pomodoro Technique, default is 4.</remarks>
        /// </summary>
        public int m_pomodoroCount = 4;
    }
}