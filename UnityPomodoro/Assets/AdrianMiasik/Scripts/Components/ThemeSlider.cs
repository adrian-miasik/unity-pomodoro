using AdrianMiasik.Components.Core;
using AdrianMiasik.ScriptableObjects;
using TMPro;
using UnityEngine;

namespace AdrianMiasik.Components
{
    public class ThemeSlider : ThemeElement
    {
        [SerializeField] private BooleanSlider toggle;
        [SerializeField] private TMP_Text label;
        
        public void Initialize(PomodoroTimer _timer, bool _state)
        {
            base.Initialize(_timer);
            
            toggle.OverrideDotColor(_timer.GetTheme().GetCurrentColorScheme().foreground);
            toggle.Initialize(_timer, _state);
        }

        public override void ColorUpdate(Theme _theme)
        {
            toggle.ColorUpdate(_theme);
            label.color = _theme.GetCurrentColorScheme().foreground;
            label.text = timer.GetTheme().isLightModeOn ? "Dark" : "Light";
            // TODO: Change toggle dot image to a moon for dark mode
        }

        // Piper methods
        public void OverrideFalseColor(Color _color)
        {
            toggle.OverrideFalseColor(_color);
        }

        public void OverrideTrueColor(Color _color)
        {
            toggle.OverrideTrueColor(_color);
        }

        public void Interact()
        {
            toggle.OnPointerClick(null);
        }
    }
}
