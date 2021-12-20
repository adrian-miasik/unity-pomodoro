using TMPro;
using UnityEngine;

namespace AdrianMiasik.Components.Core
{
    public class ClickButtonText: ClickButton
    {
        public TMP_Text text;
        
        public override void Show()
        {
            base.Show();
            text.gameObject.SetActive(true);
        }

        public override void Hide()
        {
            base.Hide();
            text.gameObject.SetActive(false);
        }
    }
}