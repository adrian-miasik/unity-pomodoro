using System.Collections.Generic;
using AdrianMiasik.ScriptableObjects;
using UnityEngine;

namespace AdrianMiasik.Components
{
    /// <summary>
    /// Holds a list of <see cref="UPIcon"/>'s and updates their color's based on the current <see cref="Theme"/>.
    /// Intended to be used in the <see cref="AboutPanel"/>.
    /// </summary>
    public class SocialButtons : MonoBehaviour
    {
        [SerializeField] private List<UPIcon> m_icons = new List<UPIcon>();

        /// <summary>
        /// Applies our <see cref="Theme"/> changes to our referenced components when necessary.
        /// </summary>
        /// <param name="theme">The theme to apply on our referenced components.</param>
        public void ColorUpdate(Theme theme)
        {
            foreach (UPIcon icon in m_icons)
            {
                icon.ColorUpdate(theme);
            }
        }
    }
}