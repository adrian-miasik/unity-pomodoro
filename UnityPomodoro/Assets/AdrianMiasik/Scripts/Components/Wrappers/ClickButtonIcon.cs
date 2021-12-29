using AdrianMiasik.Components.Core;
using Unity.VectorGraphics;

namespace AdrianMiasik.Components.Wrappers
{
    public class ClickButtonIcon: ClickButton
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