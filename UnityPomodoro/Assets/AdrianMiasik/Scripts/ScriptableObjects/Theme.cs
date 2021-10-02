using System;
using System.Collections.Generic;
using AdrianMiasik.Interfaces;
using UnityEngine;

namespace AdrianMiasik
{
    [CreateAssetMenu(fileName = "New Theme", menuName = "Adrian Miasik/Create New Theme")]
    public class Theme : ScriptableObject
    {
        public bool isLightModeOn = true;
        public ColorScheme light;
        public ColorScheme dark;

        private readonly List<IColorHook> colorElements = new List<IColorHook>();

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

        public ColorScheme GetCurrentColorScheme()
        {
            return isLightModeOn ? light : dark;
        }
        
        public void ApplyColorChanges()
        {
            foreach (IColorHook hook in colorElements)
            {
                hook.ColorUpdate(GetCurrentColorScheme());
            }
        }

        public void SetToDarkMode()
        {
            isLightModeOn = false;
            ApplyColorChanges();
        }

        public void SetToLightMode()
        {
            isLightModeOn = true;
            ApplyColorChanges();
        }
    }
}