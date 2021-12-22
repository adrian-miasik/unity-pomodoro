using AdrianMiasik.ScriptableObjects;
using Unity.VectorGraphics;
using UnityEngine;

namespace AdrianMiasik.Components
{
    public class UPIcon : MonoBehaviour
    {
        [SerializeField] private SVGImage m_icon;

        [SerializeField] private Sprite m_lightSprite;
        [SerializeField] private Sprite m_darkSprite;

        public void ColorUpdate(Theme theme)
        {
            m_icon.sprite = theme.m_isLightModeOn ? m_lightSprite : m_darkSprite;
        }
    }
}
