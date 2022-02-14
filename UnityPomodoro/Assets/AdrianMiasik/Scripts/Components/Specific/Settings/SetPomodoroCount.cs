using System;
using AdrianMiasik.Components.Base;
using AdrianMiasik.Components.Core;

namespace AdrianMiasik.Components.Specific.Settings
{
    /// <summary>
    /// A <see cref="ThemeElement"/> dropdown with a label.
    /// Intended to be used for 'Set Pomodoro Count' settings option. (See <see cref="SettingsPanel"/>)
    /// </summary>
    public class SetPomodoroCount : SettingsOptionDropdown
    {
        /// <summary>
        /// <remarks>Used as a UnityEvent on the TMP_Dropdown. See <see cref="m_dropdown"/></remarks>
        /// </summary>
        /// <param name="i"></param>
        public void SetPomodoroLongBreakCount(Int32 i)
        {
            int desiredCount = i + 1; // Dependant on our dropdown options.

            if (Timer.HasTomatoProgression())
            {
                // If the new count removes/truncates potential progress...
                if (Timer.GetTomatoProgress() > desiredCount)
                {
                    Timer.SpawnConfirmationDialog(() =>
                    {
                        // Set to new count and remove additional progress
                        Timer.SetPomodoroCount(desiredCount, desiredCount);
                    }, () =>
                    {
                        SetDropdownValue(Timer.GetTomatoCount() - 1);
                    }, "This action will delete some of your pomodoro/tomato progress.");
                }
                else
                {
                    // New count number is higher than our progress
                    Timer.SetPomodoroCount(desiredCount, Timer.GetTomatoProgress());
                }
            }
            else
            {
                // No progress
                Timer.SetPomodoroCount(desiredCount, 0);
            }
        }
    }
}
