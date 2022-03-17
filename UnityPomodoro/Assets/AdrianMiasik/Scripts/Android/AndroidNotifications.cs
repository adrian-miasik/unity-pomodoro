using System;
using Unity.Notifications.Android;
using UnityEngine;

namespace AdrianMiasik.Android
{
    // TODO: Implement TimerState interface instead
    public class AndroidNotifications : MonoBehaviour
    {
        private int timerNotificationID;
        
        private enum NotificationChannels
        {
            ALARMS
        }

        public void Initialize()
        {
            // Create notification channel for our alarms / timer completions.
            AndroidNotificationChannel channel = new AndroidNotificationChannel
            {
                Id = GetChannelString(NotificationChannels.ALARMS),
                Name = "Alarms (& Timers)",
                Importance = Importance.Default,
                Description = "This channel is used to send notifications on timer completions. " +
                              "This includes both work and break timers.",
            };
            
            // Register notification channel
            AndroidNotificationCenter.RegisterNotificationChannel(channel);

            Debug.Log("Created notification channel: " + GetChannelString(NotificationChannels.ALARMS));
        }

        /// <summary>
        /// Schedules a notification to activate/prompt the user at the provided fire time.
        /// </summary>
        /// <param name="notificationFireTime"></param>
        /// <returns>The scheduled notification's ID. (This ID can be used for modifying the notification if needed)
        /// </returns>
        public int ScheduleTimerNotification(DateTime notificationFireTime)
        {
            AndroidNotification notification = new AndroidNotification
            {
                Title = "Timer Completed!",
                Text = "Notification Text.",
                FireTime = notificationFireTime
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
