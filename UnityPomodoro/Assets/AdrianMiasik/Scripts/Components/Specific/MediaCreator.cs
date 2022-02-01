using System;
using System.Collections;
using AdrianMiasik.Components.Base;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace AdrianMiasik.Components.Specific
{
    public class MediaCreator: MonoBehaviour
    {
        [MenuItem("CONTEXT/PomodoroTimer/Create Media")]
        private static void CreateMedia(MenuCommand command)
        {
            // Get reference
            PomodoroTimer timer = (PomodoroTimer) command.context;
            
            // Create media capture object
            MediaCapture mediaCapture = new GameObject("MediaCapture").AddComponent<MediaCapture>();
            
            // Chain screenshot queue
            TakeSetupScreenshot(timer, mediaCapture, () =>
            {
                TakeRunningScreenshot(timer, mediaCapture, () =>
                {
                    TakeCompletedScreenshot(timer, mediaCapture);
                });
            });
        }

        private static void TakeSetupScreenshot(PomodoroTimer timer, MediaCapture mediaCapture, Action nextAction)
        {
            mediaCapture.CaptureScreenshot("../promotional/screenshot_0.png", nextAction);
        }

        private static void TakeRunningScreenshot(PomodoroTimer timer, MediaCapture mediaCapture, Action nextAction)
        {
            // Clean up
            timer.HideCreditsBubble();
            
            // Setup second screenshot
            timer.SwitchState(PomodoroTimer.States.RUNNING);
            timer.ShowEndTimestampBubble();
            
            TimeSpan timeSpan = new TimeSpan(0,0,25,0,0);
            TimeSpan subSpan = new TimeSpan(0,0,3,12,0);
            timeSpan = timeSpan.Subtract(subSpan);
            timer.SetCurrentTime((float)timeSpan.TotalSeconds);

            mediaCapture.CaptureScreenshot("../promotional/screenshot_1.png", nextAction);
        }

        private static void TakeCompletedScreenshot(PomodoroTimer timer, MediaCapture mediaCapture)
        {
            timer.SwitchState(PomodoroTimer.States.COMPLETE);
            timer.DisableCompletionAnimation();
            
            mediaCapture.CaptureScreenshot("../promotional/screenshot_2.png", null);
        }
    }
}