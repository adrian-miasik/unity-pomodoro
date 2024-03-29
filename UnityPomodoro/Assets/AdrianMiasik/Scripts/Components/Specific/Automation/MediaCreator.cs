using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AdrianMiasik.Components.Specific.Automation
{
    /// <summary>
    /// Generates and creates our promotional screenshots automatically (Previously this was done manually).
    /// <remarks>Intended to be run for any new release so we can get the most up-to-date app media.</remarks>
    /// <remarks>To use this tool: Make sure your editor game view window matches your desired screenshot resolution.
    /// Then enter play mode. Once the timer spawn animation completes, right click the PomodoroTimer script in the
    /// Inspector, and press "Create Media". This will start the screenshot scenarios and generate
    /// screenshots for our release. See console log for output details.</remarks>
    /// </summary>
    public class MediaCreator: MonoBehaviour
    {
        private static PomodoroTimer _timer;
        private static Queue<Action> _screenshotScenarios = new();
        private static Queue<Action> _ssCopy = new(); // Intended to be used for dark mode capture
        private static bool _hasDarkModeBeenCaptured;
        private static int _screenshotIndex;
        private static bool _startingThemeDarkMode;
        
#if UNITY_EDITOR
        [MenuItem("CONTEXT/PomodoroTimer/Create Media")]
        private static void CreateMedia(MenuCommand command)
        {
            // Early Exit
            if (!Application.isPlaying)
            {
                Debug.LogWarning("Please make sure Unity is in play mode and this timer is initialized to " +
                                 "create your media.");
                Debug.Log("No media has been created.");
                return;
            }
            
            // Clear
            _screenshotIndex = 0;
            _hasDarkModeBeenCaptured = false;
            
            // Get reference
            _timer = (PomodoroTimer) command.context;
            _startingThemeDarkMode = _timer.GetSystemSettings().m_darkMode;
            
            // Create media capture object
            MediaCapture mediaCapture = new GameObject("MediaCapture").AddComponent<MediaCapture>();

            // Setup theme
            _timer.GetTheme().SetToLightMode(false);
            _timer.MCDisableDarkModeToggle();

            // Chain screenshot scenarios
            _screenshotScenarios.Enqueue(() => TakeSetupScreenshot(mediaCapture));
            _screenshotScenarios.Enqueue(() => TakeRunningScreenshot(mediaCapture));
            _screenshotScenarios.Enqueue(() => TakeCompletedScreenshot(mediaCapture));
            _screenshotScenarios.Enqueue(() => TakeBreakScreenshot(mediaCapture));
            _screenshotScenarios.Enqueue(() => TakeSidebarScreenshot(mediaCapture));
            _screenshotScenarios.Enqueue(() => TakeSelectionSetupScreenshot(mediaCapture));
            _screenshotScenarios.Enqueue(() => TakeSettingScreenshot(mediaCapture));
            _screenshotScenarios.Enqueue(() => TakeAboutScreenshot(mediaCapture));
            _screenshotScenarios.Enqueue(() => TakeRunningPopupScreenshot(mediaCapture));
            
            // Cache queue for 2nd pass intended for dark mode screenshots
            _ssCopy = new Queue<Action>(_screenshotScenarios);

            // Begin media capture
            MoveToNextScreenshotScenario();
        }

        private static void MoveToNextScreenshotScenario()
        {
            if (_screenshotScenarios.Count > 0)
            {
                _screenshotScenarios.Dequeue().Invoke();
                _screenshotIndex++;
            }
            else
            {
                if (!_hasDarkModeBeenCaptured)
                {
                    // Dirty flag
                    _hasDarkModeBeenCaptured = true;

                    // Clean up
                    _timer.SwitchState(PomodoroTimer.States.SETUP);
                    TimeSpan timeSpan = new TimeSpan(0,0,25,0,0);
                    _timer.SetCurrentTime((float)timeSpan.TotalSeconds);
                    _timer.GetConfirmDialogManager().GetCurrentConfirmationDialog().Close();
                    
                    // Moved cached copy back into our scenarios
                    _screenshotScenarios = _ssCopy;
                    
                    // Swap theme
                    _timer.GetTheme().SetToDarkMode(false);
                    _timer.MCEnableDarkModeToggle();
                    
                    // Begin media capture for dark mode
                    MoveToNextScreenshotScenario();
                }
                else
                {
                    _timer.Restart(false);
                    _timer.GetConfirmDialogManager().GetCurrentConfirmationDialog().Close();

                    if (_startingThemeDarkMode)
                    {
                        _timer.GetTheme().SetToDarkMode(false);
                        _timer.MCEnableDarkModeToggle();
                    }
                    else
                    {
                        _timer.GetTheme().SetToLightMode(false);
                        _timer.MCDisableDarkModeToggle();
                    }
                    _timer.MCEnableThemeToggleAnimation();
                    
                    // Fix tick anim
                    _timer.ShowTickAnimation();

                    Debug.Log("Media Creation Complete!");
                }
            }
        }

        private static void CaptureScreenshot(MediaCapture mediaCapture)
        {
            mediaCapture.CaptureScreenshot("../exports/screenshot-" + _screenshotIndex + ".png",
                MoveToNextScreenshotScenario);
        }
        
        private static void TakeSetupScreenshot(MediaCapture mediaCapture)
        {
            CaptureScreenshot(mediaCapture);
        }

        [ContextMenu("TakeRunningScreenshot")]
        private static void TakeRunningScreenshot(MediaCapture mediaCapture)
        {
            _timer.MCHideCreditsBubble();
            _timer.SwitchState(PomodoroTimer.States.RUNNING);
            _timer.MCShowEndTimestampBubble();
            TimeSpan timeSpan = new TimeSpan(0,0,25,0,0);
            TimeSpan subSpan = new TimeSpan(0,0,3,12,0);
            timeSpan = timeSpan.Subtract(subSpan);
            _timer.SetCurrentTime((float)timeSpan.TotalSeconds);

            CaptureScreenshot(mediaCapture);
        }

        private static void TakeCompletedScreenshot(MediaCapture mediaCapture)
        {
            _timer.SwitchState(PomodoroTimer.States.COMPLETE);
            _timer.MCDisableCompletionAnimation();
            
            CaptureScreenshot(mediaCapture);
        }

        private static void TakeBreakScreenshot(MediaCapture mediaCapture)
        {
            _timer.SwitchTimer(true);
            _timer.MCEnableBreakSlider();
            _timer.SwitchState(PomodoroTimer.States.SETUP);
            
            CaptureScreenshot(mediaCapture);
        }

        private static void TakeSidebarScreenshot(MediaCapture mediaCapture)
        {
            _timer.SwitchTimer(false);
            _timer.MCDisableBreakSlider();
            _timer.SwitchState(PomodoroTimer.States.SETUP);
            _timer.MCShowCreditsBubble();
            _timer.MCShowSidebar();
            
            CaptureScreenshot(mediaCapture);
        }

        private static void TakeSelectionSetupScreenshot(MediaCapture mediaCapture)
        {
            _timer.MCHideCreditsBubble();
            _timer.MCHideSidebar();
            _timer.SelectAll();
            
            CaptureScreenshot(mediaCapture);
        }

        private static void TakeSettingScreenshot(MediaCapture mediaCapture)
        {
            _timer.SetSelection(null); // Clear selection
            _timer.MCShowSettings();
            
            CaptureScreenshot(mediaCapture);
        }

        private static void TakeAboutScreenshot(MediaCapture mediaCapture)
        {
            _timer.MCShowAbout();
            _timer.MCShowCreditsBubble();
            
            CaptureScreenshot(mediaCapture);
        }

        private static void TakeRunningPopupScreenshot(MediaCapture mediaCapture)
        {
            _timer.MCShowMain();
            
            _timer.MCHideCreditsBubble();
            _timer.SwitchState(PomodoroTimer.States.RUNNING);
            _timer.MCShowEndTimestampBubble();
            TimeSpan timeSpan = new TimeSpan(0,0,25,0,0);
            TimeSpan subSpan = new TimeSpan(0,0,3,12,0);
            timeSpan = timeSpan.Subtract(subSpan);
            _timer.SetCurrentTime((float)timeSpan.TotalSeconds);
            _timer.GetConfirmDialogManager().SpawnConfirmationDialog(null);
            _timer.GetConfirmDialogManager().GetCurrentConfirmationDialog().Show();
            
            CaptureScreenshot(mediaCapture);
        }
#endif
    }
}