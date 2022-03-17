using System;
using System.Linq;
using Unity.Notifications.Android;
using UnityEngine;

namespace AdrianMiasik.Android
{
    // TODO: Implement TimerState interface instead
    public class AndroidNotifications : MonoBehaviour
    {
        private int timerNotificationID;
        private PomodoroTimer timer;
        
        private enum NotificationChannels
        {
            ALARMS
        }

        public void Initialize(PomodoroTimer pomodoroTimer)
        {
            timer = pomodoroTimer;
            
            // Create notification channel for our alarms / timer completions.
            AndroidNotificationChannel channel = new AndroidNotificationChannel
            {
                Id = GetChannelString(NotificationChannels.ALARMS),
                Name = "Alarms (& Timers)",
                Importance = Importance.High,
                Description = "This channel is used to send notifications on timer completions. " +
                              "This includes both work and break timers.",
            };
            
            // Register notification channel
            AndroidNotificationCenter.RegisterNotificationChannel(channel);

            Debug.Log("Created notification channel: " + GetChannelString(NotificationChannels.ALARMS));
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus)
            {
                AndroidNotificationIntentData intentData = AndroidNotificationCenter.GetLastNotificationIntent();

                if (intentData != null)
                {
                    timer.Skip();
                }
            }
        }

        /// <summary>
        /// Schedules a notification to activate/prompt the user at the provided fire time.
        /// </summary>
        /// <param name="notificationFireTime"></param>
        /// <param name="titlePrefix"></param>
        /// <returns>The scheduled notification's ID. (This ID can be used for modifying the notification if needed)
        /// </returns>
        public int ScheduleTimerNotification(string titlePrefix, DateTime notificationFireTime)
        {
            string notificationTitle = "Timer Completed!";

            // If a title prefix has been provided...
            if (!string.IsNullOrEmpty(titlePrefix))
            {
                // Set prefix
                string newTitle = titlePrefix + " " + notificationTitle;
                notificationTitle = newTitle;
            }
            
            // TODO: When notification is clicked...return to app in completed state.
            AndroidNotification notification = new AndroidNotification
            {
                Title = notificationTitle,
                Text = "Your timer is complete! (" + notificationFireTime.ToString("h:mm:ss tt") + ")",
                FireTime = notificationFireTime,
                LargeIcon = "../../Sprites/icon-unity-pomodoro.png"
            };

            timerNotificationID = AndroidNotificationCenter.SendNotification(notification, 
                GetChannelString(NotificationChannels.ALARMS));

            return timerNotificationID;
        }

        public void CancelScheduledTimerNotification()
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
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private string GetChannelString(NotificationChannels desiredChannel)
        {
            return desiredChannel.ToString().ToLower();
        }
    }
}
