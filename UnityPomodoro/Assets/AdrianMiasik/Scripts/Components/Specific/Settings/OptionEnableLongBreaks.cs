using AdrianMiasik.Components.Base;
using AdrianMiasik.Components.Core.Settings;
using AdrianMiasik.Components.Specific.Pages;
using UnityEngine;

namespace AdrianMiasik.Components.Specific.Settings
{
    /// <summary>
    /// A <see cref="SettingsOptionDropdown"/> that enables / disables pomodoro progression for long breaks.
    /// (See <see cref="PomodoroTimer.SetSettingLongBreaks"/>, also see <seealso cref="SettingsPage"/>)
    /// </summary>
    public class OptionEnableLongBreaks : SettingsOptionToggleSlider
    {
        public override void Initialize(PomodoroTimer pomodoroTimer)
        {
            base.Initialize(pomodoroTimer);
            if (Timer.GetTimerSettings().m_longBreaks)
            {
                m_toggleSlider.Initialize(pomodoroTimer, Timer.GetTimerSettings().m_longBreaks);
            }
        }

        /// <summary>
        /// <remarks>Used as a UnityEvent by toggle.</remarks>
        /// </summary>
        /// <param name="state"></param>
        public void SetSettingLongBreak(bool state)
        {
            if (state == false)
            {
                if (Timer.HasTomatoProgression())
                {
                    // Prompt for user permission first
                    Timer.GetConfirmDialogManager().SpawnConfirmationDialog(() =>
                    {
                        Timer.SetSettingLongBreaks(false);
                    }, () =>
                    {
                        // Cancel visuals if they don't agree, similar how we do the work/break slider
                        m_toggleSlider.Refresh(true);
                        
                        // Edge condition: If playing in editor and tweaking values via inspector...
#if UNITY_EDITOR
                        if (Application.isPlaying)
                        {
                            // Apply and save
                            Timer.GetTimerSettings().m_longBreaks = true;
                            UserSettingsSerializer.SaveTimerSettings(Timer.GetTimerSettings());
                        }
#endif
                    }, "This action will delete your current pomodoro/tomato progress.");
                }
                else
                {
                    // Set immediately
                    Timer.SetSettingLongBreaks(false);
                }
            }
            else
            {
                // Set immediately
                Timer.SetSettingLongBreaks();
            }
        }
    }
}