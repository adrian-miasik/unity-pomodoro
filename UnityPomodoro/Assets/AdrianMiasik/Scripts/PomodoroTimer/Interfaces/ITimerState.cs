namespace AdrianMiasik.PomodoroTimer
{
    public interface ITimerState
    {
        public void StateUpdate(PomodoroTimer.States state);
    }
}