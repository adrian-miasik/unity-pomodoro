using AdrianMiasik.Components.Base;
using Unity.VectorGraphics;

namespace AdrianMiasik.Components.Core
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