using System;
using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif
using UnityEngine;

namespace AdrianMiasik.Android
{
    /// <summary>
    /// Used to serve Android OS level notifications/toasts to the user. Intended to be used with
    /// <see cref="ITimerState"/> to queue notifications when necessary.
    /// </summary>
    public class AndroidNotifications : MonoBehaviour, ITimerState
    {
#if UNITY_ANDROID
        private int timerNotificationID;
        private PomodoroTimer timer;
        
        private enum NotificationChannels
        {
            ALARMS
        }

        /// <summary>
        /// Creates our necessary Android Notification Channels 
        /// </summary>
        /// <param name="pomodoroTimer"></param>
        public void Initialize(PomodoroTimer pomodoroTimer)
        {
            timer = pomodoroTimer;
            
            // Create notification channel for our alarms / timer completions.
            AndroidNotificationChannel channel = new AndroidNotificationChannel
            {
                Id = GetChannelString(NotificationChannels.ALARMS),
                Name = "Timer Completions",
                Importance = Importance.High,
                Description = "This channel is used to send notifications on timer completions. " +
                              "This includes both work and break timers.",
            };
            
            // Register notification channel
            AndroidNotificationCenter.RegisterNotificationChannel(channel);
        }

        /// <summary>
        /// Schedules a notification to activate/prompt the user at the provided fire time.
        /// </summary>
        /// <param name="notificationFireTime"></param>
        /// <param name="titlePrefix"></param>
        /// <returns>The scheduled notification's ID. (This ID can be used for modifying the notification if needed)
        /// </returns>
        private void ScheduleTimerNotification(string titlePrefix, DateTime notificationFireTime)
        {
            string notificationTitle = "Timer Completed!";

            // If a title prefix has been provided...
            if (!string.IsNullOrEmpty(titlePrefix))
            {
                // Set prefix
                string newTitle = titlePrefix + " " + notificationTitle;
                notificationTitle = newTitle;
            }
            
            AndroidNotification notification = new AndroidNotification
            {
                Title = notificationTitle,
                Text = "Your timer is complete! (" + notificationFireTime.ToString("h:mm:ss tt") + ")",
                FireTime = notificationFireTime,
                SmallIcon = "app_favicon",
                LargeIcon = "app_icon"
            };

            timerNotificationID = AndroidNotificationCenter.SendNotification(notification, 
                GetChannelString(NotificationChannels.ALARMS));
        }

        /// <summary>
        /// Cancels and prevents our scheduled notifications from being shown or invoked later.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private void CancelScheduledTimerNotification()
        {
            NotificationStatus notificationStatus = 
                AndroidNotificationCenter.CheckScheduledNotificationStatus(timerNotificationID);

            switch (notificationStatus)
            {
                case NotificationStatus.Unavailable:
                    break;
                case NotificationStatus.Unknown:
                    break;
                case NotificationStatus.Scheduled:
                    AndroidNotificationCenter.CancelScheduledNotification(timerNotificationID);
                    break;
                case NotificationStatus.Delivered:
                    AndroidNotificationCenter.CancelNotification(timerNotificationID);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        /// <summary>
        /// Returns the channel string in lower case sensitivity.
        /// </summary>
        /// <param name="desiredChannel"></param>
        /// <returns></returns>
        private string GetChannelString(NotificationChannels desiredChannel)
        {
            return desiredChannel.ToString().ToLower();
        }

#endif
        /// <summary>
        /// Determines what to do with our notifications based on Timer States. So if we return to setup mode,
        /// we discard our queued notifications, etc...
        /// </summary>
        /// <param name="state"></param>
        /// <param name="theme"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void StateUpdate(PomodoroTimer.States state, Theme theme)
        {
#if UNITY_ANDROID
            switch (state)
            {
                case PomodoroTimer.States.SETUP:
                    CancelScheduledTimerNotification();
                    break;
                
                case PomodoroTimer.States.RUNNING:
                    // Schedule Android Notification when timer ends
                    ScheduleTimerNotification(!timer.IsOnBreak() ? "Work" : "Break", 
                        DateTime.Now.AddSeconds(timer.GetCurrentTime()));
                    break;
                
                case PomodoroTimer.States.PAUSED:
                    CancelScheduledTimerNotification();
                    break;
                
                case PomodoroTimer.States.COMPLETE:
                    // We don't need to show notification when app is in focus.
                    if (Application.isFocused)
                    {
                        CancelScheduledTimerNotification();
                    }
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
#endif
        }
    }
}