using AdrianMiasik.Components.Base;
using AdrianMiasik.ScriptableObjects;
using Unity.VectorGraphics;
using UnityEngine;

namespace AdrianMiasik.Components.Core
{
    /// <summary>
    /// An SVG image component that switches between two sprites depending on the <see cref="Theme"/> preference:
    /// Light / Dark.
    /// </summary>
    public class ThemeIcon : ThemeElement
    {
        [SerializeField] private SVGImage m_icon;

        [SerializeField] private Sprite m_lightSprite;
        [SerializeField] private Sprite m_darkSprite;

        /// <summary>
        /// Applies our <see cref="Theme"/> changes to our referenced components when necessary.
        /// </summary>
        /// <param name="theme">The theme to apply on our referenced components.</param>
        public override void ColorUpdate(Theme theme)
        {
            m_icon.sprite = Timer.GetSystemSettings().m_darkMode ? m_darkSprite : m_lightSprite;
        }

        /// <summary>
        /// Swaps the SVG image icon to use the provided sprite.
        /// </summary>
        /// <param name="sprite"></param>
        public void ChangeSprite(Sprite sprite)
        {
            m_icon.sprite = sprite;
        }

        /// <summary>
        /// Sets the SVG image icon to the provided color.
        /// </summary>
        /// <param name="color"></param>
        public void ChangeColor(Color color)
        {
            m_icon.color = color;
        }
    }
}