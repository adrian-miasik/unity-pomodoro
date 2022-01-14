using System.Collections.Generic;
using AdrianMiasik.Components.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AdrianMiasik.Components
{
    /// <summary>
    /// Responsible for our detecting and executing our keyboard shortcut actions.
    /// Single keys are processed in `ProcessKeys()`,
    /// and multi-keys are processed in `ProcessKeybinds()`.
    /// </summary>
    public class HotkeyDetector : MonoBehaviour
    {
        private PomodoroTimer timer;
        private bool isInitialized;

        public void Initialize(PomodoroTimer pomodoroTimer)
        {
            timer = pomodoroTimer;
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
            if (Input.GetKeyDown(KeyCode.T))
            {
                timer.TriggerThemeSwitch();
            }

            // Clear digit selection
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                timer.ClearSelection();
            }
            
            // Restart application
            if (Input.GetKeyDown(KeyCode.F5))
            {
                timer.SpawnConfirmationDialog(() =>
                {
                    timer.GetTheme().DeregisterAllElements();
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }, null, 
                    "This action will <color=red>reset all settings to their factory defaults.</color>", 
                    null, 
                    false);
            }

            // Tab between digits
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                List<Selectable> selectables = timer.GetSelections();
                if (selectables.Count >= 1)
                {
                    // Get only first selection
                    Selectable selection = selectables[0];
                    Selectable rightSelection = selection.FindSelectableOnRight();
                    if (rightSelection != null && rightSelection.gameObject != null)
                    {
                        EventSystem.current.SetSelectedGameObject(rightSelection.gameObject);
                    }
                }
            }
        }

        /// <summary>
        /// Processes combined key strokes and multiple inputs
        /// </summary>
        private void ProcessKeybinds()
        {
            // Restart timer / Switch timer mode
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (!IsUserHoldingControl())
                {
                    timer.TriggerTimerRestart();
                }
                else
                {
                    timer.TriggerTimerSwitch();
                }
            }
            
            // Control modifier
            if (IsUserHoldingControl())
            {
                // Switch theme
                if (Input.GetKeyDown(KeyCode.U))
                {
                    timer.TriggerThemeSwitch();
                }
                
                // Switch digit layouts
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

        private bool IsUserHoldingControl()
        {
            return Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
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