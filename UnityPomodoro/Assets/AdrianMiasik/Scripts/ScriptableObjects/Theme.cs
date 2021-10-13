using System.Collections.Generic;
using AdrianMiasik.Interfaces;
using UnityEngine;

namespace AdrianMiasik.ScriptableObjects
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
            foreach (IColorHook _colorHook in colorElements)
            {
                Debug.Log(_colorHook.ToString(), _colorHook.gameObject);    
            }
        }

        public void RegisterColorHook(IColorHook _hook)
        {
            if (colorElements.Contains(_hook))
            {
                Debug.LogWarning("This interface has already been registered.");
            }
            else
            {
                colorElements.Add(_hook);
            }
        }

        public ColorScheme GetCurrentColorScheme()
        {
            return isLightModeOn ? light : dark;
        }
        
        public void ApplyColorChanges()
        {
            foreach (IColorHook _hook in colorElements)
            {
                _hook.ColorUpdate(this);
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