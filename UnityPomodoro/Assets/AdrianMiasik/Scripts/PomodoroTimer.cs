using UnityEngine;
using UnityEngine.UI;

namespace AdrianMiasik
{
    // TODO: Remove debug logs
    public class PomodoroTimer : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button playPause;
        [SerializeField] private Button reset;

        [SerializeField] private Image progress;

        // TODO: Get correct color palette
        // TODO: Pass colors to ring shader
        [Header("Colors")]
        [SerializeField] private Color setupColor = Color.blue;
        [SerializeField] private Color runningColor = Color.green;
        [SerializeField] private Color completedColor = Color.red;
        
        public float startingTime = 15f; // in seconds

        // Cache
        private bool isRunning = false;
        private float currentTime = 0;

        private void Start()
        {
            progress.color = setupColor;
        }

        private void Update()
        {
            if (isRunning)
            {
                if (currentTime <= 0)
                {
                    Debug.Log("Timer is complete");
                    isRunning = false;
                    progress.fillAmount = 1f;
                    progress.color = completedColor;
                }
                
                progress.fillAmount = currentTime / startingTime;
                currentTime -= Time.deltaTime;
            }
        }

        public void OnClickPlayPause()
        {
            Debug.Log("On click play pause");
            
            // Play button
            if (!isRunning)
            {
                currentTime = startingTime;
                isRunning = true;
                progress.color = runningColor;
                // TODO: swap button sprite to pause
            }
            // Pause button
            else
            {   
                // TODO: swap button sprite to play
            }
        }
        
        public void OnClickReset()
        {
            Debug.Log("On click reset");
            if (isRunning)
            {
                currentTime = startingTime;
            }
        }
    }
}
