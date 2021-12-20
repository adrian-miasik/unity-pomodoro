using AdrianMiasik.ScriptableObjects;

namespace AdrianMiasik.Interfaces
{
    public interface ITimerState
    {
        public void StateUpdate(PomodoroTimer.States _state, Theme _theme);
    }
}