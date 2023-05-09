using UnityEngine;
#if UNITY_WSA
using AdrianMiasik.Components.Core.Settings;
using UnityEngine.WSA;
#endif

namespace AdrianMiasik.UWP
{
    /// <summary>
    /// Used to serve Windows OS level notification/toasts to the user. (using the referenced components)
    /// Intended to be invoked via UnityEvent <see cref="PomodoroTimer.m_onTimerCompletion"/>.
    /// </summary>
    public class UWPNotifications : MonoBehaviour
    {
#if UNITY_WSA
        // UWP
        [Header("Toast")]
        [SerializeField] private TextAsset m_xmlToastAlarm;
        [SerializeField] private TextAsset m_xmlToastNoAlarm;
        
        // Cache
        private SystemSettings settings;

        public void Initialize(SystemSettings systemSettings)
        {
            settings = systemSettings;
        }

        /// <summary>
        /// Serves a windows notification toast to the user.
        /// <remarks>UnityEvent used by <see cref="PomodoroTimer.m_onTimerCompletion"/>.</remarks>
        /// </summary>
        public void ShowToast()
        {
            // When app is not focused...(we only want to show prompts if the app is not in focus)
            if (UnityEngine.Application.isFocused)
            {
                return;
            }

            // And the user wants to mute audio when app is out of focus...
            if (settings.m_muteSoundWhenOutOfFocus)
            {
                // Play Alarm Notification
                Toast toast = Toast.Create(m_xmlToastAlarm.text);
                toast.Show();   
            }
            // Otherwise, Play No Alarm Notification 
            else
            {
                Toast toast = Toast.Create(m_xmlToastNoAlarm.text);
                toast.Show();
            }
        }
#endif
    }
}