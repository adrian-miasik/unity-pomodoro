using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace AdrianMiasik.Components
{
    public class Background : MonoBehaviour, IColorHook
    {
        [SerializeField] private Image m_background;
        [SerializeField] private Selectable m_selectable;

        public void Initialize(PomodoroTimer pomodoroTimer)
        {
            pomodoroTimer.GetTheme().RegisterColorHook(this);
            ColorUpdate(pomodoroTimer.GetTheme());
        }
        
        public void Select()
        {
            m_selectable.Select();
        }

        public void ColorUpdate(Theme theme)
        {
            m_background.color = theme.GetCurrentColorScheme().m_background;
        }

        public void SetSelectionNavigation(Navigation backgroundNav)
        {
            m_selectable.navigation = backgroundNav;
        }
    }
}
