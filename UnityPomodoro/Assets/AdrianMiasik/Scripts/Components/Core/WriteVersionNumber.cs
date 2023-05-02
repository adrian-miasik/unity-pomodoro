#if !UNITY_ANDROID && !UNITY_WSA
using Steamworks;
#endif
using TMPro;
using UnityEngine;

namespace AdrianMiasik.Components.Core
{
    /// <summary>
    /// Writes the current Unity application version to the referenced text field.
    /// </summary>
    public class WriteVersionNumber : MonoBehaviour
    {
        [SerializeField] private TMP_Text m_text;
        [SerializeField] private string m_prefixString = "v";

        public void Initialize()
        {
            m_text.text = m_prefixString + Application.version;

#if !UNITY_ANDROID && !UNITY_WSA
            // Append Steam flag to version numbering as suffix
            if (SteamClient.IsValid)
            {
                m_text.text += "-s";
            }
#endif
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