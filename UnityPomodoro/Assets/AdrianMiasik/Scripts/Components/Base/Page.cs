using AdrianMiasik.Components.Specific.Pages;
using AdrianMiasik.ScriptableObjects;
using TMPro;
using UnityEngine;

namespace AdrianMiasik.Components.Base
{
    /// <summary>
    /// A <see cref="ThemeElement"/> page that displays/hides it's associated content.
    /// <remarks>
    /// Intended to be used with <see cref="ItemSelector{T}"/>, also see <seealso cref="SidebarPages"/>.
    /// </remarks>
    /// </summary>
    public class Page : ThemeElement
    {
        [SerializeField] private TMP_Text m_title;

        private bool isOpen;
        
        public override void ColorUpdate(Theme theme)
        {
            // Skip the color update if this page isn't open.
            if (!isOpen)
            {
                return;
            }

            m_title.color = theme.GetCurrentColorScheme().m_foreground;
        }

        public virtual void Refresh()
        {
            if (!IsPageOpen())
            {
                return;
            }
            ColorUpdate(Timer.GetTheme());
        }
        
        /// <summary>
        /// Displays this page to the user.
        /// </summary>
        public virtual void Show()
        {
            gameObject.SetActive(true);
            isOpen = true;
            
            ColorUpdate(Timer.GetTheme());
        }

        /// <summary>
        /// Hides this page away from the user.
        /// </summary>
        public virtual void Hide()
        {
            gameObject.SetActive(false);
            isOpen = false;
        }
        
        /// <summary>
        /// Is this page currently open and visible?
        /// </summary>
        /// <returns></returns>
        public bool IsPageOpen()
        {
            return isOpen;
        }
    }
}