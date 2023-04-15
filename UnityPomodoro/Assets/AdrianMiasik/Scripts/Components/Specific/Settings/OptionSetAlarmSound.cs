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

            m_dropdown.onValueChanged.AddListener(SetAlarmSound);

            SetDropdownValue(Timer.GetTimerSettings().m_alarmSoundIndex);
        }

        private void SetAlarmSound(Int32 i)
        {
            Timer.GetTimerSettings().m_alarmSoundIndex = i;
            UserSettingsSerializer.SaveSettingsFile(Timer.GetTimerSettings(), "timer-settings");

            Timer.SetAlarmSound(m_alarmSounds[i]);
        }
    }
}