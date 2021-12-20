using Unity.VectorGraphics;
using UnityEngine;

namespace AdrianMiasik.Components.Core
{
    public class ClickButtonIcon: ClickButton
    {
        public SVGImage icon;

        public SVGImage GetIcon()
        {
            return icon;
        }
        
        public override void Show()
        {
            base.Show();
            icon.gameObject.SetActive(true);
        }

        public override void Hide()
        {
            base.Hide();
            icon.gameObject.SetActive(false);
        }
    }
}