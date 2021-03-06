using AdrianMiasik.Components.Base;
using AdrianMiasik.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace AdrianMiasik.Components.Specific
{
    /// <summary>
    /// A <see cref="ThemeElement"/> image used to pull focus off selected elements.
    /// The background is our default selection for Unity's EventSystem.
    /// </summary>
    public class Background : ThemeElement
    {
        [SerializeField] private Image m_background;
        [SerializeField] private Button m_button;
        [SerializeField] private Selectable m_selectable;

        public override void Initialize(PomodoroTimer pomodoroTimer, bool updateColors = true)
        {
            base.Initialize(pomodoroTimer, updateColors);
            
            m_button.onClick.AddListener(Timer.ClearSelection);
        }

        /// <summary>
        /// Sets the <see cref="UnityEngine.EventSystems"/> current selection to this background.
        /// </summary>
        public void Select()
        {
            m_button.Select();
        }

        /// <summary>
        /// Applies our <see cref="Theme"/> changes to our referenced components when necessary.
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