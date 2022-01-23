using UnityEngine;

namespace AdrianMiasik.Components.Base 
{ 
    /// <summary>
    /// An abstract **base** timer class that is intended to be used on any component/class that needs something to
    /// happen after `X` couple seconds. Supports duration, and the ability to loop.
    /// </summary>
    public abstract class TimerProgress : MonoBehaviour
    {
        /// <summary>
        /// How long should this timer run for? (In seconds)
        /// </summary>
        [SerializeField] protected float m_duration = 3f;
        
        /// <summary>
        /// Should this timer loop on completion?
        /// <remarks>If you enable this via the Inspector, the timer will reactivate on next timer iteration.</remarks>
        /// </summary>
        [SerializeField] private bool m_loop;
        
        private bool isInitialized;
        private bool isRunning; // Set this to true to run timer
        private float progress; // A float between 0-1
        private float elapsedTime; // How long in seconds?
        private bool hasCompleted; // Has this timer completed at least once?

        /// <summary>
        /// Sets up and starts the timer using the provided preferences.
        /// </summary>
        /// <param name="timerDuration">How long should this timer run for?</param>
        /// <param name="loop">Should the timer repeat on completion?</param>
        protected void Initialize(float timerDuration, bool loop = false)
        {
            m_duration = timerDuration;
            m_loop = loop;
            
            // Defaults
            elapsedTime = 0;
            hasCompleted = false;
            isRunning = true;
            isInitialized = true;
            
            OnStart();
        }

        private void OnValidate()
        {
            // Start again, if user enables loop via Inspector
            if (m_loop)
            {
                Restart();
            }
        }
        
        /// <summary>
        /// Restarts the timer and immediately starts the timer.
        /// </summary>
        protected void Restart()
        {
            Initialize(m_duration, m_loop);
        }

        /// <summary>
        /// Invoked on timer start / component initialization.
        /// <remarks>Gets invoked on every timer loop too.</remarks>
        /// </summary>
        protected abstract void OnStart();
        
        /// <summary>
        /// Update loop for timer.
        /// </summary>
        /// <param name="progress">A progress value between 0 to 1</param>
        protected abstract void OnUpdate(float progress);
        
        /// <summary>
        /// Invoked when the timer has completed.
        /// </summary>
        protected abstract void OnComplete();
        
        /// <summary>
        /// Update/tick method for timer progression.
        /// </summary>
        protected virtual void Update()
        {
            if (isInitialized)
            {
                if (isRunning)
                {
                    progress = elapsedTime / m_duration;
                    elapsedTime += Time.deltaTime;
                    
                    // Update tick
                    OnUpdate(progress);
                    
                    // Exit condition
                    if (elapsedTime >= m_duration)
                    {
                        OnComplete();

                        // Reset time
                        elapsedTime = 0;

                        // Stop timer
                        isRunning = false;

                        // Raise flag
                        hasCompleted = true;
                        
                        if (m_loop)
                        {
                            Restart();
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Returns the active running state of the timer.
        /// <remarks>This turns to false on timer completion.</remarks>
        /// </summary>
        /// <returns>Is timer updating/ticking/running right now?</returns>
        public bool IsRunning()
        {
            return isRunning;
        }
    }
}