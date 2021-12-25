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
        [SerializeField] private TMP_Text m_label;
        [SerializeField] private TMP_Dropdown m_dropdown;
        [SerializeField] private Image m_containerOutline;
        [SerializeField] private SVGImage m_arrow;

        private PomodoroTimer timer;
    
        public void Initialize(PomodoroTimer pomodoroTimer)
        {
            timer = pomodoroTimer;
            timer.GetTheme().RegisterColorHook(this);

            SetDropdownValue(timer.GetDigitFormat());
        }

        public void SetDropdownValue(int selectionValue)
        {
            // Set dropdown value to current digit format 
            // Note: This will trigger an OnValueChanged invoke
            m_dropdown.value = selectionValue;
        }

        public void ColorUpdate(Theme theme)
        {
            m_label.color = theme.GetCurrentColorScheme().m_foreground;
            m_dropdown.captionText.color = theme.GetCurrentColorScheme().m_foreground;
            m_containerOutline.color = theme.GetCurrentColorScheme().m_backgroundHighlight;
            m_arrow.color = theme.GetCurrentColorScheme().m_foreground;
        }
    }
}