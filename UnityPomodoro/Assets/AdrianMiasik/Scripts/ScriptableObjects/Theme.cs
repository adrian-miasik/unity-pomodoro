using System.Collections.Generic;
using AdrianMiasik.Components.Core;
using AdrianMiasik.Components.Core.Settings;
using AdrianMiasik.Interfaces;
using UnityEngine;

namespace AdrianMiasik.ScriptableObjects
{
    /// <summary>
    /// Responsible for switching and applying color changes using the referenced <see cref="ColorScheme"/>'s.
    /// Two <see cref="ColorScheme"/>'s necessary: One for light mode, and one for dark mode.
    /// </summary>
    [CreateAssetMenu(fileName = "New Theme", menuName = "Adrian Miasik/Create New Theme")]
    public class Theme : ScriptableObject
    {
        public ColorScheme m_light;
        public ColorScheme m_dark;

        // Cache
        [HideInInspector] public SystemSettings m_systemSettings = new SystemSettings();
        private List<IColorHook> colorElements = new List<IColorHook>();

        private void OnValidate()
        {
            ApplyColorChanges();
        }

        private void OnEnable()
        {
            colorElements.Clear();
        }

        /// <summary>
        /// Displays into the console all the color elements (<see cref="IColorHook"/>) gameobjects that have been
        /// registered to this theme.
        /// </summary>
        [ContextMenu("List Interfaces")]
        public void ListInterfaces()
        {
            foreach (IColorHook colorHook in colorElements)
            {
                Debug.Log(colorHook.ToString(), colorHook.gameObject);    
            }
        }

        /// <summary>
        /// Associates the provided (<see cref="IColorHook"/>) color element to this theme. This is used
        /// for updating our elements when we are validating editor colors, or switching between different
        /// <see cref="ColorScheme"/>'s. See <see cref="SetToDarkMode"/> and <see cref="SetToLightMode"/>.
        /// </summary>
        /// <param name="hook">The color element you want to register/associate with this theme.</param>
        public void Register(IColorHook hook)
        {
            if (colorElements.Contains(hook))
            {
                Debug.LogWarning("This interface has already been registered.", hook.gameObject);
            }
            else
            {
                colorElements.Add(hook);
            }
        }

        // Init
        public void Register(IColorHook hook, SystemSettings systemSettings)
        {
            Register(hook);
            m_systemSettings = systemSettings;
        }

        /// <summary>
        /// Disassociates the provided (<see cref="IColorHook"/>) color element from this theme (If they exist).
        /// <remarks>This is usually invoked before gameobject deletion or if you no longer want to update
        /// color elements between <see cref="ColorScheme"/>'s changes.</remarks>
        /// </summary>
        /// <param name="colorHook">The color element you want to deregister/disassociate with this theme.</param>
        public void Deregister(IColorHook colorHook)
        {
            if (colorElements.Contains(colorHook))
            {
                colorElements.Remove(colorHook);
            }
        }

        /// <summary>
        /// Disassociates all our color hook elements from this <see cref="Theme"/>.
        /// </summary>
        public void DeregisterAllElements()
        {
            colorElements.Clear();
        }

        /// <summary>
        /// Fetches the appropriate ColorScheme depending on the user's preference. Depending if they prefer
        /// light / dark mode. 
        /// </summary>
        /// <returns>The user's preferred <see cref="ColorScheme"/></returns>
        public ColorScheme GetCurrentColorScheme()
        {
            return m_systemSettings.m_darkMode ? m_dark : m_light;
        }

        private List<IColorHook> GetColorElements()
        {
            return colorElements;
        }

        private void SetColorElements(List<IColorHook> hooks)
        {
            colorElements = hooks;
        }

        /// <summary>
        /// Transfers all our (<see cref="IColorHook"/>) color elements from one theme to another.
        /// Including theme related settings.
        /// </summary>
        /// <param name="sourceTheme">The theme you want to pull color elements from.</param>
        /// <param name="destinationTheme">The theme you want to transfer your color elements to.</param>
        public void TransferColorElements(Theme sourceTheme, Theme destinationTheme)
        {
            destinationTheme.SetColorElements(sourceTheme.GetColorElements());
            destinationTheme.m_systemSettings.m_darkMode = sourceTheme.m_systemSettings.m_darkMode;
        }
        
        /// <summary>
        /// Updates (and invokes ColorUpdate to) all our registered (<see cref="IColorHook"/>) color elements.
        /// </summary>
        public void ApplyColorChanges()
        {
            foreach (IColorHook hook in colorElements)
            {
                hook.ColorUpdate(this);
            }
        }

        /// <summary>
        /// Set's the current ColorScheme to the dark variation and updates all registered elements.
        /// </summary>
        public void SetToDarkMode(bool save = true)
        {
            m_systemSettings.m_darkMode = true;

            if (save)
            {
                UserSettingsSerializer.SaveSystemSettings(m_systemSettings);
            }

            ApplyColorChanges();
        }

        /// <summary>
        /// Set's the current ColorScheme to the light variation and updates all registered elements.
        /// </summary>
        public void SetToLightMode(bool save = true)
        {
            m_systemSettings.m_darkMode = false;

            if (save)
            {
                UserSettingsSerializer.SaveSystemSettings(m_systemSettings);
            }

            ApplyColorChanges();
        }
    }
}