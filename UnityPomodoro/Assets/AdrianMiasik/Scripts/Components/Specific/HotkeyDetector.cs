using System.Collections.Generic;
using AdrianMiasik.Components.Core;
using AdrianMiasik.Components.Core.Containers;
using AdrianMiasik.Components.Core.Settings;
#if !UNITY_ANDROID
using Steamworks;
#endif
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;using AdrianMiasik.Components.Core.Items.Pages;
using System.Collections.Generic;

namespace AdrianMiasik.Components.Specific
{
    /// <summary>
    /// Responsible for our detecting and executing our keyboard shortcuts/binding actions.
    /// Single keys are processed in `ProcessKeys()`,
    /// and multi-keys are processed in `ProcessKeybinds()`.
    /// </summary>
    public class HotkeyDetector : MonoBehaviour
    {
        public TodoPage todoPage;
        private bool todoIsOpen;
        public CustomizationPage customizationPage;
        private bool customizationIsOpen;
        private PomodoroTimer timer;
        private bool isInitialized;

        private bool ignoreInput;
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

            if (ignoreInput)
            {
                return;
            }
            
            todoIsOpen = todoPage.isOpen;
            customizationIsOpen = customizationPage.isOpen;
            ProcessKeys();
            ProcessKeybinds();
        }

        /// <summary>
        /// Processes individual key strokes and single inputs
        /// </summary>
        private void ProcessKeys()
        {
            if (!todoIsOpen && !customizationIsOpen)
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

                if (Input.GetKeyDown(KeyCode.Return))
                {
                    timer.TrySubmitConfirmationDialog();
                }

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    // Clear digit selection
                    timer.ClearSelection();

                    // Cancel confirmation dialog
                    timer.TryCancelConfirmationDialog();
                }

            }

        }

        private void PromptApplicationRestart(bool quitApplicationOnRestartConfirm = false)
        {
            string topText = "This action will <color=red>reset all settings to their factory defaults.</color>";

#if !UNITY_ANDROID
            if (SteamClient.IsValid)
            {
                topText += " This will also wipe your Steam stats, any unlocked/progressed Steam achievements, and" +
                           " any uploaded Steam cloud save data.";
            }
#endif

            if (quitApplicationOnRestartConfirm)
            {
                topText += " (Quitting on Confirmation)";
            }
                        
            timer.GetConfirmDialogManager().ClearCurrentDialogPopup();
            timer.GetConfirmDialogManager().SpawnConfirmationDialog(() =>
                {
#if !UNITY_ANDROID
                    SteamUserStats.ResetAll(true);
                    SteamUserStats.StoreStats();
                    SteamUserStats.RequestCurrentStats();
#endif
                    RestartApplication(quitApplicationOnRestartConfirm);
                }, null, 
                topText, 
                null, 
                false);
        }

        private void RestartApplication(bool quitApplicationOnRestart = false)
        {
            timer.GetTheme().DeregisterAllElements();

            UserSettingsSerializer.DeleteSettingsFile("system-settings");
            UserSettingsSerializer.DeleteSettingsFile("timer-settings");
            
#if !UNITY_ANDROID
            timer.ShutdownSteamManager();
#endif
            Debug.Log("Application: Factory Reset");

            if (!quitApplicationOnRestart)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            else
            {
#if UNITY_EDITOR
                EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }
        }

        /// <summary>
        /// Processes combined key strokes and multiple inputs
        /// </summary>
        private void ProcessKeybinds()
        {
            // Restart timer / Switch timer mode
            if (Input.GetKeyDown(KeyCode.R) && !todoIsOpen && !customizationIsOpen)
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
            
            if (!IsUserHoldingControl() && Input.GetKeyDown(KeyCode.F5))
            {
                timer.TryChangeFormat(DigitFormat.SupportedFormats.DD_HH_MM_SS_MS);
            }

            if (IsUserHoldingControl())
            {
                // Switch theme
                if (Input.GetKeyDown(KeyCode.U))
                {
                    timer.TriggerThemeSwitch();
                }

                if (Input.GetKeyDown(KeyCode.F5))
                {
                    if (IsUserHoldingShift())
                    {
                        // Restart application and quit
                        PromptApplicationRestart(true);
                    }
                    else
                    {
                        // Restart application
                        PromptApplicationRestart();
                    }
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

        private bool IsUserHoldingShift()
        {
            return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
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

        public void PauseInputs()
        {
            ignoreInput = true;
        }

        public void ResumeInputs()
        {
            ignoreInput = false;
        }
    }
}