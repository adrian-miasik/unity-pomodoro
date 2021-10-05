using UnityEngine;

namespace AdrianMiasik.Interfaces
{
    public interface IColorHook
    {
        public void ColorUpdate(Theme theme);
        GameObject gameObject { get; }
    }
}