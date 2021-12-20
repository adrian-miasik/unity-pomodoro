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
        [SerializeField] private TextAsset xmlToastAlarm;
        [SerializeField] private TextAsset xmlToastNoAlarm;

        // Cache
        private PomodoroTimer timer;

        public void Initialize(PomodoroTimer _timer)
        {
            timer = _timer;
        }

        public void ShowToast()
        {
#if ENABLE_WINMD_SUPPORT
            // When app is not focused...
            if (!UnityEngine.Application.isFocused)
            {
                // And the user wants to mute audio when app is out of focus...
                if (timer.MuteSoundWhenOutOfFocus())
                {
                    // Play Alarm Notification
                    Toast toast = Toast.Create(xmlToastAlarm.text);
                    toast.Show();   
                }
                // Otherwise,  Play No Alarm Notification 
                else{
                    Toast toast = Toast.Create(xmlToastNoAlarm.text);
                    toast.Show();
                }
            }
#endif
        }
    }
}