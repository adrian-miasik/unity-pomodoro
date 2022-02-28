using AdrianMiasik.Components.Specific;
using AdrianMiasik.Components.Specific.Pages;
using AdrianMiasik.Components.Specific.Settings;
using AdrianMiasik.ScriptableObjects;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;
using Dropdown = AdrianMiasik.Components.Core.Dropdown;

namespace AdrianMiasik.Components.Base
{
    /// <summary>
    /// A **base class** <see cref="ThemeElement"/> that has a label with a <see cref="Dropdown"/> intended to be used
    /// as a settings option on the <see cref="SettingsPage"/>.
    /// </summary>
    public class SettingsOptionDropdown : ThemeElement
    {
        [SerializeField] private TMP_Text m_label;
        [SerializeField] private TMP_Text m_dropdownText;
        [SerializeField] protected Dropdown m_dropdown;
        [SerializeField] private Image m_containerOutline;
        [SerializeField] private SVGImage m_arrow;

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