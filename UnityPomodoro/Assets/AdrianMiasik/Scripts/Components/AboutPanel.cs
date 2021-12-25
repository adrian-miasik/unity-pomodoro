using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
using TMPro;
using UnityEngine;

namespace AdrianMiasik.Components
{
    public class AboutPanel : MonoBehaviour, IColorHook
    {
        [SerializeField] private TMP_Text m_title;
        [SerializeField] private TMP_Text m_description;
        [SerializeField] private SocialButtons m_socials;
        [SerializeField] private WriteVersionNumber m_versionNumber;
        [SerializeField] private TMP_Text m_copyrightDisclaimer;

        private PomodoroTimer timer;
        private bool isInfoPageOpen;
        private bool isInitialized;
        
        public void Initialize(PomodoroTimer pomodoroTimer)
        {
            timer = pomodoroTimer;
            timer.GetTheme().RegisterColorHook(this);
            isInitialized = true;
        }
        
        public bool IsInitialized()
        {
            return isInitialized;
        }

        public void ColorUpdate(Theme theme)
        {
            if (!isInfoPageOpen)
            {
                return;
            }
            
            ColorScheme currentColors = theme.GetCurrentColorScheme();
            m_title.color = currentColors.m_foreground;
            m_description.color = currentColors.m_foreground;
            m_socials.ColorUpdate(theme);
            m_versionNumber.SetTextColor(currentColors.m_foreground);
            m_copyrightDisclaimer.color = currentColors.m_foreground;
        }

        public bool IsInfoPageOpen()
        {
            return isInfoPageOpen;
        }

        public void Show()
        {
            gameObject.SetActive(true);
            isInfoPageOpen = true;
            
            ColorUpdate(timer.GetTheme());
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            isInfoPageOpen = false;
        }
    }
}