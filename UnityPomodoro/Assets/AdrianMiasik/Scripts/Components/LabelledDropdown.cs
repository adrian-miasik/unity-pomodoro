using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
using TMPro;
using UnityEngine;

namespace AdrianMiasik.Components
{
    public class LabelledDropdown : MonoBehaviour, IColorHook
    {
        [SerializeField] private TMP_Text label;
        [SerializeField] private TMP_Dropdown dropdown;

        private PomodoroTimer timer;
    
        public void Initialize(PomodoroTimer _timer)
        {
            timer = _timer;
            timer.GetTheme().RegisterColorHook(this);
        }

        public void ColorUpdate(Theme _theme)
        {
            label.color = _theme.GetCurrentColorScheme().foreground;
        }
    }
}
