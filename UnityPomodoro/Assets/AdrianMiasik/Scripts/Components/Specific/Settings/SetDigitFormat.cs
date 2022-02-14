using AdrianMiasik.Components.Base;
using AdrianMiasik.ScriptableObjects;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace AdrianMiasik.Components.Specific.Settings
{
    /// <summary>
    /// A <see cref="ThemeElement"/> dropdown with a label.
    /// Intended to be used for 'Set digit format' settings option. (See <see cref="SettingsPanel"/>)
    /// </summary>
    public class SetDigitFormat : ThemeElement
    {
        [SerializeField] private TMP_Text m_label;
        [SerializeField] private TMP_Text m_dropdownText;
        [SerializeField] private TMP_Dropdown m_dropdown;
        [SerializeField] private Image m_containerOutline;
        [SerializeField] private SVGImage m_arrow;

        /// <summary>
        /// Sets the dropdown value to the <see cref="PomodoroTimer"/>'s current digit format.
        /// </summary>
        /// <param name="pomodoroTimer"></param>
        public void Initialize(PomodoroTimer pomodoroTimer)
        {
            base.Initialize(pomodoroTimer);

            SetDropdownValue(Timer.GetDigitFormat());
        }

        /// <summary>
        /// Set the dropdown selection to the provided index value.
        /// </summary>
        /// <param name="selectionValue"></param>
        public void SetDropdownValue(int selectionValue)
        {
            // Set dropdown value to current digit format 
            // Note: This will trigger an OnValueChanged invoke
            m_dropdown.value = selectionValue;
        }

        /// <summary>
        /// Applies our <see cref="Theme"/> changes to our referenced components when necessary.
        /// </summary>
        /// <param name="theme">The theme to apply on our referenced components.</param>
        public override void ColorUpdate(Theme theme)
        {
            m_label.color = theme.GetCurrentColorScheme().m_foreground;
            m_dropdownText.color = theme.GetCurrentColorScheme().m_foreground;
            m_dropdown.captionText.color = theme.GetCurrentColorScheme().m_foreground;
            m_containerOutline.color = theme.GetCurrentColorScheme().m_backgroundHighlight;
            m_arrow.color = theme.GetCurrentColorScheme().m_foreground;
        }
    }
}