using System;
using System.Collections.Generic;
using UnityEngine;

namespace AdrianMiasik
{
    [CreateAssetMenu(fileName = "New Color Scheme", menuName = "Adrian Miasik/Create New Color Scheme")]
    public class ColorScheme : ScriptableObject
    {
        public List<Theme> includedInTheseThemes = new List<Theme>();

        private void OnValidate()
        {
            foreach (Theme theme in includedInTheseThemes)
            {
                theme.ApplyColorChanges();
            }
        }

        public Color background = Color.white;
        public Color backgroundHighlight = new Color(0.91f, 0.91f, 0.91f);
        public Color foreground = Color.black;
        public Color close = Color.red;
        public Color modeOne = new Color(0.05f, 0.47f, 0.95f); // Work
        public Color modeTwo = new Color(1f, 0.83f, 0.23f); // Break
        public Color running = new Color(0.35f, 0.89f, 0.4f);
        public Color complete = new Color(0.97f, 0.15f, 0.15f);
    }
}