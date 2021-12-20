using System.Collections.Generic;
using UnityEngine;

namespace AdrianMiasik.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Color Scheme", menuName = "Adrian Miasik/Create New Color Scheme")]
    public class ColorScheme : ScriptableObject
    {
       public List<Theme> m_includedInTheseThemes = new List<Theme>();

        private void OnValidate()
        {
            foreach (Theme theme in m_includedInTheseThemes)
            {
                theme.ApplyColorChanges();
            }
        }

        public Color m_background = Color.white;
        public Color m_backgroundHighlight = new Color(0.91f, 0.91f, 0.91f); 
        public Color m_foreground = Color.black;
        public Color m_modeOne = new Color(0.05f, 0.47f, 0.95f);
        public Color m_running = new Color(0.35f, 0.89f, 0.4f);
        public Color m_complete = new Color(0.97f, 0.15f, 0.15f);
        public Color m_modeTwo = new Color(1f, 0.83f, 0.23f);
        public Color m_close = Color.red;
    }
}