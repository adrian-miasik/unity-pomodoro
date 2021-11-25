using AdrianMiasik.Components.Core;
using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
using TMPro;
using UnityEngine;

namespace AdrianMiasik.Components
{
    public class SettingsPanel : MonoBehaviour, IColorHook
    {
        [SerializeField] private TMP_Text title;
        [SerializeField] private LabelledDropdown digitFormatDropdown;
        [SerializeField] private TMP_Text themeLabel;
        [SerializeField] private BooleanSlider themeSlider;

        private PomodoroTimer timer;

        public void Initialize(PomodoroTimer _timer)
        {
            timer = _timer;
            timer.GetTheme().RegisterColorHook(this);

            themeSlider.OverrideFalseColor(_timer.GetTheme().light.backgroundHighlight);
            themeSlider.OverrideTrueColor(new Color(0.59f, 0.33f, 1f));
            
            themeSlider.Initialize(_timer, !timer.GetTheme().isLightModeOn);
            if (timer.GetTheme().isLightModeOn)
            {
                themeSlider.Disable();
            }
            else
            {
                themeSlider.Enable();
            }
            
            digitFormatDropdown.Initialize(timer);
        }

        public BooleanSlider GetThemeSlider()
        {
            return themeSlider;
        }

        public void ColorUpdate(Theme _theme)
        {
            // TODO: Check if settings page is open
            title.color = _theme.GetCurrentColorScheme().foreground;
            themeLabel.color = _theme.GetCurrentColorScheme().foreground;
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
