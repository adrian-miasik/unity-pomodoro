using System.Collections.Generic;
using AdrianMiasik.ScriptableObjects;
using UnityEngine;

namespace AdrianMiasik.Components
{
    public class SocialButtons : MonoBehaviour
    {
        [SerializeField] private List<UPIcon> icons = new List<UPIcon>();

        public void ColorUpdate(Theme _theme)
        {
            foreach (UPIcon _icon in icons)
            {
                _icon.ColorUpdate(_theme);
            }
        }
    }
}