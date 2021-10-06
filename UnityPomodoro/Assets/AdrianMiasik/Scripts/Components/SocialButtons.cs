using System.Collections.Generic;
using UnityEngine;

namespace AdrianMiasik.Components
{
    public class SocialButtons : MonoBehaviour
    {
        [SerializeField] private List<UPIcon> icons = new List<UPIcon>();

        public void ColorUpdate(Theme theme)
        {
            foreach (UPIcon icon in icons)
            {
                icon.ColorUpdate(theme);
            }
        }
    }
}