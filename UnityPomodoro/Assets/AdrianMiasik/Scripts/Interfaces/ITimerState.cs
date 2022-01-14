using AdrianMiasik.ScriptableObjects;

namespace AdrianMiasik.Interfaces
{
    public interface ITimerState
    {
        /// <summary>
        /// Invoked when the timers current state changes.
        /// </summary>
        /// <param name="state">The new timer state</param>
        /// <param name="theme">The current theme</param>
        public void StateUpdate(PomodoroTimer.States state, Theme theme);
    }
}