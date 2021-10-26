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

        public void SetSeparatorScale(float _scale = 1f)
        {
            separator.transform.localScale = Vector3.one * _scale;
        }
    }
}
