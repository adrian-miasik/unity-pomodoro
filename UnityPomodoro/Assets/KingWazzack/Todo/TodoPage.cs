using AdrianMiasik.Components.Base;
using AdrianMiasik.Components.Core.Containers;
using AdrianMiasik.ScriptableObjects;
using TMPro;
using UnityEngine;

namespace KingWazzack.Todo
{
    /// <summary>
    /// A <see cref="Page"/> used to display information about our application.
    /// Includes a description, social buttons (<seealso cref="ThemeIconContainer"/>), version number, and a
    /// copyright disclaimer.
    /// </summary>
    public class TodoPage : Page
    {
        [SerializeField] private TMP_Text m_description;
        
        /// <summary>
        /// Applies our <see cref="Theme"/> changes to our referenced components when necessary.
        /// </summary>
        /// <param name="theme">The theme to apply on our referenced components.</param>
        public override void ColorUpdate(Theme theme)
        {
            base.ColorUpdate(theme);
            ColorScheme currentColors = theme.GetCurrentColorScheme();
            m_description.color = currentColors.m_foreground;
        }
    }
}