using AdrianMiasik.Components.Base;
using Unity.VectorGraphics;

namespace AdrianMiasik.Components.Core
{
    /// <summary>
    /// A <see cref="ClickButton"/> with a SVG image.
    /// </summary>
    public class ClickButtonSVGIcon: ClickButton
    {
        public SVGImage m_icon;

        public override void Show()
        {
            base.Show();
            m_icon.gameObject.SetActive(true);
        }

        public override void Hide()
        {
            base.Hide();
            m_icon.gameObject.SetActive(false);
        }
    }
}