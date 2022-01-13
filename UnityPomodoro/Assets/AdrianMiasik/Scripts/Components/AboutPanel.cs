using AdrianMiasik.Components.Core;
using AdrianMiasik.ScriptableObjects;
using TMPro;
using UnityEngine;

namespace AdrianMiasik.Components
{
    /// <summary>
    /// A <see cref="ThemeElement"/> page used to display information about our application.
    /// Includes a description, social buttons <seealso cref="SocialButtons"/>, version number, and a
    /// copyright disclaimer.
    /// </summary>
    public class AboutPanel : ThemeElement
    {
        [SerializeField] private TMP_Text m_title;
        [SerializeField] private TMP_Text m_description;
        [SerializeField] private SocialButtons m_socials;
        [SerializeField] private WriteVersionNumber m_versionNumber;
        [SerializeField] private TMP_Text m_copyrightDisclaimer;
        
        private bool isInfoPageOpen;

        public override void ColorUpdate(Theme theme)
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
            
            ColorUpdate(Timer.GetTheme());
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            isInfoPageOpen = false;
        }
    }
}