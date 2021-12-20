using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace AdrianMiasik.Components
{
    public class Background : MonoBehaviour, IColorHook
    {
        [SerializeField] private Image background;
        [SerializeField] private Selectable selectable;

        public void Initialize(PomodoroTimer _timer)
        {
            _timer.GetTheme().RegisterColorHook(this);
            ColorUpdate(_timer.GetTheme());
        }
        
        public void Select()
        {
            selectable.Select();
        }

        public void ColorUpdate(Theme _theme)
        {
            background.color = _theme.GetCurrentColorScheme().m_background;
        }

        public void SetSelectionNavigation(Navigation _backgroundNav)
        {
            selectable.navigation = _backgroundNav;
        }
    }
}
