using AdrianMiasik.Components;
using AdrianMiasik.Components.Base;
using AdrianMiasik.ScriptableObjects;
using UnityEngine;
#if ENABLE_WINMD_SUPPORT
using UnityEngine.WSA;
#endif

namespace AdrianMiasik.UWP
{
    // TODO: Replace PomodoroTimer dependency with Setting class dependency
    /// <summary>
    /// Used to serve Windows OS level notification/toasts to the user. (using the referenced components)
    /// Intended to be invoked via UnityEvent <see cref="PomodoroTimer.m_onTimerCompletion"/>.
    /// </summary>
    public class NotificationManager : MonoBehaviour
    {
        // UWP
        [Header("Toast")]
        [SerializeField] private TextAsset m_xmlToastAlarm;
        [SerializeField] private TextAsset m_xmlToastNoAlarm;

        // Cache
        private PomodoroTimer timer;

        /// <summary>
        /// Sets up the timer reference. Not a <see cref="ThemeElement"/> since this doesn't have visuals
        /// and thus doesn't need a <see cref="Theme"/>.
        /// </summary>
        /// <param name="pomodoroTimer"></param>
        public void Initialize(PomodoroTimer pomodoroTimer)
        {
            timer = pomodoroTimer;
        }

        /// <summary>
        /// Serves a windows notification toast to the user.
        /// <remarks>UnityEvent used by <see cref="PomodoroTimer.m_onTimerCompletion"/>.</remarks>
        /// </summary>
        public void ShowToast()
        {
#if ENABLE_WINMD_SUPPORT
            // When app is not focused...(we only want to show prompts if the app is not in focus)
            if (UnityEngine.Application.isFocused)
            {
                return;
            }

            // And the user wants to mute audio when app is out of focus...
            if (timer.MuteSoundWhenOutOfFocus())
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
#endif
        }
    }
}