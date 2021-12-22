using TMPro;
using UnityEngine;

namespace AdrianMiasik.Components
{
    public class WriteVersionNumber : MonoBehaviour
    {
        [SerializeField] private TMP_Text m_text;
        [SerializeField] private string m_prefixString = "v";

        private void Start()
        {
            m_text.text = m_prefixString + Application.version;
        }

        public void SetTextColor(Color newColor)
        {
            m_text.color = newColor;
        }
    }
}
