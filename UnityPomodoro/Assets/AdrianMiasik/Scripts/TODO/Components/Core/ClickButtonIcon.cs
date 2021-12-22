using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.Serialization;

namespace AdrianMiasik.Components.Core
{
    public class ClickButtonIcon: ClickButton
    {
        [FormerlySerializedAs("icon")] public SVGImage m_icon;

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