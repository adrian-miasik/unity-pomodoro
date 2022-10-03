using UnityEngine;
using UnityEngine.Events;

namespace AdrianMiasik.Components.Specific
{
    /// <summary>
    /// Responsible for detecting when the application window size changes. By default, it checks the current
    /// windows size every second and compares it with its cache to determine if the window has been resized.
    /// If the window has been resized it will fire off an action upon checking.
    /// </summary>
    public class ResolutionDetector : MonoBehaviour
    {
        private PomodoroTimer timer;
        private bool isInitialized;

        private Vector2 cachedResolution;
        private float accumulatedTime;
        private float checkEveryXSeconds = 1f;

        [HideInInspector] public UnityEvent onResolutionChange;

        public void Initialize(PomodoroTimer pomodoroTimer)
        {
            timer = pomodoroTimer;
            isInitialized = true;
            
            // Cache current resolution
            cachedResolution = GetCurrentResolution();
        }

        private void Update()
        {
            if (!isInitialized)
            {
                return;
            }

            accumulatedTime += Time.deltaTime;

            if (accumulatedTime >= checkEveryXSeconds)
            {
                accumulatedTime = 0;

                // Debug.Log("Resolution check");
                
                // Check resolution
                Vector2 currentResolution = GetCurrentResolution();
                if (currentResolution.x != cachedResolution.x || currentResolution.y != cachedResolution.y)
                {
                    // Debug.Log("Resolution has been changed!");
                    onResolutionChange?.Invoke();
                    cachedResolution = currentResolution;
                }
            }
        }

        private Vector2 GetCurrentResolution()
        {
            return new Vector2(Screen.width, Screen.height);
        }
    }
}