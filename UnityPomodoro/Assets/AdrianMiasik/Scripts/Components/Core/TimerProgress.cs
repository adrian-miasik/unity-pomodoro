using UnityEngine;

namespace AdrianMiasik.Components.Core 
{ 
    public abstract class TimerProgress : MonoBehaviour
    {
        [SerializeField] protected float duration = 3f;
        [SerializeField] private bool loop = false;

        private bool isRunning;
        private bool isInit; // Set this to true to run timer
        private float progress;
        private float elapsedTime;
        private bool hasCompleted;

        protected void Initialize(float timerDuration)
        {
            duration = timerDuration;
            isInit = true;
            hasCompleted = false;
            isRunning = true;
        }

        protected void Restart()
        {
            Initialize(duration);
            elapsedTime = 0;
        }
        
        /// <summary>
        /// Update loop for timer
        /// </summary>
        /// <param name="progress">A progress value between 0 to 1</param>
        protected abstract void OnUpdate(float progress);
        
        /// <summary>
        /// Invoked when the timer has completed
        /// </summary>
        protected abstract void OnComplete();
        
        protected virtual void Update()
        {
            if (!isInit || hasCompleted && !loop)
            {
                return;
            }
        
            elapsedTime += Time.deltaTime;
            progress = elapsedTime / duration;
            OnUpdate(progress);

            if (elapsedTime >= duration)
            {
                OnComplete();
                elapsedTime = 0;
                hasCompleted = true;
                isRunning = false;
            }
        }

        public bool IsRunning()
        {
            return isRunning;
        }
    }
}
