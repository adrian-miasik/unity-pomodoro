using System;
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
            // Get reference
            PomodoroTimer timer = (PomodoroTimer) command.context;
            
            // Create media capture object
            MediaCapture mediaCapture = new GameObject("MediaCapture").AddComponent<MediaCapture>();
            
            // Chain screenshot queue
            // TODO: Create a queue system
            TakeSetupScreenshot(timer, mediaCapture, () =>
            {
                TakeRunningScreenshot(timer, mediaCapture, () =>
                {
                    TakeCompletedScreenshot(timer, mediaCapture, (() =>
                    {
                        TakeBreakScreenshot(timer, mediaCapture, () =>
                        {
                            TakeSidebarScreenshot(timer, mediaCapture, () =>
                            {
                                TakeSelectionSetupScreenshot(timer, mediaCapture, () =>
                                {
                                    TakeSettingScreenshot(timer, mediaCapture, () =>
                                    {
                                        TakeAboutScreenshot(timer, mediaCapture, () =>
                                        {
                                            TakeRunningPopupScreenshot(timer, mediaCapture, MediaCleanup);
                                        });
                                    });
                                });
                            });
                        });
                    }));
                });
            });
        }
        
        private static void MediaCleanup()
        {
            
        }
        
        private static void TakeSetupScreenshot(PomodoroTimer timer, MediaCapture mediaCapture, Action nextAction)
        {
            mediaCapture.CaptureScreenshot("../promotional/screenshot_0.png", nextAction);
        }

        private static void TakeRunningScreenshot(PomodoroTimer timer, MediaCapture mediaCapture, Action nextAction)
        {
            timer.HideCreditsBubble();
            timer.SwitchState(PomodoroTimer.States.RUNNING);
            timer.ShowEndTimestampBubble();
            TimeSpan timeSpan = new TimeSpan(0,0,25,0,0);
            TimeSpan subSpan = new TimeSpan(0,0,3,12,0);
            timeSpan = timeSpan.Subtract(subSpan);
            timer.SetCurrentTime((float)timeSpan.TotalSeconds);

            mediaCapture.CaptureScreenshot("../promotional/screenshot_1.png", nextAction);
        }

        private static void TakeCompletedScreenshot(PomodoroTimer timer, MediaCapture mediaCapture, Action nextAction)
        {
            timer.SwitchState(PomodoroTimer.States.COMPLETE);
            timer.DisableCompletionAnimation();
            
            mediaCapture.CaptureScreenshot("../promotional/screenshot_2.png", nextAction);
        }

        private static void TakeBreakScreenshot(PomodoroTimer timer, MediaCapture mediaCapture, Action nextAction)
        {
            timer.SwitchTimer(true);
            timer.EnableBreakSlider();
            timer.SwitchState(PomodoroTimer.States.SETUP);
            
            mediaCapture.CaptureScreenshot("../promotional/screenshot_3.png", nextAction);
        }

        private static void TakeSidebarScreenshot(PomodoroTimer timer, MediaCapture mediaCapture, Action nextAction)
        {
            timer.SwitchTimer(false);
            timer.DisableBreakSlider();
            timer.SwitchState(PomodoroTimer.States.SETUP);
            timer.ShowCreditsBubble();
            timer.ShowSidebar();
            
            mediaCapture.CaptureScreenshot("../promotional/screenshot_4.png", nextAction);
        }

        private static void TakeSelectionSetupScreenshot(PomodoroTimer timer, MediaCapture mediaCapture, 
            Action nextAction)
        {
            timer.HideCreditsBubble();
            timer.HideSidebar();
            timer.SelectAll();
            
            mediaCapture.CaptureScreenshot("../promotional/screenshot_5.png", nextAction);
        }

        private static void TakeSettingScreenshot(PomodoroTimer timer, MediaCapture mediaCapture, Action nextAction)
        {
            timer.SetSelection(null); // Clear selection
            timer.ShowSettings();
            
            mediaCapture.CaptureScreenshot("../promotional/screenshot_6.png", nextAction);
        }

        private static void TakeAboutScreenshot(PomodoroTimer timer, MediaCapture mediaCapture, Action nextAction)
        {
            timer.ShowAbout();
            timer.ShowCreditsBubble();
            
            mediaCapture.CaptureScreenshot("../promotional/screenshot_7.png", nextAction);
        }

        private static void TakeRunningPopupScreenshot(PomodoroTimer timer, MediaCapture mediaCapture, 
            Action nextAction)
        {
            timer.ShowMainContent();
            
            timer.HideCreditsBubble();
            timer.SwitchState(PomodoroTimer.States.RUNNING);
            timer.ShowEndTimestampBubble();
            TimeSpan timeSpan = new TimeSpan(0,0,25,0,0);
            TimeSpan subSpan = new TimeSpan(0,0,3,12,0);
            timeSpan = timeSpan.Subtract(subSpan);
            timer.SetCurrentTime((float)timeSpan.TotalSeconds);
            timer.SpawnConfirmationDialog(null);
            timer.GetCurrentConfirmationDialog().Show();
            
            mediaCapture.CaptureScreenshot("../promotional/screenshot_8.png", nextAction);
        }
    }
}