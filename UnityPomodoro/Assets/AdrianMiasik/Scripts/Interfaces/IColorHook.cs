using AdrianMiasik.ScriptableObjects;
using UnityEngine;

namespace AdrianMiasik.Interfaces
{
    public interface IColorHook
    {
        public void ColorUpdate(Theme _theme);
        // ReSharper disable once InconsistentNaming
        GameObject gameObject { get; }
    }
}