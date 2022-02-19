using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
using UnityEngine;

namespace AdrianMiasik.Components.Core
{
    public class ThemeManager : MonoBehaviour
    {
        [SerializeField] private Theme m_activeTheme;
        
        /// <summary>
        /// Returns our current active <see cref="Theme"/>
        /// </summary>
        /// <returns></returns>
        public Theme GetTheme()
        {
            return m_activeTheme;
        }

        /// <summary>
        /// Sets our <see cref="Theme"/> preference to light mode, and update's all our necessary components.
        /// <remarks>Used as a UnityEvent on our <see cref="m_themeSlider"/>.</remarks>
        /// </summary>
        public void SetToLightMode()
        {
            m_activeTheme.SetToLightMode();
        }

        /// <summary>
        /// Sets our <see cref="Theme"/> preference to dark mode, and update's all our necessary components.
        /// <remarks>Used as a UnityEvent on our <see cref="m_themeSlider"/>.</remarks>
        /// </summary>
        public void SetToDarkMode()
        {
            m_activeTheme.SetToDarkMode();
        }

        /// <summary>
        /// Sets our current active <see cref="Theme"/> to the provided <see cref="Theme"/>. This will transfer
        /// all our <see cref="IColorHook"/> element's to the new <see cref="Theme"/> as well.
        /// </summary>
        /// <param name="desiredTheme"></param>
        public void SwitchTheme(Theme desiredTheme)
        {
            // Transfer elements to new theme (So theme knows which elements to color update)
            m_activeTheme.TransferColorElements(m_activeTheme, desiredTheme);
            
            // Swap our theme
            m_activeTheme = desiredTheme;
            
            // Apply our changes
            m_activeTheme.ApplyColorChanges();
        }

        // Piper methods
        public void Register(PomodoroTimer pomodoroTimer)
        {
            m_activeTheme.Register(pomodoroTimer);
        }

        public void ApplyColorChanges()
        {
            m_activeTheme.ApplyColorChanges();
        }
    }
}