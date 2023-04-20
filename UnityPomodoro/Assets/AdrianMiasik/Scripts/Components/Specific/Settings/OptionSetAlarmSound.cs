using System;
using System.Collections.Generic;
using System.Linq;
using AdrianMiasik.Components.Base;
using AdrianMiasik.Components.Core.Settings;
using QFSW.QC;
using TMPro;
using UnityEngine;

namespace AdrianMiasik.Components.Specific.Settings
{
    public class OptionSetAlarmSound : SettingsOptionDropdown
    {
        [SerializeField] private List<AudioClip> m_preinstalledAlarms;

        private Dictionary<int, AudioClip> alarmBank;

        public override void Initialize(PomodoroTimer pomodoroTimer, bool updateColors = true)
        {
            base.Initialize(pomodoroTimer, updateColors);

            alarmBank = new Dictionary<int, AudioClip>();

            // Move pre-installed alarms to alarm bank...
            for (int i = 0; i < m_preinstalledAlarms.Count; i++)
            {
                AudioClip audioClip = m_preinstalledAlarms[i];
                alarmBank.TryAdd(i, audioClip);
            }

            ValidateAlarmSoundIndex();

            // Invoke SetAlarmSound anytime the dropdown value changes
            m_dropdown.onValueChanged.AddListener(SetAlarmSound);
        }

        public void ValidateAlarmSoundIndex()
        {
            // Get saved key
            int startingIndex = Timer.GetTimerSettings().m_alarmSoundIndex;

            bool isIndexValid = false;

            // Iterate through alarm bank...
            foreach (int i in alarmBank.Keys.OrderBy(i => i))
            {
                // If this key matches our saved key...
                if (i == startingIndex)
                {
                    // Mark key as valid
                    isIndexValid = true;
                    break;
                }
            }

            // Reset to default alarm if saved key in invalid...
            if (!isIndexValid)
            {
                Debug.LogWarning("Preferred alarm sound key [" + startingIndex + "] not found. " +
                                 "Reverting back to default alarm. [0]");
                startingIndex = 0;
            }

            // Set alarm sound (directly) based on current setting
            if (alarmBank.TryGetValue(startingIndex, out AudioClip c))
            {
                m_dropdown.value = startingIndex;
                Timer.SetAlarmSound(c, false);
            }
        }

        private void SetAlarmSound(Int32 i)
        {
            // Apply change to settings
            Timer.GetTimerSettings().m_alarmSoundIndex = i;
            UserSettingsSerializer.SaveSettingsFile(Timer.GetTimerSettings(), "timer-settings");

            // Apply change to timer
            if (alarmBank.TryGetValue(i, out AudioClip c))
            {
                Timer.SetAlarmSound(c, true);
            }
        }

        public void AddCustomDropdownSoundOption(AudioClip audio, string alarmName)
        {
            // Add audio to sound bank
            int nextOpenIndex = alarmBank.Count;
            alarmBank.TryAdd(nextOpenIndex, audio);

            // Add option to dropdown
            m_dropdown.options.Add(new TMP_Dropdown.OptionData(alarmName));
        }

        private void RemoveCustomDropdownSoundOption(int key)
        {
            if (alarmBank.TryGetValue(key, out AudioClip audioClip))
            {
                // Remove from dictionary
                alarmBank.Remove(key);

                // Iterate through all the custom audio options...
                for (int i = m_preinstalledAlarms.Count - 1; i < m_dropdown.options.Count; i++)
                {
                    // If the removed alarm from the bank matches the dropdown option...
                    if (m_dropdown.options[i].text == audioClip.name)
                    {
                        // Remove drop down option
                        m_dropdown.options.RemoveAt(i);
                    }
                }
            }
        }

        [Command("print-custom-alarm-sound-dictionary")]
        [ContextMenu("Print 'Custom alarm sounds' dictionary.")]
        public void PrintCustomAlarmSoundsDictionary()
        {
            if (alarmBank == null)
            {
                Debug.LogWarning("Nothing has been added to the custom alarm dictionary.");
                return;
            }
            
            foreach (KeyValuePair<int, AudioClip> pair in alarmBank)
            {
                Debug.Log("key[" +pair.Key + "] is " + pair.Value.name);
            }
        }

        /// <summary>
        /// Disposes of all our custom alarm sounds.
        /// </summary>
        public void ResetCustomSoundFiles()
        {
            if (alarmBank.Count <= m_preinstalledAlarms.Count)
            {
                // No custom sound files have been registered/found. Reset is redundant.
                // Early exit
                return;
            }

            Debug.Log("Recalculating Custom Alarms...");

            // Copy
            Dictionary<int, AudioClip> cachedAdditionalSounds = new(alarmBank);

            // Iterate through each key in order...
            foreach (int index in cachedAdditionalSounds.Keys.OrderBy(i => i))
            {
                // Ignore pre-installed alarms in alarm bank...
                if (index <= m_preinstalledAlarms.Count - 1)
                {
                    continue;
                }

                // Remove custom sound entry only.
                RemoveCustomDropdownSoundOption(index);
            }

            // Clear copy
            cachedAdditionalSounds.Clear();
        }
    }
}
