using System;
using System.Collections.Generic;
using AdrianMiasik.Interfaces;
using UnityEngine;
using UnityEngine.Serialization;

namespace AdrianMiasik.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Theme", menuName = "Adrian Miasik/Create New Theme")]
    public class Theme : ScriptableObject
    {
        [FormerlySerializedAs("m_isLightModeOn")] public bool m_darkMode = true;
        public ColorScheme m_light;
        public ColorScheme m_dark;

        private List<IColorHook> colorElements = new List<IColorHook>();

        private void OnValidate()
        {
            ApplyColorChanges();
        }

        private void OnEnable()
        {
            colorElements.Clear();
        }

        [ContextMenu("List Interfaces")]
        public void ListInterfaces()
        {
            foreach (IColorHook colorHook in colorElements)
            {
                Debug.Log(colorHook.ToString(), colorHook.gameObject);    
            }
        }

        public void RegisterColorHook(IColorHook hook)
        {
            if (colorElements.Contains(hook))
            {
                Debug.LogWarning("This interface has already been registered.");
            }
            else
            {
                colorElements.Add(hook);
            }
        }

        public void Deregister(IColorHook colorHook)
        {
            if (colorElements.Contains(colorHook))
            {
                colorElements.Remove(colorHook);
            }
        }

        public ColorScheme GetCurrentColorScheme()
        {
            return m_darkMode ? m_dark : m_light;
        }

        private List<IColorHook> GetColorElements()
        {
            return colorElements;
        }

        private void SetColorElements(List<IColorHook> hooks)
        {
            this.colorElements = hooks;
        }

        /// <summary>
        /// Transfers color elements from one theme to another
        /// </summary>
        /// <param name="sourceTheme">The theme you want to pull color elements from</param>
        /// <param name="destinationTheme">The theme you want to transfer your color elements to</param>
        public void TransferColorElements(Theme sourceTheme, Theme destinationTheme)
        {
            destinationTheme.SetColorElements(sourceTheme.GetColorElements());
            destinationTheme.m_darkMode = sourceTheme.m_darkMode;
        }
        
        public void ApplyColorChanges()
        {
            foreach (IColorHook hook in colorElements)
            {
                hook.ColorUpdate(this);
            }
        }

        public void SetToDarkMode()
        {
            m_darkMode = true;
            ApplyColorChanges();
        }

        public void SetToLightMode()
        {
            m_darkMode = false;
            ApplyColorChanges();
        }
    }
}