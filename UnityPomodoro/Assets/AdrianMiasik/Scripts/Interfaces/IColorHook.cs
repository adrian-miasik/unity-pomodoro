using UnityEngine;

namespace AdrianMiasik.Interfaces
{
    public interface IColorHook
    {
        public void ColorUpdate(ColorScheme currentColors);
        GameObject gameObject { get; }
    }
}