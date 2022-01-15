using AdrianMiasik.Components.Core.Containers;
using TMPro;
using UnityEngine;

namespace AdrianMiasik.Components.Core.Items
{
    /// <summary>
    /// A single text component with the ability to set it's color.
    /// (Also see <see cref="DigitFormat"/>)
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
