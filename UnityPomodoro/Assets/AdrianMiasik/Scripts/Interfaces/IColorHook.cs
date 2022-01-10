using AdrianMiasik.ScriptableObjects;
using UnityEngine;

namespace AdrianMiasik.Interfaces
{
    public interface IColorHook
    {
        /// <summary>
        /// This method is invoked when switching between themes / modifying active themes
        /// </summary>
        /// <param name="theme"></param>
        public void ColorUpdate(Theme theme);
        // ReSharper disable once InconsistentNaming
        GameObject gameObject { get; }
        public void OnDestroy();
    }
}