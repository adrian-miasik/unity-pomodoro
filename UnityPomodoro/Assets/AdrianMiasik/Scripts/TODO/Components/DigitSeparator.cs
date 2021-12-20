using TMPro;
using UnityEngine;

namespace AdrianMiasik.Components
{
    public class DigitSeparator : MonoBehaviour
    {
        [SerializeField] private TMP_Text separator;

        public void SetSeparatorColor(Color _newColor)
        {
            separator.color = _newColor;
        }
    }
}
