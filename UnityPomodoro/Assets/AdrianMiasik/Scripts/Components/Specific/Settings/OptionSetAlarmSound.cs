using System;
using System.Collections.Generic;
using AdrianMiasik.Components.Base;
using AdrianMiasik.Components.Core.Settings;
using UnityEngine;

namespace AdrianMiasik.Components.Specific.Settings
{
    public class OptionSetAlarmSound : SettingsOptionDropdown
    {
        [SerializeField] private List<AudioClip> m_alarmSounds;

        public override void Initialize(PomodoroTimer pomodoroTimer, bool updateColors = true)
        {
            base.Initialize(pomodoroTimer, updateColors);

            // Set alarm sound (directly) based on current setting
            m_dropdown.value = Timer.GetTimerSettings().m_alarmSoundIndex;
            Timer.SetAlarmSound(m_alarmSounds[Timer.GetTimerSettings().m_alarmSoundIndex], false);

            // Invoke SetAlarmSound anytime the dropdown value changes
            m_dropdown.onValueChanged.AddListener(SetAlarmSound);
        }

        private void SetAlarmSound(Int32 i)
        {
            // Apply change to settings
            Timer.GetTimerSettings().m_alarmSoundIndex = i;
            UserSettingsSerializer.SaveSettingsFile(Timer.GetTimerSettings(), "timer-settings");

            // Apply change to timer
            Timer.SetAlarmSound(m_alarmSounds[i], true);
        }
    }
}