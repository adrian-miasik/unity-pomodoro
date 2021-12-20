using AdrianMiasik.ScriptableObjects;
using UnityEngine;

namespace AdrianMiasik.Interfaces
{
    public interface IColorHook
    {
        /// <summary>
        /// This method is invoked when switching between themes / modifying active themes
        /// </summary>
        /// <param name="_theme"></param>
        public void ColorUpdate(Theme _theme);
        // ReSharper disable once InconsistentNaming
        GameObject gameObject { get; }
    }
}