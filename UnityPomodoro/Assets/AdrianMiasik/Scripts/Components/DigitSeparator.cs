using TMPro;
using UnityEngine;

namespace AdrianMiasik.Components
{
    /// <summary>
    /// A single text component used to hold reference and set it's color.
    /// </summary>
    public class DigitSeparator : MonoBehaviour
    {
        [SerializeField] private TMP_Text m_separator;

        /// <summary>
        /// Sets the separator's color to the provided color.
        /// </summary>
        /// <param name="newColor">The color you want this separator to be.</param>
        public void SetSeparatorColor(Color newColor)
        {
            m_separator.color = newColor;
        }
    }
}
