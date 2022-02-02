using System;
using AdrianMiasik.Components.Base;
using UnityEditor;
using UnityEngine;

namespace AdrianMiasik.Components.Specific
{
    public class MediaCreator: MonoBehaviour
    {
        private static PomodoroTimer m_timer;
        
        [MenuItem("CONTEXT/PomodoroTimer/Create Media")]
        private static void CreateMedia(MenuCommand command)
        {
            // Get reference
            m_timer = (PomodoroTimer) command.context;
            
            // Create media capture object
            MediaCapture mediaCapture = new GameObject("MediaCapture").AddComponent<MediaCapture>();
            
            // Chain screenshot queue
            // TODO: Create a queue system
            TakeSetupScreenshot(mediaCapture, () =>
            {
                TakeRunningScreenshot(mediaCapture, () =>
                {
                    TakeCompletedScreenshot(mediaCapture, (() =>
                    {
                        TakeBreakScreenshot(mediaCapture, () =>
                        {
                            TakeSidebarScreenshot(mediaCapture, () =>
                            {
                                TakeSelectionSetupScreenshot(mediaCapture, () =>
                                {
                                    TakeSettingScreenshot(mediaCapture, () =>
                                    {
                                        TakeAboutScreenshot(mediaCapture, () =>
                                        {
                                            TakeRunningPopupScreenshot(mediaCapture, MediaCleanup);
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
        
        private static void TakeSetupScreenshot(MediaCapture mediaCapture, Action nextAction)
        {
            mediaCapture.CaptureScreenshot("../promotional/screenshot_0.png", nextAction);
        }

        private static void TakeRunningScreenshot(MediaCapture mediaCapture, Action nextAction)
        {
            m_timer.HideCreditsBubble();
            m_timer.SwitchState(PomodoroTimer.States.RUNNING);
            m_timer.ShowEndTimestampBubble();
            TimeSpan timeSpan = new TimeSpan(0,0,25,0,0);
            TimeSpan subSpan = new TimeSpan(0,0,3,12,0);
            timeSpan = timeSpan.Subtract(subSpan);
            m_timer.SetCurrentTime((float)timeSpan.TotalSeconds);

            mediaCapture.CaptureScreenshot("../promotional/screenshot_1.png", nextAction);
        }

        private static void TakeCompletedScreenshot(MediaCapture mediaCapture, Action nextAction)
        {
            m_timer.SwitchState(PomodoroTimer.States.COMPLETE);
            m_timer.DisableCompletionAnimation();
            
            mediaCapture.CaptureScreenshot("../promotional/screenshot_2.png", nextAction);
        }

        private static void TakeBreakScreenshot(MediaCapture mediaCapture, Action nextAction)
        {
            m_timer.SwitchTimer(true);
            m_timer.EnableBreakSlider();
            m_timer.SwitchState(PomodoroTimer.States.SETUP);
            
            mediaCapture.CaptureScreenshot("../promotional/screenshot_3.png", nextAction);
        }

        private static void TakeSidebarScreenshot(MediaCapture mediaCapture, Action nextAction)
        {
            m_timer.SwitchTimer(false);
            m_timer.DisableBreakSlider();
            m_timer.SwitchState(PomodoroTimer.States.SETUP);
            m_timer.ShowCreditsBubble();
            m_timer.ShowSidebar();
            
            mediaCapture.CaptureScreenshot("../promotional/screenshot_4.png", nextAction);
        }

        private static void TakeSelectionSetupScreenshot(MediaCapture mediaCapture, 
            Action nextAction)
        {
            m_timer.HideCreditsBubble();
            m_timer.HideSidebar();
            m_timer.SelectAll();
            
            mediaCapture.CaptureScreenshot("../promotional/screenshot_5.png", nextAction);
        }

        private static void TakeSettingScreenshot(MediaCapture mediaCapture, Action nextAction)
        {
            m_timer.SetSelection(null); // Clear selection
            m_timer.ShowSettings();
            
            mediaCapture.CaptureScreenshot("../promotional/screenshot_6.png", nextAction);
        }

        private static void TakeAboutScreenshot(MediaCapture mediaCapture, Action nextAction)
        {
            m_timer.ShowAbout();
            m_timer.ShowCreditsBubble();
            
            mediaCapture.CaptureScreenshot("../promotional/screenshot_7.png", nextAction);
        }

        private static void TakeRunningPopupScreenshot(MediaCapture mediaCapture, 
            Action nextAction)
        {
            m_timer.ShowMainContent();
            
            m_timer.HideCreditsBubble();
            m_timer.SwitchState(PomodoroTimer.States.RUNNING);
            m_timer.ShowEndTimestampBubble();
            TimeSpan timeSpan = new TimeSpan(0,0,25,0,0);
            TimeSpan subSpan = new TimeSpan(0,0,3,12,0);
            timeSpan = timeSpan.Subtract(subSpan);
            m_timer.SetCurrentTime((float)timeSpan.TotalSeconds);
            m_timer.SpawnConfirmationDialog(null);
            m_timer.GetCurrentConfirmationDialog().Show();
            
            mediaCapture.CaptureScreenshot("../promotional/screenshot_8.png", nextAction);
        }
    }
}