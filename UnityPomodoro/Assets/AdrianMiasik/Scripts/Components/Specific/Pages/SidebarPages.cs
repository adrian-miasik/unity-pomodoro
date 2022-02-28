using System.Collections.Generic;
using AdrianMiasik.Components.Base;
using UnityEngine;

namespace AdrianMiasik.Components.Specific.Pages
{
    public class SidebarPages : ItemSelector<Page>
    {
        [SerializeField] private TimerPage m_timerPage;
        [SerializeField] private SettingsPage m_settingsPage;
        [SerializeField] private AboutPage m_aboutPage;
        
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

            Initialize(Timer, pages, updateColors);
            onSelectionChange += OnSelectionChange;
        }

        private new void OnSelectionChange(Page previousPage, Page newPage)
        {
            previousPage.Hide();
            newPage.Show();
        }

        public void SwitchToTimerPage()
        {
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
    }
}