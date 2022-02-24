using AdrianMiasik.Components.Base;
using AdrianMiasik.Components.Core.Settings;
using AdrianMiasik.Components.Specific;
using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
using UnityEngine;

namespace AdrianMiasik.Components.Core
{
    /// <summary>
    /// Holds our current active theme and is responsible for changing between themes.
    /// </summary>
    public class ThemeManager : MonoBehaviour
    {
        [SerializeField] private Theme m_defaultTheme;
        [SerializeField] private Theme m_halloweenTheme;
        
        private Theme activeTheme;

        /// <summary>
        /// Returns our current active <see cref="Theme"/>
        /// </summary>
        /// <returns></returns>
        public Theme GetTheme()
        {
            return activeTheme;
        }
        
        public void Register(PomodoroTimer pomodoroTimer)
        {
            activeTheme = m_defaultTheme;
            activeTheme.Register(pomodoroTimer, pomodoroTimer);
        }

        /// <summary>
        /// Sets our <see cref="Theme"/> preference to light mode, and update's all our necessary components.
        /// <remarks>Used as a UnityEvent on our <see cref="ThemeSlider"/>.</remarks>
        /// </summary>
        public void SetToLightMode()
        {
            activeTheme.SetToLightMode();
        }

        /// <summary>
        /// Sets our <see cref="Theme"/> preference to dark mode, and update's all our necessary components.
        /// <remarks>Used as a UnityEvent on our <see cref="ThemeSlider"/>.</remarks>
        /// </summary>
        public void SetToDarkMode()
        {
            activeTheme.SetToDarkMode();
        }

        /// <summary>
        /// Sets our current active <see cref="Theme"/> to the provided <see cref="Theme"/>. This will transfer
        /// all our <see cref="IColorHook"/> element's to the new <see cref="Theme"/> as well.
        /// </summary>
        /// <param name="desiredTheme"></param>
        public void SwitchTheme(Theme desiredTheme)
        {
            // Transfer elements to new theme (So theme knows which elements to color update)
            activeTheme.TransferColorElements(activeTheme, desiredTheme);
            
            // Swap our theme
            activeTheme = desiredTheme;
            
            // Apply our changes
            activeTheme.ApplyColorChanges();
        }

        public void SwitchToHalloweenTheme()
        {
            SwitchTheme(m_halloweenTheme);
        }

        public void SwitchToDefaultTheme()
        {
            SwitchTheme(m_defaultTheme);
        }
    }
}