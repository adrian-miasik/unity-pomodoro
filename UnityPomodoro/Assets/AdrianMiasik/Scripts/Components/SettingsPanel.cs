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
        [SerializeField] private TMP_Text muteSoundOutOfFocusLabel;
        [SerializeField] private BooleanSlider muteSoundOutOfFocusBoolean;

        private bool isInitialized;
        private bool isOpen = false;
        private PomodoroTimer timer;

        private int digitFormatPreviousSelection;

        public void Initialize(PomodoroTimer _timer)
        {
            timer = _timer;
            timer.GetTheme().RegisterColorHook(this);
            
            muteSoundOutOfFocusBoolean.OverrideFalseColor(timer.GetTheme().GetCurrentColorScheme().backgroundHighlight);
            muteSoundOutOfFocusBoolean.OverrideTrueColor(timer.GetTheme().GetCurrentColorScheme().modeOne);
            
            digitFormatDropdown.Initialize(timer);
            muteSoundOutOfFocusBoolean.Initialize(_timer, _timer.MuteSoundWhenOutOfFocus());

            digitFormatPreviousSelection = digitFormatDropdown.GetDropdownValue();
            isInitialized = true;
        }
        
        public bool IsInitialized()
        {
            return isInitialized;
        }

        public void ColorUpdate(Theme _theme)
        {
            if (isOpen)
            {
                title.color = _theme.GetCurrentColorScheme().foreground;
                digitFormatDropdown.ColorUpdate(timer.GetTheme());
                muteSoundOutOfFocusLabel.color = _theme.GetCurrentColorScheme().foreground;
                muteSoundOutOfFocusBoolean.ColorUpdate(_theme);
            }
        }

        public void UpdateDropdown()
        {
            digitFormatPreviousSelection = digitFormatDropdown.GetDropdownValue();
            digitFormatDropdown.UpdateDropdownValue(timer.GetDigitFormat());
        }

        public void RevertDropdownToPreviousSelection()
        {
            digitFormatDropdown.UpdateDropdownValue(digitFormatPreviousSelection);
        }

        public void Show()
        {
            gameObject.SetActive(true);
            digitFormatDropdown.UpdateDropdownValue(timer.GetDigitFormat());
            isOpen = true;
            ColorUpdate(timer.GetTheme());
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            isOpen = false;
        }

        public bool IsPageOpen()
        {
            return isOpen;
        }
    }
}
