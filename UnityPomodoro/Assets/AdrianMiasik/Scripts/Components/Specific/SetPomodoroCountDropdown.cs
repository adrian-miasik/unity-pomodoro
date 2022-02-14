using System;
using AdrianMiasik.Components.Base;
using AdrianMiasik.ScriptableObjects;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace AdrianMiasik.Components.Specific
{
    // TODO: Create Dropdown base class and implement here and in DigitFormatDropdown.cs
    /// <summary>
    /// A <see cref="ThemeElement"/> dropdown with a label.
    /// Intended to be used for 'Set Pomodoro Count' settings option. (See <see cref="SettingsPanel"/>)
    /// </summary>
    public class SetPomodoroCountDropdown : ThemeElement
    {
        [SerializeField] private TMP_Text m_label;
        [SerializeField] private TMP_Text m_dropdownText;
        [SerializeField] private TMP_Dropdown m_dropdown;
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

        /// <summary>
        /// <remarks>Used as a UnityEvent on the TMP_Dropdown. See <see cref="m_dropdown"/></remarks>
        /// </summary>
        /// <param name="i"></param>
        public void SetPomodoroCount(Int32 i)
        {
            int desiredCount = i + 1; // Dependant on our dropdown options.

            if (Timer.HasTomatoProgression())
            {
                // If the new count removes/truncates potential progress...
                if (Timer.GetTomatoProgress() > desiredCount)
                {
                    Timer.SpawnConfirmationDialog(() =>
                    {
                        // Set to new count and remove additional progress
                        Timer.SetPomodoroCount(desiredCount, desiredCount);
                    }, () =>
                    {
                        SetDropdownValue(Timer.GetTomatoCount() - 1);
                    }, "This action will delete some of your pomodoro/tomato progress.");
                }
                else
                {
                    // New count number is higher than our progress
                    Timer.SetPomodoroCount(desiredCount, Timer.GetTomatoProgress());
                }
            }
            else
            {
                // No progress
                Timer.SetPomodoroCount(desiredCount, 0);
            }
        }
    }
}
