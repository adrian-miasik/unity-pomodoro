using AdrianMiasik.Components.Core;
using AdrianMiasik.ScriptableObjects;
using TMPro;
using UnityEngine;

namespace AdrianMiasik.Components
{
    public class SettingsPanel : ThemeElement
    {
        [SerializeField] private TMP_Text m_title;
        [SerializeField] private LabelledDropdown m_digitFormatDropdown;
        [SerializeField] private TMP_Text m_muteSoundOutOfFocusLabel;
        [SerializeField] private BooleanSlider m_muteSoundOutOfFocusBoolean;

        private bool isInitialized;
        private bool isOpen;

        public void Initialize(PomodoroTimer pomodoroTimer)
        {
            base.Initialize(pomodoroTimer, false);
            
            m_muteSoundOutOfFocusBoolean.OverrideFalseColor(Timer.GetTheme().GetCurrentColorScheme().m_backgroundHighlight);
            m_muteSoundOutOfFocusBoolean.OverrideTrueColor(Timer.GetTheme().GetCurrentColorScheme().m_modeOne);
            
            m_digitFormatDropdown.Initialize(Timer);
            m_muteSoundOutOfFocusBoolean.Initialize(pomodoroTimer, pomodoroTimer.MuteSoundWhenOutOfFocus());

            isInitialized = true;
        }
        
        public bool IsInitialized()
        {
            return isInitialized;
        }

        public override void ColorUpdate(Theme theme)
        {
            if (isOpen)
            {
                m_title.color = theme.GetCurrentColorScheme().m_foreground;
                m_digitFormatDropdown.ColorUpdate(Timer.GetTheme());
                m_muteSoundOutOfFocusLabel.color = theme.GetCurrentColorScheme().m_foreground;
                m_muteSoundOutOfFocusBoolean.ColorUpdate(theme);
            }
        }

        public void UpdateDropdown()
        {
            m_digitFormatDropdown.SetDropdownValue(Timer.GetDigitFormat());
        }

        public void SetDropdown(int value)
        {
            m_digitFormatDropdown.SetDropdownValue(value);
        }
        
        public void Show()
        {
            gameObject.SetActive(true);
            m_digitFormatDropdown.SetDropdownValue(Timer.GetDigitFormat());
            isOpen = true;
            ColorUpdate(Timer.GetTheme());
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
