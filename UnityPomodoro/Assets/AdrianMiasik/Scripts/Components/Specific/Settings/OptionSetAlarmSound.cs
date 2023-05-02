using System;
using System.Collections.Generic;
using System.IO;
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
        /// <summary>
        /// A list of alarm sounds that come included with the project build.
        /// </summary>
        [SerializeField] private List<AudioClip> m_preInstalledAlarms;

        /// <summary>
        /// A dictionary of all our alarm sounds (preinstalled + custom).
        /// To add a custom alarm audio component, See <see cref="AddCustomDropdownSoundOption"/>().
        /// </summary>
        [SerializeField] private Dictionary<int, AudioClip> alarmBank = new();

        /// <summary>
        /// Initializes this SettingsOptionDropdown, adds our preInstalledAlarms to the alarm bank dictionary,
        /// verifies our saved index/key is valid, and add onValueChanged listener to dropdown.
        /// </summary>
        /// <param name="pomodoroTimer"></param>
        /// <param name="updateColors"></param>
        public override void Initialize(PomodoroTimer pomodoroTimer, bool updateColors = true)
        {
            base.Initialize(pomodoroTimer, updateColors);

            // Move pre-installed alarms to alarm bank...
            for (int i = 0; i < m_preInstalledAlarms.Count; i++)
            {
                AudioClip audioClip = m_preInstalledAlarms[i];
                alarmBank.TryAdd(i, audioClip);
            }

            // ValidateAlarmSoundIndex();

            // Invoke SetAlarmSound anytime the dropdown value changes
            m_dropdown.onValueChanged.AddListener((i) =>
            {
                SetAlarmSound(i, true);
            });
        }

        public void ValidateAlarmSoundIndex()
        {
            // Get saved key
            int startingIndex = Timer.GetTimerSettings().m_alarmSoundIndex;

            // Verify if index exists in dictionary...
            if (!alarmBank.TryGetValue(startingIndex, out AudioClip audioClip))
            {
                Debug.LogWarning("Preferred alarm sound key [" + startingIndex + "] not found. " +
                                 "Reverting back to default alarm. [0]");
                startingIndex = 0;
            }

            // Set alarm sound (directly) based on current setting
            if (alarmBank.TryGetValue(startingIndex, out AudioClip c))
            {
                m_dropdown.value = startingIndex;
                SetAlarmSound(startingIndex);
            }
        }

        private void SetAlarmSound(Int32 i, bool playPreviewSound = false)
        {
            // Apply change to settings
            Timer.GetTimerSettings().m_alarmSoundIndex = i;
            UserSettingsSerializer.SaveSettingsFile(Timer.GetTimerSettings(), "timer-settings");

            // Apply change to timer
            if (alarmBank.TryGetValue(i, out AudioClip c))
            {
                Timer.SetAlarmSound(c, playPreviewSound);
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

        public void RemoveCustomDropdownSoundOption(string audioFile)
        {
            // Copy
            Dictionary<int, AudioClip> cachedAdditionalSounds = new(alarmBank);

            // Iterate through each sound in our bank...
            foreach (KeyValuePair<int,AudioClip> keyValuePair in cachedAdditionalSounds)
            {
                // If the option name matches our file name...
                if (keyValuePair.Value.name == Path.GetFileName(audioFile))
                {
                    // Remove element from original bank (not the bank we are currently iterating)
                    alarmBank.Remove(keyValuePair.Key);

                    // Iterate through all the custom audio options...
                    for (int i = m_preInstalledAlarms.Count; i < m_dropdown.options.Count; i++)
                    {
                        // If the removed alarm from the bank matches the dropdown option...
                        if (m_dropdown.options[i].text == Path.GetFileName(audioFile))
                        {
                            // Remove drop down option
                            m_dropdown.options.RemoveAt(i);
                            // m_dropdown.options[i].text = "-";

                            // Something got removed from our dictionary, we will attempt to reshuffle
                            // the keys to be in ascending index order. This way our dropdown settings can match the
                            // dictionary keys.
                            CascadeAlarmBankDictionary();
                        }
                    }
                }
            }

            // Clear copy
            cachedAdditionalSounds.Clear();
        }

        private void CascadeAlarmBankDictionary()
        {
            Debug.Log("Cascading Dictionary!");

            // Cache current alarm
            alarmBank.TryGetValue(Timer.GetTimerSettings().m_alarmSoundIndex, out AudioClip currentAlarm);

            // Create
            Dictionary<int, AudioClip> cascadedAlarmBank = new Dictionary<int, AudioClip>();

            // Move pre-installed alarms to cascaded alarm bank...
            for (int i = 0; i < m_preInstalledAlarms.Count; i++)
            {
                AudioClip audioClip = m_preInstalledAlarms[i];
                cascadedAlarmBank.TryAdd(i, audioClip);
            }

            // Iterate through only our custom alarms...
            for (int i = m_preInstalledAlarms.Count; i < alarmBank.Count; i++)
            {
                KeyValuePair<int, AudioClip> element = alarmBank.ElementAt(i);

                // Add them to bank but to first available index, not the element's preferred key.
                cascadedAlarmBank.Add(i, element.Value);

                // If the moved/cascaded element matches the players current preference.
                if (element.Value == currentAlarm)
                {
                    // Set the dropdown value to match their preference
                    m_dropdown.value = i;

                    // Apply/save preference
                    Timer.GetTimerSettings().m_alarmSoundIndex = i;
                    UserSettingsSerializer.SaveSettingsFile(Timer.GetTimerSettings(), "timer-settings");
                }
            }

            // Apply cascade
            alarmBank = cascadedAlarmBank;
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
    }
}
