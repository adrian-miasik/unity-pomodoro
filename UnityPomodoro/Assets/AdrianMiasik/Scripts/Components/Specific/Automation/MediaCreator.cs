using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AdrianMiasik.Components.Specific.Automation
{
    public class MediaCreator: MonoBehaviour
    {
        private static PomodoroTimer _timer;
        private static readonly Queue<Action> ScreenshotScenarios = new Queue<Action>();

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
            
            // Chain screenshot scenarios
            ScreenshotScenarios.Enqueue(() => TakeSetupScreenshot(mediaCapture));
            ScreenshotScenarios.Enqueue(() => TakeRunningScreenshot(mediaCapture));
            ScreenshotScenarios.Enqueue(() => TakeCompletedScreenshot(mediaCapture));
            ScreenshotScenarios.Enqueue(() => TakeBreakScreenshot(mediaCapture));
            ScreenshotScenarios.Enqueue(() => TakeSidebarScreenshot(mediaCapture));
            ScreenshotScenarios.Enqueue(() => TakeSelectionSetupScreenshot(mediaCapture));
            ScreenshotScenarios.Enqueue(() => TakeSettingScreenshot(mediaCapture));
            ScreenshotScenarios.Enqueue(() => TakeAboutScreenshot(mediaCapture));
            ScreenshotScenarios.Enqueue(() => TakeRunningPopupScreenshot(mediaCapture));

            // Begin media capture
            MoveToNextScreenshotScenario();
        }

        private static void MoveToNextScreenshotScenario()
        {
            if (ScreenshotScenarios.Count > 0)
            {
                ScreenshotScenarios.Dequeue().Invoke();
            }
            else
            {
                _timer.Restart(false);
                _timer.GetCurrentConfirmationDialog().Close();
                Debug.Log("Media Creation Complete!");
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