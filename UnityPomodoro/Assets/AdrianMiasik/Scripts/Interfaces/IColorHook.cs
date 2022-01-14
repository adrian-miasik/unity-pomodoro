using AdrianMiasik.ScriptableObjects;
using UnityEngine;

namespace AdrianMiasik.Interfaces
{
    /// <summary>
    /// Interface for components/classes that need theme updating when switching between themes / modifying active
    /// themes.
    /// </summary>
    public interface IColorHook
    {
        /// <summary>
        /// This method is invoked when switching between themes / modifying active themes.
        /// </summary>
        public void ColorUpdate(Theme theme);
        
        // ReSharper disable once InconsistentNaming
        GameObject gameObject { get; }
        public void OnDestroy();
    }
}