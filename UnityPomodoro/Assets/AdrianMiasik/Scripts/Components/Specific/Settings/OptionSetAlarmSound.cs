using System;
using System.Collections.Generic;
using AdrianMiasik.Components.Base;
using AdrianMiasik.Components.Core.Settings;
using QFSW.QC;
using TMPro;
using UnityEngine;

namespace AdrianMiasik.Components.Specific.Settings
{
    public class OptionSetAlarmSound : SettingsOptionDropdown
    {
        [SerializeField] private List<AudioClip> m_alarmSounds;

        private Dictionary<AudioClip, int> customAdditionalSounds;

        public override void Initialize(PomodoroTimer pomodoroTimer, bool updateColors = true)
        {
            base.Initialize(pomodoroTimer, updateColors);

            ValidateAlarmSoundIndex();

            // Invoke SetAlarmSound anytime the dropdown value changes
            m_dropdown.onValueChanged.AddListener(SetAlarmSound);
        }

        public void ValidateAlarmSoundIndex()
        {
            // Validate index
            int startingIndex = Timer.GetTimerSettings().m_alarmSoundIndex;

            // If preferred alarm sound is custom...(bounds check)
            if (startingIndex > m_dropdown.options.Count - 1)
            {
                Debug.LogWarning("Preferred alarm sound index exceeds options dropdown count. " +
                                 "Resetting alarm sound back to default.");
                startingIndex = 0;
            }

            // Set alarm sound (directly) based on current setting
            m_dropdown.value = startingIndex;
            Timer.SetAlarmSound(m_alarmSounds[startingIndex], false);
        }

        private void SetAlarmSound(Int32 i)
        {
            Debug.LogWarning("Playing sound!");
            
            // Apply change to settings
            Timer.GetTimerSettings().m_alarmSoundIndex = i;
            UserSettingsSerializer.SaveSettingsFile(Timer.GetTimerSettings(), "timer-settings");

            // Apply change to timer
            Timer.SetAlarmSound(m_alarmSounds[i], true);
        }

        public void AddCustomDropdownSoundOption(AudioClip audio, string alarmName)
        {
            // Init dictionary
            customAdditionalSounds ??= new Dictionary<AudioClip, int>();

            // Add audio to sound bank
            m_alarmSounds.Add(audio);

            // Add option to dropdown
            m_dropdown.options.Add(new TMP_Dropdown.OptionData(alarmName));

            // Register to dictionary
            customAdditionalSounds.Add(audio, m_dropdown.options.Count - 1);

            Debug.Log("Custom sound has been added successfully.");
        }

        public void RemoveCustomDropdownSoundOption(AudioClip audio)
        {
            // Check if additional sound is found...
            if (customAdditionalSounds.TryGetValue(audio, out int index))
            {
                // Deregister from dictionary
                customAdditionalSounds.Remove(audio);

                // Remove from dropdown options
                m_dropdown.options.RemoveAt(index);

                // Remove from sound bank
                m_alarmSounds.RemoveAt(index);
            }
            else
            {
                Debug.LogWarning("Unable to remove sound drop down option. The provided audio has not been" +
                                 "found and therefore hasn't been added to the drop down options. " +
                                 "See AddCustomDropdownSoundOption()");
            }
        }

        [Command("print-custom-alarm-sound-dictionary")]
        [ContextMenu("Print 'Custom alarm sounds' dictionary.")]
        public void PrintCustomAlarmSoundsDictionary()
        {
            if (customAdditionalSounds == null)
            {
                Debug.LogWarning("Nothing has been added to the custom alarm dictionary.");
                return;
            }
            
            foreach (KeyValuePair<AudioClip, int> pair in customAdditionalSounds)
            {
                Debug.Log("index[" +pair.Value + "] is " + pair.Key.name);
            }
        }

        public void ResetCustomSoundFiles()
        {
            if (customAdditionalSounds == null || customAdditionalSounds.Count <= 0)
            {
                // No custom sound files have been registered/found. Reset is redundant.
                // Early exit
                return;
            }

            Debug.Log("Removing custom sounds...");

            // Copy
            Dictionary<AudioClip, int> cachedAdditionalSounds = new(customAdditionalSounds);

            // Remove entry
            foreach (KeyValuePair<AudioClip,int> keyValuePair in cachedAdditionalSounds)
            {
                RemoveCustomDropdownSoundOption(keyValuePair.Key);
            }

            // Clear copy
            cachedAdditionalSounds.Clear();
        }
    }
}
