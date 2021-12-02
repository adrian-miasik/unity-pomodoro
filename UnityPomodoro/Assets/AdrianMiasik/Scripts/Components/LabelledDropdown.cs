using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace AdrianMiasik.Components
{
    public class LabelledDropdown : MonoBehaviour, IColorHook
    {
        [SerializeField] private TMP_Text label;
        [SerializeField] private TMP_Dropdown dropdown;
        [SerializeField] private Image containerOutline;
        [SerializeField] private SVGImage arrow;

        private PomodoroTimer timer;
    
        public void Initialize(PomodoroTimer _timer)
        {
            timer = _timer;
            timer.GetTheme().RegisterColorHook(this);
        }

        public void ColorUpdate(Theme _theme)
        {
            label.color = _theme.GetCurrentColorScheme().foreground;
            dropdown.captionText.color = _theme.GetCurrentColorScheme().foreground;
            containerOutline.color = _theme.GetCurrentColorScheme().foreground;
            arrow.color = _theme.GetCurrentColorScheme().foreground;
        }
    }
}
