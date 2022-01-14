using TMPro;
using UnityEngine;

namespace AdrianMiasik.Components
{
    /// <summary>
    /// Writes the current Unity application version (bundleVersion) to the referenced text field.
    /// </summary>
    public class WriteVersionNumber : MonoBehaviour
    {
        [SerializeField] private TMP_Text m_text;
        [SerializeField] private string m_prefixString = "v";

        private void Start()
        {
            m_text.text = m_prefixString + Application.version;
        }

        /// <summary>
        /// Sets the color of the referenced text to the provided color. <param name="newColor"></param>
        /// </summary>
        /// <param name="newColor">The color you want the referenced text to change to.</param>
        public void SetTextColor(Color newColor)
        {
            m_text.color = newColor;
        }
    }
}
