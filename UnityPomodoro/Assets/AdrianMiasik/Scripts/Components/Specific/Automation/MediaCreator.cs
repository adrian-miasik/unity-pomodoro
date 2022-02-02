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
            
            // Get reference
            _timer = (PomodoroTimer) command.context;
            
            // Create media capture object
            MediaCapture mediaCapture = new GameObject("MediaCapture").AddComponent<MediaCapture>();
            
            // Setup theme
            _timer.SetToLightMode();
            
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
            }
            else
            {
                if (!_hasDarkModeBeenCaptured)
                {
                    // Dirty flag
                    _hasDarkModeBeenCaptured = true;

                    // Clean up
                    _timer.SwitchState(PomodoroTimer.States.SETUP);
                    _timer.GetCurrentConfirmationDialog().Close();
                    
                    // Moved cached copy back into our scenarios
                    _screenshotScenarios = _ssCopy;
                    
                    // Swap theme
                    _timer.SetToDarkMode();
                    
                    // Begin media capture for dark mode
                    _screenshotScenarios.Dequeue().Invoke();
                }
                else
                {
                    _timer.Restart(false);
                    _timer.GetCurrentConfirmationDialog().Close();
                    Debug.Log("Media Creation Complete!");
                }
            }
        }

        private static void TakeSetupScreenshot(MediaCapture mediaCapture)
        {
            mediaCapture.CaptureScreenshot("../promotional/screenshot_0.png", MoveToNextScreenshotScenario);
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

            mediaCapture.CaptureScreenshot("../promotional/screenshot_1.png", MoveToNextScreenshotScenario);
        }

        private static void TakeCompletedScreenshot(MediaCapture mediaCapture)
        {
            _timer.SwitchState(PomodoroTimer.States.COMPLETE);
            _timer.DisableCompletionAnimation();
            
            mediaCapture.CaptureScreenshot("../promotional/screenshot_2.png", MoveToNextScreenshotScenario);
        }

        private static void TakeBreakScreenshot(MediaCapture mediaCapture)
        {
            _timer.SwitchTimer(true);
            _timer.EnableBreakSlider();
            _timer.SwitchState(PomodoroTimer.States.SETUP);
            
            mediaCapture.CaptureScreenshot("../promotional/screenshot_3.png", MoveToNextScreenshotScenario);
        }

        private static void TakeSidebarScreenshot(MediaCapture mediaCapture)
        {
            _timer.SwitchTimer(false);
            _timer.DisableBreakSlider();
            _timer.SwitchState(PomodoroTimer.States.SETUP);
            _timer.ShowCreditsBubble();
            _timer.ShowSidebar();
            
            mediaCapture.CaptureScreenshot("../promotional/screenshot_4.png", MoveToNextScreenshotScenario);
        }

        private static void TakeSelectionSetupScreenshot(MediaCapture mediaCapture)
        {
            _timer.HideCreditsBubble();
            _timer.HideSidebar();
            _timer.SelectAll();
            
            mediaCapture.CaptureScreenshot("../promotional/screenshot_5.png", MoveToNextScreenshotScenario);
        }

        private static void TakeSettingScreenshot(MediaCapture mediaCapture)
        {
            _timer.SetSelection(null); // Clear selection
            _timer.ShowSettings();
            
            mediaCapture.CaptureScreenshot("../promotional/screenshot_6.png", MoveToNextScreenshotScenario);
        }

        private static void TakeAboutScreenshot(MediaCapture mediaCapture)
        {
            _timer.ShowAbout();
            _timer.ShowCreditsBubble();
            
            mediaCapture.CaptureScreenshot("../promotional/screenshot_7.png", MoveToNextScreenshotScenario);
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
            _timer.SpawnConfirmationDialog(null);
            _timer.GetCurrentConfirmationDialog().Show();
            
            mediaCapture.CaptureScreenshot("../promotional/screenshot_8.png", MoveToNextScreenshotScenario);
        }
    }
}