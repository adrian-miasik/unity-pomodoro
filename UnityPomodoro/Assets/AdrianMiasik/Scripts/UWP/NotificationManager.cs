using UnityEngine;
#if ENABLE_WINMD_SUPPORT
using UnityEngine.WSA;
#endif

namespace AdrianMiasik.UWP
{
    // TODO: Remove PomodoroTimer dependency
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
            // When app is not focused: muted audio, play alarm.
            if (timer.MuteSoundWhenOutOfFocus())
            {
                // Alarm
                Toast toast = Toast.Create(xmlToastAlarm.text);
                toast.Show();   
            }
            // Otherwise: unmuted audio, no alarm sound.
            else{
                // No alarm
                Toast toast = Toast.Create(xmlToastNoAlarm.text);
                toast.Show();
            }
#endif
        }
    }
}