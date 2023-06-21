using System.Collections;
using System.Collections.Generic;
using System.IO;
using AdrianMiasik.Components.Base;
using AdrianMiasik.Components.Core.Items.Pages;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace AdrianMiasik.Components.Core.Containers
{
    /// <summary>
    /// A <see cref="ItemSelector{T}"/> that hold's references to all our application <see cref="Page"/>'s.
    /// Intended to control which page is currently selected/opened/seen.
    /// </summary>
    public class SidebarPages : ItemSelector<Page>
    {
        [SerializeField] private TimerPage m_timerPage;
        [SerializeField] private TodoPage m_todoPage;
        [SerializeField] private CustomizationPage m_customizationPage;
        [SerializeField] private SettingsPage m_settingsPage;
        [SerializeField] private AboutPage m_aboutPage;

        private readonly UnityEvent onTimerPageShow = new();
        
        public override void Initialize(PomodoroTimer pomodoroTimer, bool updateColors = true)
        {
            base.Initialize(pomodoroTimer, updateColors);
            
            m_timerPage.Initialize(pomodoroTimer);
            m_todoPage.Initialize(pomodoroTimer);
            m_settingsPage.Initialize(pomodoroTimer);
            m_aboutPage.Initialize(pomodoroTimer);
            m_customizationPage.Initialize(pomodoroTimer);

            List<Page> pages = new List<Page>
            {
                m_timerPage,
                m_todoPage,
                m_customizationPage,
                m_settingsPage,
                m_aboutPage
            };

            Initialize(pages);
            onSelectionChange += OnSelectionChange;
        }

        private new void OnSelectionChange(Page previousPage, Page newPage)
        {
            previousPage.Hide(() =>
            {
                newPage.Show(() =>
                {
                    Timer.ColorUpdateEndTimestampGhost();
                });
                
                if (newPage == m_timerPage)
                {
                    onTimerPageShow?.Invoke();
                    onTimerPageShow.RemoveAllListeners();
                }
            });
        }

        public void SwitchToTimerPage(UnityAction onShow = null)
        {
            if (onShow != null)
            {
                onTimerPageShow.AddListener(onShow);
            }

            Select(m_timerPage);
        }

        public IEnumerator AddCustomSoundFiles(List<string> customAudioFiles)
        {
            foreach (string audioFile in customAudioFiles)
            {
                yield return StartCoroutine(AddCustomAudioFile(audioFile));
            }
        }

        IEnumerator AddCustomAudioFile(string audioFile)
        {
            // Fetch audio file type based on audio file extension
            string audioFileExtension = Path.GetExtension(audioFile);
            AudioType type;
            switch (audioFileExtension)
            {
                case ".wav":
                    type = AudioType.WAV;
                    break;
                case ".mp3":
                    type = AudioType.MPEG;
                    break;
                default:
                    Debug.LogWarning("Unsupported audio file extension.");
                    type = AudioType.UNKNOWN;
                    break;
            }

            // Define request
            using UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + audioFile, type);

            // Submit and await request
            yield return www.SendWebRequest();

            // On request return...
            // If the request failed...
            if (www.result != UnityWebRequest.Result.Success)
            {
                // Log the error in question
                Debug.Log(www.error);
            }
            // Otherwise...
            else
            {
                // Create and load AudioClip
                AudioClip audioClip = DownloadHandlerAudioClip.GetContent(www);
                audioClip.name = Path.GetFileName(audioFile);

                // Cache custom alarm to sound bank + add dropdown option
                m_settingsPage.AddCustomDropdownSoundOption(audioClip, Path.GetFileName(audioClip.name));
            }
        }

        public void RemoveCustomAudioFile(string audioFile)
        {
            m_settingsPage.RemoveCustomDropdownSoundOption(audioFile);
        }

        public void ValidateCustomSoundChoice()
        {
            m_settingsPage.ValidateCustomSoundChoice();
        }

        public void SwitchToSettingsPage()
        {
            Select(m_settingsPage);
        }

        public void SwitchToTodoPage()
        {
            Select(m_todoPage);
        }
        public void SwitchToCustomizationPage()
        {
            Select(m_customizationPage);
        }

        public void SwitchToAboutPage()
        {
            Select(m_aboutPage);
        }

        public void RefreshTimerPage()
        {
            m_timerPage.Refresh();
        }

        public void RefreshSettingsPage()
        {
            m_settingsPage.Refresh();
        }

        public bool IsAboutPageOpen()
        {
            return m_aboutPage.IsPageOpen();
        }

        public bool IsTodoPageOpen()
        {
            return m_todoPage.IsPageOpen();
        }
        public bool IsCustomizationPageOpen()
        {
            return m_customizationPage.IsPageOpen();
        }

        public bool IsTimerPageOpen()
        {
            return m_timerPage.gameObject.activeSelf;
        }

        public bool IsSettingsPageOpen()
        {
            return m_settingsPage.IsPageOpen();
        }

        public void UpdateSettingsDigitFormatDropdown()
        {
            m_settingsPage.UpdateDigitFormatDropdown();
        }

        public void SetSettingDigitFormatDropdown(int digitFormatIndex)
        {
            m_settingsPage.SetDigitFormatDropdown(digitFormatIndex);
        }

        public void MCSwitchToMainPageInstant()
        {
            m_aboutPage.gameObject.SetActive(false);
            m_settingsPage.gameObject.SetActive(false);
            
            m_timerPage.gameObject.SetActive(true);
        }

        public void MCSwitchToAboutPageInstant()
        {
            m_timerPage.gameObject.SetActive(false);
            m_settingsPage.gameObject.SetActive(false);
            
            m_aboutPage.gameObject.SetActive(true);
            m_aboutPage.ColorUpdate(Timer.GetTheme());
        }

        public void MCSwitchToSettingsPageInstant()
        {
            m_timerPage.gameObject.SetActive(false);
            m_aboutPage.gameObject.SetActive(false);
            
            m_settingsPage.gameObject.SetActive(true);
            m_settingsPage.ColorUpdate(Timer.GetTheme());
        }
    }
}