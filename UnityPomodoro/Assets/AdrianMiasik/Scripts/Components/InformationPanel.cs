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
        
        public void ColorUpdate(Theme _theme)
        {
            ColorScheme _currentColors = _theme.GetCurrentColorScheme();
            title.color = _currentColors.foreground;
            description.color = _currentColors.foreground;
            socials.ColorUpdate(_theme);
            versionNumber.SetTextColor(_currentColors.foreground);
            copyrightDisclaimer.color = _currentColors.foreground;
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
