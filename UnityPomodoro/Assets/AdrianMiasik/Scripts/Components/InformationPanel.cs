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
        
        public void ColorUpdate(ColorScheme currentColors)
        {
            title.color = currentColors.foreground;
            description.color = currentColors.foreground;
            // TODO: Socials
            versionNumber.SetTextColor(currentColors.foreground);
            copyrightDisclaimer.color = currentColors.foreground;
        }
    }
}
