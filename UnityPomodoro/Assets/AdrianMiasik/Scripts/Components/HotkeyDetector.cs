using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AdrianMiasik.Components
{
    public class HotkeyDetector : MonoBehaviour
    {
        private PomodoroTimer timer;
        private bool isInitialized;

        public void Initialize(PomodoroTimer _pomodoroTimer)
        {
            timer = _pomodoroTimer;
            isInitialized = true;
        }

        private void Update()
        {
            if (!isInitialized)
            {
                return;
            }
            
            ProcessKeys();
            ProcessKeybinds();
        }

        /// <summary>
        /// Processes individual key strokes and single inputs
        /// </summary>
        private void ProcessKeys()
        {
            // Play / pause the timer
            if (Input.GetKeyDown(KeyCode.Space))
            {
                timer.TriggerPlayPause();
            }
            
            // Quick switch
            if (Input.GetKeyDown(KeyCode.Q))
            {
                timer.TriggerTimerSwitch();
            }
            
            // Theme switch
            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
            {
                timer.TriggerThemeSwitch();
            }

            // Clear digit selection
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                timer.ClearSelection();
            }
            
            // Tab between digits
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                List<Selectable> _selectables = timer.GetSelections();
                if (_selectables.Count >= 1)
                {
                    // Get only first selection
                    Selectable _selection = _selectables[0];
                    Selectable _rightSelection = _selection.FindSelectableOnRight();
                    if (_rightSelection != null && _rightSelection.gameObject != null)
                    {
                        EventSystem.current.SetSelectedGameObject(_rightSelection.gameObject);
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.F1))
            {
                timer.TryChangeFormat(DigitFormat.SupportedFormats.SS);
            }
            
            if (Input.GetKeyDown(KeyCode.F2))
            {
                timer.TryChangeFormat(DigitFormat.SupportedFormats.MM_SS);
            }
            
            if (Input.GetKeyDown(KeyCode.F3))
            {
                timer.TryChangeFormat(DigitFormat.SupportedFormats.HH_MM_SS);
            }
            
            if (Input.GetKeyDown(KeyCode.F4))
            {
                timer.TryChangeFormat(DigitFormat.SupportedFormats.HH_MM_SS_MS);
            }
            
            if (Input.GetKeyDown(KeyCode.F5))
            {
                timer.TryChangeFormat(DigitFormat.SupportedFormats.DD_HH_MM_SS_MS);
            }
        }

        /// <summary>
        /// Processes combined key strokes and multiple inputs
        /// </summary>
        private void ProcessKeybinds()
        {
            // Restart timer
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                {
                    timer.TriggerTimerSwitch();
                }
                else
                {
                    timer.TriggerTimerRestart();
                }
            }
            
            // Select All
            if (IsUserSelectingAll())
            {
                timer.SelectAll();
            }
            
            // Copy
            if (IsUserCopying())
            {
                GUIUtility.systemCopyBuffer = timer.GetTimerString();
            }
            
            // Paste
            if (IsUserPasting())
            {
                timer.SetTimerValue(GUIUtility.systemCopyBuffer);
            }
        }

        private bool IsUserSelectingAll()
        {
            return Input.GetKeyDown(KeyCode.A) && Input.GetKey(KeyCode.LeftControl) ||
                   Input.GetKeyDown(KeyCode.A) && Input.GetKey(KeyCode.RightControl);
        }
        
        private bool IsUserCopying()
        {
            return Input.GetKeyDown(KeyCode.C) && Input.GetKey(KeyCode.LeftControl) ||
                   Input.GetKeyDown(KeyCode.C) && Input.GetKey(KeyCode.RightControl);
        }
        
        private bool IsUserPasting()
        {
            return Input.GetKeyDown(KeyCode.V) && Input.GetKey(KeyCode.LeftControl) ||
                   Input.GetKeyDown(KeyCode.V) && Input.GetKey(KeyCode.RightControl);
        }
    }
}