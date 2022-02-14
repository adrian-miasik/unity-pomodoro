using AdrianMiasik.Components.Core;

namespace AdrianMiasik.Components.Specific.Settings
{
    /// <summary>
    /// A settings option intended be used for 'Enable long break' settings option. (See <see cref="SettingsPanel"/>)
    /// </summary>
    public class LongBreaks : SettingsOptionToggleSlider
    {
        public override void Initialize(PomodoroTimer pomodoroTimer, ScriptableObjects.Settings settingsConfig)
        {
            base.Initialize(pomodoroTimer, settingsConfig);
            if (Settings.m_longBreaks)
            {
                m_toggleSlider.Initialize(pomodoroTimer, Settings.m_longBreaks);
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
                    Timer.SpawnConfirmationDialog(() =>
                    {
                        Timer.SetSettingLongBreaks(false);
                    }, (() =>
                    {
                        // Cancel visuals if they don't agree, similar how we do the work/break slider
                        m_toggleSlider.Initialize(Timer, true);
                    }), "This action will delete your current pomodoro/tomato progress.");
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