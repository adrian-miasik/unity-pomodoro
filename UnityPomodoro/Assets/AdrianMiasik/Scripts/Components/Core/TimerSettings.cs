using System;

namespace AdrianMiasik.Components.Core
{
    /// <summary>
    /// Settings that are to be applied / correspond to a PomodoroTimer.
    /// </summary>
    [Serializable]
    public class TimerSettings
    {
        // TODO: Set Digit Format
        // TODO: Set Pomodoro Count

        /// <summary>
        /// Enable timer long breaks?
        /// If `True`, the user will be able to accumulate tomatoes/pomodoro & long breaks. (Work/Break/Long Break)
        /// If `False`, the user will not be able to user long breaks. (Work/Break): The way the
        /// timer worked on version 1.6.0. or lower). 
        /// </summary>
        public bool m_longBreaks;
    }
}