using AdrianMiasik.Components.Core;
using AdrianMiasik.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace AdrianMiasik.Components
{
    public class Background : ThemeElement
    {
        [SerializeField] private Image m_background;
        [SerializeField] private Selectable m_selectable;
        
        public void Select()
        {
            m_selectable.Select();
        }

        public override void ColorUpdate(Theme theme)
        {
            m_background.color = theme.GetCurrentColorScheme().m_background;
        }

        public void SetSelectionNavigation(Navigation backgroundNav)
        {
            m_selectable.navigation = backgroundNav;
        }
    }
}
