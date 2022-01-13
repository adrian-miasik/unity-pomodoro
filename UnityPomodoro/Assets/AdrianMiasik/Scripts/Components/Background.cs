using AdrianMiasik.Components.Core;
using AdrianMiasik.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace AdrianMiasik.Components
{
    /// <summary>
    /// A <see cref="ThemeElement"/> <see cref="Image"/> used to pull focus off selected elements.
    /// The background is our default selection for the Unity <see cref="UnityEngine.EventSystems"/>.
    /// </summary>
    public class Background : ThemeElement
    {
        [SerializeField] private Image m_background;
        [SerializeField] private Selectable m_selectable;
        
        /// <summary>
        /// Sets the <see cref="UnityEngine.EventSystems"/> current selection to this background.
        /// </summary>
        public void Select()
        {
            m_selectable.Select();
        }

        /// <summary>
        /// Applies our theme changes to our referenced components when necessary.
        /// </summary>
        /// <param name="theme">The theme to apply on our referenced components.</param>
        public override void ColorUpdate(Theme theme)
        {
            m_background.color = theme.GetCurrentColorScheme().m_background;
        }

        /// <summary>
        /// Sets our background's selection navigation to the provided <see cref="Navigation"/>.
        /// <remarks>Intended to change focus to our digits when attempting to select left / right
        /// from the background.</remarks>
        /// </summary>
        /// <param name="backgroundNav"></param>
        public void SetSelectionNavigation(Navigation backgroundNav)
        {
            m_selectable.navigation = backgroundNav;
        }
    }
}
