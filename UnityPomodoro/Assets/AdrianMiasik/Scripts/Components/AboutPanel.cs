using AdrianMiasik.Components.Core;
using AdrianMiasik.ScriptableObjects;
using TMPro;
using UnityEngine;

namespace AdrianMiasik.Components
{
    /// <summary>
    /// A <see cref="ThemeElement"/> page used to display information about our application.
    /// Includes a description, social buttons (<seealso cref="SocialButtons"/>), version number, and a
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

        /// <summary>
        /// Applies our <see cref="Theme"/> changes to our referenced components when necessary.
        /// </summary>
        /// <param name="theme">The theme to apply on our referenced components.</param>
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

        /// <summary>
        /// Is this <see cref="AboutPanel"/> currently open and visible?
        /// </summary>
        /// <returns></returns>
        public bool IsInfoPageOpen()
        {
            return isInfoPageOpen;
        }

        /// <summary>
        /// Displays this panel to the user.
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
            isInfoPageOpen = true;
            
            ColorUpdate(Timer.GetTheme());
        }

        /// <summary>
        /// Hides this panel from the user.
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
            isInfoPageOpen = false;
        }
    }
}