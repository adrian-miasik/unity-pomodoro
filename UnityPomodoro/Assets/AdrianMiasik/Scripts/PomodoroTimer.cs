using System;
using UnityEngine;
using UnityEngine.UI;

namespace AdrianMiasik
{
    public class PomodoroTimer : MonoBehaviour
    {
        [Header("Digits")] 
        [SerializeField] private DoubleDigit hourDigits;
        [SerializeField] private DoubleDigit minuteDigits;
        [SerializeField] private DoubleDigit secondDigits;

        [Header("Buttons")]
        [SerializeField] private GameObject playPauseParent;
        [SerializeField] private PlayPauseButton playPauseButton;

        [Header("Ring")]
        [SerializeField] private Image progress;

        [Header("Colors")] 
        [SerializeField] private Color setupColor = new Color(0.05f, 0.47f, 0.95f);
        [SerializeField] private Color runningColor = new Color(0.35f, 0.89f, 0.4f);
        [SerializeField] private Color completedColor = new Color(0.97f, 0.15f, 0.15f);

        [Header("Data")]
        [SerializeField] private int hours = 0;
        [SerializeField] private int minutes = 0;
        [SerializeField] private int seconds = 15;
        
        // Cache
        private bool _isRunning;
        private double _currentTime;
        private float _totalTime; // In seconds
        private bool _isComplete;
        
        // Specific to our ring shader
        private static readonly int RingColor = Shader.PropertyToID("Color_297012532bf444df807f8743bdb7e4fd");
        
        private void OnValidate()
        {
            // Prevent values from going over their limit
            hours = Mathf.Clamp(hours, 0, 99);
            minutes = Mathf.Clamp(minutes, 0, 59);
            seconds = Mathf.Clamp(seconds, 0, 59);
        }

        private void Start()
        {
            // Initialize components
            hourDigits.Initialize(this);
            minuteDigits.Initialize(this);
            secondDigits.Initialize(this);
            
            playPauseButton.Initialize(this);            
            
            Initialize();
        }

        /// <summary>
        /// Sets up the timer for proper use - Updates visuals, and calculates digits
        /// </summary>
        private void Initialize()
        {
            _isComplete = false;
            playPauseParent.gameObject.SetActive(true);
            
            // Update visuals
            progress.fillAmount = 1f;
            playPauseButton.UpdateIcon();

            // Calculate time
            TimeSpan ts = TimeSpan.FromHours(hours) +  TimeSpan.FromMinutes(minutes) +  TimeSpan.FromSeconds(seconds);
            _currentTime = ts.TotalSeconds;
            _totalTime = (float) ts.TotalSeconds;
            UpdateDigitValues(ts);
        }

        private void UpdateDigitValues(TimeSpan timeSpan)
        {
            hourDigits.SetDigits((int) timeSpan.TotalHours);
            minuteDigits.SetDigits(timeSpan.Minutes);
            secondDigits.SetDigits(timeSpan.Seconds);
        }

        private void Update()
        {
            if (_isRunning)
            {
                if (_currentTime <= 0)
                {
                    _isRunning = false;
                    
                    Complete();
                    progress.material.SetColor(RingColor, completedColor);
                    
                    // Early exit
                    return;
                }

                progress.fillAmount = (float) _currentTime / _totalTime;
                UpdateDigitValues(TimeSpan.FromSeconds(_currentTime));
                _currentTime -= Time.deltaTime;
            }
        }

        /// <summary>
        /// Marks timer as complete, fills progress to full, and updates play/pause visuals
        /// </summary>
        private void Complete()
        {
            _isComplete = true;
            progress.fillAmount = 1f;
            
            playPauseParent.gameObject.SetActive(false);
        }

        public void Play()
        {
            // if (_isComplete) // Hitting play on a completed timer will restart the clock
            // {
            //     Restart();
            // }
            
            // Run timer
            _isRunning = true;
            progress.material.SetColor(RingColor, runningColor);
            LockEditing();
        }

        private void LockEditing()
        {
            hourDigits.Lock();
            minuteDigits.Lock();
            secondDigits.Lock();
        }

        public void Pause()
        {
            // if (_isComplete) // Hitting pause on a complete timer will re-init the clock
            // {
            //     Initialize();
            // }
            
            // Pause timer
            _isRunning = false;
            progress.material.SetColor(RingColor, setupColor);
        }

        /// <summary>
        /// Unity OnClick
        /// </summary>
        public void Restart()
        {
            Pause();
            Initialize();
            UnlockEditing();
        }

        private void UnlockEditing()
        {
            hourDigits.Unlock();
            minuteDigits.Unlock();
            secondDigits.Unlock();
        }

        // Getter

        public bool IsRunning()
        {
            return _isRunning;
        }

        // Setters
        public void SetHours(string hours)
        {
            this.hours = string.IsNullOrEmpty(hours) ? 0 : int.Parse(hours);
            OnValidate();
            Initialize();
        }
        
        public void SetMinutes(string minutes)
        {
            this.minutes = string.IsNullOrEmpty(minutes) ? 0 : int.Parse(minutes);
            OnValidate();
            Initialize();
        }
        
        public void SetSeconds(string seconds)
        {
            this.seconds = string.IsNullOrEmpty(seconds) ? 0 : int.Parse(seconds);
            OnValidate();
            Initialize();
        } 
    }
}
