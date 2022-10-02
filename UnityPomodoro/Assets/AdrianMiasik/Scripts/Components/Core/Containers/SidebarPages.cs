using System.Collections.Generic;
using AdrianMiasik.Components.Base;
using AdrianMiasik.Components.Core.Items.Pages;
using UnityEngine;
using UnityEngine.Events;

namespace AdrianMiasik.Components.Core.Containers
{
    /// <summary>
    /// A <see cref="ItemSelector{T}"/> that hold's references to all our application <see cref="Page"/>'s.
    /// Intended to control which page is currently selected/opened/seen.
    /// </summary>
    public class SidebarPages : ItemSelector<Page>
    {
        [SerializeField] private TimerPage m_timerPage;
        [SerializeField] private SettingsPage m_settingsPage;
        [SerializeField] private AboutPage m_aboutPage;

        private readonly UnityEvent onTimerPageShow = new();
        
        public override void Initialize(PomodoroTimer pomodoroTimer, bool updateColors = true)
        {
            base.Initialize(pomodoroTimer, updateColors);
            
            m_timerPage.Initialize(pomodoroTimer);
            m_settingsPage.Initialize(pomodoroTimer);
            m_aboutPage.Initialize(pomodoroTimer);

            List<Page> pages = new List<Page>
            {
                m_timerPage,
                m_settingsPage,
                m_aboutPage
            };

            Initialize(pages);
            onSelectionChange += OnSelectionChange;
        }

        private new void OnSelectionChange(Page previousPage, Page newPage)
        {
            // if (previousPage == newPage)
            // {
            //     return;
            // }
            previousPage.Hide(() =>
            {
                newPage.Show(null);
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
        
        public void SwitchToSettingsPage()
        {
            Select(m_settingsPage);
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