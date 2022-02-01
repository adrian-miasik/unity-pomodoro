using UnityEditor;
using UnityEngine;

namespace AdrianMiasik.Components.Specific
{
    public static class MediaCreator
    {
        [MenuItem("CONTEXT/PomodoroTimer/Create Media")]
        private static void CreateMedia(MenuCommand command)
        {
            PomodoroTimer timer = (PomodoroTimer) command.context;
            
            timer.SetToLightMode();

            timer.SwitchState(PomodoroTimer.States.SETUP);
            timer.SetTimerValue("00:25:00");
            timer.HideDigitArrows();
            timer.ShowCreditsBubble();
            
            ScreenCapture.CaptureScreenshot("screenshot_0.png");
        }
    }
}