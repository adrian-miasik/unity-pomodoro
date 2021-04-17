using System;
using UnityEngine;
using UnityEngine.UI;

namespace AdrianMiasik
{
    public class PomodoroTimer : MonoBehaviour
    {
        // TODO: Create digit layouts
        // TODO: Support more timer digit layouts
        public enum Digits
        {
            HOURS,
            MINUTES,
            SECONDS
        }

        [Header("Digits")] [SerializeField] private DoubleDigit hourDigits;
        [SerializeField] private DoubleDigit minuteDigits;
        [SerializeField] private DoubleDigit secondDigits;

        [Header("Buttons")] [SerializeField] private GameObject playPauseParent;
        [SerializeField] private PlayPauseButton playPauseButton;

        [Header("Ring")] [SerializeField] private Image progress;

        [Header("Colors")] [SerializeField] private Color setupColor = new Color(0.05f, 0.47f, 0.95f);
        [SerializeField] private Color runningColor = new Color(0.35f, 0.89f, 0.4f);
        [SerializeField] private Color completedColor = new Color(0.97f, 0.15f, 0.15f);

        [Header("Data")] [SerializeField] private int hours = 0;
        [SerializeField] private int minutes = 0;
        [SerializeField] private int seconds = 15;

        // Cache
        private bool _isRunning;
        private double _currentTime;
        private float _totalTime; // In seconds


        // Specific to our ring shader
        private static readonly int RingColor = Shader.PropertyToID("Color_297012532bf444df807f8743bdb7e4fd");

        private void OnValidate()
        {
            // Prevent values from going over their limit
            hours = Mathf.Clamp(hours, GetDigitMin(), GetDigitMax(Digits.HOURS));
            minutes = Mathf.Clamp(minutes, GetDigitMin(), GetDigitMax(Digits.MINUTES));
            seconds = Mathf.Clamp(seconds, GetDigitMin(), GetDigitMax(Digits.SECONDS));
        }

        private void Start()
        {
            // Initialize digits
            TimeSpan ts = TimeSpan.FromHours(hours) + TimeSpan.FromMinutes(minutes) + TimeSpan.FromSeconds(seconds);
            hourDigits.Initialize(Digits.HOURS, this, (int) ts.TotalHours);
            minuteDigits.Initialize(Digits.MINUTES, this, ts.Minutes);
            secondDigits.Initialize(Digits.SECONDS, this, ts.Seconds);

            // Initialize button
            playPauseButton.Initialize(this);

            Initialize();
        }

        /// <summary>
        /// Sets up the timer for proper use - Updates visuals, and calculates digits
        /// </summary>
        private void Initialize()
        {
            playPauseParent.gameObject.SetActive(true);

            // Update visuals
            progress.fillAmount = 1f;
            playPauseButton.UpdateIcon();

            // Calculate time
            TimeSpan ts = TimeSpan.FromHours(hours) + TimeSpan.FromMinutes(minutes) + TimeSpan.FromSeconds(seconds);
            _currentTime = ts.TotalSeconds;
            _totalTime = (float) ts.TotalSeconds;
            UpdateDigitValues(ts);

            UnlockEditing();
        }

        private void UpdateDigitValues(TimeSpan timeSpan)
        {
            hourDigits.SetDigitsLabel((int) timeSpan.TotalHours);
            minuteDigits.SetDigitsLabel(timeSpan.Minutes);
            secondDigits.SetDigitsLabel(timeSpan.Seconds);
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
            progress.fillAmount = 1f;
            playPauseParent.gameObject.SetActive(false);
        }

        /// <summary>
        /// Runs the pomodoro countdown timer
        /// </summary>
        public void Play()
        {
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

        /// <summary>
        /// Prevents the timer from running
        /// </summary>
        public void Pause()
        {
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

        public void IncrementOne(Digits digits)
        {
            SetDigit(digits, GetDigitValue(digits) + 1);
        }

        public bool CanIncrementOne(Digits digits)
        {
            if (GetDigitValue(digits) + 1 > GetDigitMax(digits))
            {
                return false;
            }

            return true;
        }
        
        public bool CanDecrementOne(Digits digits)
        {
            if (GetDigitValue(digits) - 1 < GetDigitMin())
            {
                return false;
            }

            return true;
        }

        public void DecrementOne(Digits digits)
        {
            SetDigit(digits, GetDigitValue(digits) - 1);
        }

        public int GetDigitValue(Digits digits)
        {
            switch (digits)
            {
                case Digits.HOURS:
                    return hours;
                case Digits.MINUTES:
                    return minutes;
                case Digits.SECONDS:
                    return seconds;
                default:
                    throw new ArgumentOutOfRangeException(nameof(digits), digits, null);
            }
        }

        private int GetDigitMax(Digits digits)
        {
            switch (digits)
            {
                case Digits.HOURS:
                    return 99;
                case Digits.MINUTES:
                    return 59;
                case Digits.SECONDS:
                    return 59;
                default:
                    throw new ArgumentOutOfRangeException(nameof(digits), digits, null);
            }
        }

        private int GetDigitMin()
        {
            return 0;
        }

        private void SetDigit(Digits digit, int newValue)
        {
            switch (digit)
            {
                case Digits.HOURS:
                    hours = newValue;
                    break;
                case Digits.MINUTES:
                    minutes = newValue;
                    break;
                case Digits.SECONDS:
                    seconds = newValue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(digit), digit, null);
            }
            
            OnValidate();
            Initialize();
        }
    }
}
