using UnityEngine;
#if ENABLE_WINMD_SUPPORT
using UnityEngine.WSA;
#endif

namespace AdrianMiasik.UWP
{
    // TODO: Replace PomodoroTimer dependency with Setting class dependency
    public class NotificationManager : MonoBehaviour
    {
        // UWP
        [Header("Toast")]
        [SerializeField] private TextAsset m_xmlToastAlarm;
        [SerializeField] private TextAsset m_xmlToastNoAlarm;

        // Cache
        private PomodoroTimer timer;

        public void Initialize(PomodoroTimer pomodoroTimer)
        {
            timer = pomodoroTimer;
        }

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