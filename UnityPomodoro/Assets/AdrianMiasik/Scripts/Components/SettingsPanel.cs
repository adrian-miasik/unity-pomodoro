using AdrianMiasik.Components.Core;
using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
using TMPro;
using UnityEngine;

namespace AdrianMiasik.Components
{
    public class SettingsPanel : MonoBehaviour, IColorHook
    {
        [SerializeField] private TMP_Text m_title;
        [SerializeField] private LabelledDropdown m_digitFormatDropdown;
        [SerializeField] private TMP_Text m_muteSoundOutOfFocusLabel;
        [SerializeField] private BooleanSlider m_muteSoundOutOfFocusBoolean;

        private bool isInitialized;
        private bool isOpen;
        private PomodoroTimer timer;
        
        public void Initialize(PomodoroTimer pomodoroTimer)
        {
            timer = pomodoroTimer;
            timer.GetTheme().RegisterColorHook(this);
            
            m_muteSoundOutOfFocusBoolean.OverrideFalseColor(timer.GetTheme().GetCurrentColorScheme().m_backgroundHighlight);
            m_muteSoundOutOfFocusBoolean.OverrideTrueColor(timer.GetTheme().GetCurrentColorScheme().m_modeOne);
            
            m_digitFormatDropdown.Initialize(timer);
            m_muteSoundOutOfFocusBoolean.Initialize(pomodoroTimer, pomodoroTimer.MuteSoundWhenOutOfFocus());

            isInitialized = true;
        }
        
        public bool IsInitialized()
        {
            return isInitialized;
        }

        public void ColorUpdate(Theme theme)
        {
            if (isOpen)
            {
                m_title.color = theme.GetCurrentColorScheme().m_foreground;
                m_digitFormatDropdown.ColorUpdate(timer.GetTheme());
                m_muteSoundOutOfFocusLabel.color = theme.GetCurrentColorScheme().m_foreground;
                m_muteSoundOutOfFocusBoolean.ColorUpdate(theme);
            }
        }

        public void UpdateDropdown()
        {
            m_digitFormatDropdown.SetDropdownValue(timer.GetDigitFormat());
        }

        public void SetDropdown(int value)
        {
            m_digitFormatDropdown.SetDropdownValue(value);
        }
        
        public void Show()
        {
            gameObject.SetActive(true);
            m_digitFormatDropdown.SetDropdownValue(timer.GetDigitFormat());
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
