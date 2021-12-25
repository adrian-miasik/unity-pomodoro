using System.Collections.Generic;
using AdrianMiasik.ScriptableObjects;
using UnityEngine;

namespace AdrianMiasik.Components
{
    public class SocialButtons : MonoBehaviour
    {
        [SerializeField] private List<UPIcon> m_icons = new List<UPIcon>();

        public void ColorUpdate(Theme theme)
        {
            foreach (UPIcon icon in m_icons)
            {
                icon.ColorUpdate(theme);
            }
        }
    }
}