using AdrianMiasik.Components.Base;
using AdrianMiasik.Components.Core.Containers;
using AdrianMiasik.ScriptableObjects;
using TMPro;
using UnityEngine;

namespace AdrianMiasik.Components.Core.Items.Pages
{
    /// <summary>
    /// A <see cref="Page"/> used to display information about our application.
    /// Includes a description, social buttons (<seealso cref="ThemeIconContainer"/>), version number, and a
    /// copyright disclaimer.
    /// </summary>
    public class AboutPage : Page
    {
        [SerializeField] private TMP_Text m_description;
        [SerializeField] private ThemeIconContainer m_socials;
        [SerializeField] private WriteVersionNumber m_versionNumber;
        [SerializeField] private TMP_Text m_copyrightDisclaimer;
        
        public override void Initialize(PomodoroTimer pomodoroTimer, bool updateColors = true)
        {
            m_socials.Initialize(pomodoroTimer, updateColors);
            m_versionNumber.Initialize();
            base.Initialize(pomodoroTimer, updateColors);
        }

        /// <summary>
        /// Applies our <see cref="Theme"/> changes to our referenced components when necessary.
        /// </summary>
        /// <param name="theme">The theme to apply on our referenced components.</param>
        public override void ColorUpdate(Theme theme)
        {
            base.ColorUpdate(theme);
            
            ColorScheme currentColors = theme.GetCurrentColorScheme();
            m_description.color = currentColors.m_foreground;
            m_versionNumber.SetTextColor(currentColors.m_foreground);
            m_copyrightDisclaimer.color = currentColors.m_foreground;
        }
    }
}