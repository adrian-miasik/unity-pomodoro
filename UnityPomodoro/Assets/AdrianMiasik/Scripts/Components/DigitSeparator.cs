using TMPro;
using UnityEngine;

namespace AdrianMiasik.Components
{
    public class DigitSeparator : MonoBehaviour
    {
        [SerializeField] private TMP_Text m_separator;

        public void SetSeparatorColor(Color newColor)
        {
            m_separator.color = newColor;
        }
    }
}
