#if UNITY_EDITOR
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
        private static Queue<Action> _screenshotScenarios = new Queue<Action>();
        private static Queue<Action> _ssCopy = new Queue<Action>(); // Intended to be used for dark mode capture
        private static bool _hasDarkModeBeenCaptured;
        private static int _screenshotIndex;

        [MenuItem("CONTEXT/PomodoroTimer/Create Media")]
        private static void CreateMedia(MenuCommand command)
        {
            // Early Exit
            if (!Application.isPlaying)
            {
                Debug.LogWarning("Please make sure Unity is in play mode and this timer is initialized to" +
                                 "create your media.");
                Debug.Log("No media has been created.");
                return;
            }
            
            // Clear
            _screenshotIndex = 0;
            _hasDarkModeBeenCaptured = false;
            
            // Get reference
            _timer = (PomodoroTimer) command.context;
            
            // Create media capture object
            MediaCapture mediaCapture = new GameObject("MediaCapture").AddComponent<MediaCapture>();

            // Setup theme
            _timer.GetTheme().SetToLightMode();
            _timer.DisableDarkModeToggle();

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
                    _timer.GetTheme().SetToDarkMode();
                    _timer.EnableDarkModeToggle();
                    
                    // Begin media capture for dark mode
                    MoveToNextScreenshotScenario();
                }
                else
                {
                    _timer.Restart(false);
                    _timer.GetConfirmDialogManager().GetCurrentConfirmationDialog().Close();
                    Debug.Log("Media Creation Complete!");
                }
            }
        }

        private static void CaptureScreenshot(MediaCapture mediaCapture)
        {
            mediaCapture.CaptureScreenshot("../promotional/screenshot-" + _screenshotIndex + ".png", 
                MoveToNextScreenshotScenario);
        }
        
        private static void TakeSetupScreenshot(MediaCapture mediaCapture)
        {
            CaptureScreenshot(mediaCapture);
        }

        private static void TakeRunningScreenshot(MediaCapture mediaCapture)
        {
            _timer.HideCreditsBubble();
            _timer.SwitchState(PomodoroTimer.States.RUNNING);
            _timer.ShowEndTimestampBubble();
            TimeSpan timeSpan = new TimeSpan(0,0,25,0,0);
            TimeSpan subSpan = new TimeSpan(0,0,3,12,0);
            timeSpan = timeSpan.Subtract(subSpan);
            _timer.SetCurrentTime((float)timeSpan.TotalSeconds);

            CaptureScreenshot(mediaCapture);
        }

        private static void TakeCompletedScreenshot(MediaCapture mediaCapture)
        {
            _timer.SwitchState(PomodoroTimer.States.COMPLETE);
            _timer.DisableCompletionAnimation();
            
            CaptureScreenshot(mediaCapture);
        }

        private static void TakeBreakScreenshot(MediaCapture mediaCapture)
        {
            _timer.SwitchTimer(true);
            _timer.EnableBreakSlider();
            _timer.SwitchState(PomodoroTimer.States.SETUP);
            
            CaptureScreenshot(mediaCapture);
        }

        private static void TakeSidebarScreenshot(MediaCapture mediaCapture)
        {
            _timer.SwitchTimer(false);
            _timer.DisableBreakSlider();
            _timer.SwitchState(PomodoroTimer.States.SETUP);
            _timer.ShowCreditsBubble();
            _timer.ShowSidebar();
            
            CaptureScreenshot(mediaCapture);
        }

        private static void TakeSelectionSetupScreenshot(MediaCapture mediaCapture)
        {
            _timer.HideCreditsBubble();
            _timer.HideSidebar();
            _timer.SelectAll();
            
            CaptureScreenshot(mediaCapture);
        }

        private static void TakeSettingScreenshot(MediaCapture mediaCapture)
        {
            _timer.SetSelection(null); // Clear selection
            _timer.ShowSettings();
            
            CaptureScreenshot(mediaCapture);
        }

        private static void TakeAboutScreenshot(MediaCapture mediaCapture)
        {
            _timer.ShowAbout();
            _timer.ShowCreditsBubble();
            
            CaptureScreenshot(mediaCapture);
        }

        private static void TakeRunningPopupScreenshot(MediaCapture mediaCapture)
        {
            _timer.ShowMainContent();
            
            _timer.HideCreditsBubble();
            _timer.SwitchState(PomodoroTimer.States.RUNNING);
            _timer.ShowEndTimestampBubble();
            TimeSpan timeSpan = new TimeSpan(0,0,25,0,0);
            TimeSpan subSpan = new TimeSpan(0,0,3,12,0);
            timeSpan = timeSpan.Subtract(subSpan);
            _timer.SetCurrentTime((float)timeSpan.TotalSeconds);
            _timer.GetConfirmDialogManager().SpawnConfirmationDialog(null);
            _timer.GetConfirmDialogManager().GetCurrentConfirmationDialog().Show();
            
            CaptureScreenshot(mediaCapture);
        }
    }
}
#endif