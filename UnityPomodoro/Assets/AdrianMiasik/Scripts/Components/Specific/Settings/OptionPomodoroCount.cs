using System;
using AdrianMiasik.Components.Base;
using AdrianMiasik.Components.Core;
using UnityEngine;

namespace AdrianMiasik.Components.Specific.Settings
{
    /// <summary>
    /// A <see cref="ThemeElement"/> dropdown with a label.
    /// Intended to be used for 'Set Pomodoro Count' settings option. (See <see cref="SettingsPanel"/>)
    /// </summary>
    public class OptionPomodoroCount : SettingsOptionDropdown
    {
        public override void Initialize(PomodoroTimer pomodoroTimer, bool updateColors = true)
        {
            base.Initialize(pomodoroTimer, updateColors);

            SetDropdownValue(Timer.GetTimerSettings().m_pomodoroCount - 1);
            m_dropdown.onValueChanged.AddListener(SetPomodoroLongBreakCount);
        }

        /// <summary>
        /// <remarks>Used as a UnityEvent on the Unity Pomodoro Dropdown. See <see cref="Dropdown"/></remarks>
        /// </summary>
        /// <param name="i"></param>
        private void SetPomodoroLongBreakCount(Int32 i)
        {
            Timer.GetTimerSettings().m_pomodoroCount = i + 1;
            UserSettingsSerializer.SaveTimerSettings(Timer.GetTimerSettings());

            int desiredCount = i + 1; // Dependant on our dropdown options.

            if (Timer.HasTomatoProgression())
            {
                // If the new count removes/truncates potential progress...
                if (Timer.GetTomatoProgress() > desiredCount)
                {
                    Timer.GetConfirmDialogManager().SpawnConfirmationDialog(() =>
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
