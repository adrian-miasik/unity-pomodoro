using System;
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

        private List<IColorHook> colorElements = new List<IColorHook>();

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

        public void Deregister(IColorHook _colorHook)
        {
            if (colorElements.Contains(_colorHook))
            {
                colorElements.Remove(_colorHook);
            }
        }

        public ColorScheme GetCurrentColorScheme()
        {
            return isLightModeOn ? light : dark;
        }

        private List<IColorHook> GetColorElements()
        {
            return colorElements;
        }

        private void SetColorElements(List<IColorHook> _colorElements)
        {
            colorElements = _colorElements;
        }

        /// <summary>
        /// Transfers color elements from one theme to another
        /// </summary>
        /// <param name="_sourceTheme">The theme you want to pull color elements from</param>
        /// <param name="_destinationTheme">The theme you want to transfer your color elements to</param>
        public void TransferColorElements(Theme _sourceTheme, Theme _destinationTheme)
        {
            _destinationTheme.SetColorElements(_sourceTheme.GetColorElements());
            _destinationTheme.isLightModeOn = _sourceTheme.isLightModeOn;
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