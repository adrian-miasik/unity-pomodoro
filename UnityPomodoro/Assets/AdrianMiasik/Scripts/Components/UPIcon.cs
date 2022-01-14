using AdrianMiasik.ScriptableObjects;
using Unity.VectorGraphics;
using UnityEngine;

namespace AdrianMiasik.Components
{
    /// <summary>
    /// An SVG image component that flips between two sprites. One for each theme (light / dark).
    /// </summary>
    public class UPIcon : MonoBehaviour
    {
        [SerializeField] private SVGImage m_icon;

        [SerializeField] private Sprite m_lightSprite;
        [SerializeField] private Sprite m_darkSprite;

        /// <summary>
        /// Applies our <see cref="Theme"/> changes to our referenced components when necessary.
        /// </summary>
        /// <param name="theme">The theme to apply on our referenced components.</param>
        public void ColorUpdate(Theme theme)
        {
            m_icon.sprite = theme.m_darkMode ? m_darkSprite : m_lightSprite;
        }
    }
}
