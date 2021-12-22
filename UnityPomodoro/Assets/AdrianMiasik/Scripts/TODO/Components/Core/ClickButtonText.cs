using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace AdrianMiasik.Components.Core
{
    public class ClickButtonText: ClickButton
    {
        [FormerlySerializedAs("text")] public TMP_Text m_text;
        
        public override void Show()
        {
            base.Show();
            m_text.gameObject.SetActive(true);
        }

        public override void Hide()
        {
            base.Hide();
            m_text.gameObject.SetActive(false);
        }
    }
}