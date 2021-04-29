namespace AdrianMiasik.PomodoroTimer.Interfaces
{
    public interface ITimerState
    {
        public void StateUpdate(PomodoroTimer.States state);
    }
}