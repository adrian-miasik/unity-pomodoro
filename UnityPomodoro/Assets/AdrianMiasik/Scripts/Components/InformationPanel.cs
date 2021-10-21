using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
using TMPro;
using UnityEngine;

namespace AdrianMiasik.Components
{
    public class InformationPanel : MonoBehaviour, IColorHook
    {
        [SerializeField] private TMP_Text title;
        [SerializeField] private TMP_Text description;
        [SerializeField] private SocialButtons socials;
        [SerializeField] private WriteVersionNumber versionNumber;
        [SerializeField] private TMP_Text copyrightDisclaimer;

        private bool isInfoPageOpen;
        
        public void ColorUpdate(Theme _theme)
        {
            // Prevent
            if (!isInfoPageOpen)
            {
                return;
            }

            ColorScheme _currentColors = _theme.GetCurrentColorScheme();
            title.color = _currentColors.foreground;
            description.color = _currentColors.foreground;
            socials.ColorUpdate(_theme);
            versionNumber.SetTextColor(_currentColors.foreground);
            copyrightDisclaimer.color = _currentColors.foreground;
        }

        public bool IsInfoPageOpen()
        {
            return isInfoPageOpen;
        }

        public void Show()
        {
            gameObject.SetActive(true);
            isInfoPageOpen = true;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            isInfoPageOpen = false;
        }
    }
}
