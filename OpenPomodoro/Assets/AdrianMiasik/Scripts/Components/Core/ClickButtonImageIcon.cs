using AdrianMiasik.Components.Base;
using UnityEngine.UI;

namespace AdrianMiasik.Components.Core
{
    /// <summary>
    /// A <see cref="ClickButton"/> with an image.
    /// </summary>
    public class ClickButtonImageIcon: ClickButton
    {
        public Image m_icon;

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