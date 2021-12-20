using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
using TMPro;
using UnityEngine;

namespace AdrianMiasik.Components
{
    public class AboutPanel : MonoBehaviour, IColorHook
    {
        [SerializeField] private TMP_Text title;
        [SerializeField] private TMP_Text description;
        [SerializeField] private SocialButtons socials;
        [SerializeField] private WriteVersionNumber versionNumber;
        [SerializeField] private TMP_Text copyrightDisclaimer;

        private PomodoroTimer timer;
        private bool isInfoPageOpen;
        private bool isInitialized;
        
        public void Initialize(PomodoroTimer _timer)
        {
            timer = _timer;
            timer.GetTheme().RegisterColorHook(this);
            isInitialized = true;
        }
        
        public bool IsInitialized()
        {
            return isInitialized;
        }

        public void ColorUpdate(Theme _theme)
        {
            if (!isInfoPageOpen)
            {
                return;
            }
            
            ColorScheme _currentColors = _theme.GetCurrentColorScheme();
            title.color = _currentColors.m_foreground;
            description.color = _currentColors.m_foreground;
            socials.ColorUpdate(_theme);
            versionNumber.SetTextColor(_currentColors.m_foreground);
            copyrightDisclaimer.color = _currentColors.m_foreground;
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