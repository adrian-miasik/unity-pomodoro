using AdrianMiasik.Interfaces;
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
        
        public void ColorUpdate(Theme theme)
        {
            ColorScheme currentColors = theme.GetCurrentColorScheme();
            title.color = currentColors.foreground;
            description.color = currentColors.foreground;
            socials.ColorUpdate(theme);
            versionNumber.SetTextColor(currentColors.foreground);
            copyrightDisclaimer.color = currentColors.foreground;
        }
    }
}
