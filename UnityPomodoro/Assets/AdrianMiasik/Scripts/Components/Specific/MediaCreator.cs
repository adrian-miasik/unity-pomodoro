using System.Collections;
using AdrianMiasik.Components.Base;
using UnityEditor;
using UnityEngine;

namespace AdrianMiasik.Components.Specific
{
    public class MediaCreator: MonoBehaviour
    {
        [MenuItem("CONTEXT/PomodoroTimer/Create Media")]
        private static void CreateMedia(MenuCommand command)
        {
            // Create screenshot capturer
            ScreenshotCapturer ssCapturer = new GameObject("ScreenshotCapturer").AddComponent<ScreenshotCapturer>();
            
            // Get reference
            PomodoroTimer timer = (PomodoroTimer) command.context;

            // Setup theme
            timer.SetToLightMode();

            // Setup first screenshot
            timer.SwitchState(PomodoroTimer.States.SETUP);
            timer.SetTimerValue("00:25:00");
            timer.HideDigitArrows();
            timer.ShowCreditsBubble();
            
            // Capture first screenshot
            ssCapturer.CaptureScreenshot("screenshot_0.png", 1);
            
            // TODO: Set timer to 25:00 / 21:48 and pause while running (might have to time scale)
        }
    }
}