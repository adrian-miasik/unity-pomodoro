using System;
using UnityEngine;
using UnityEngine.UI;

namespace AdrianMiasik
{
    public class PomodoroTimer : MonoBehaviour
    {
        public enum Digits
        {
            HOURS,
            MINUTES,
            SECONDS
        }

        [Header("Digits")] 
        [SerializeField] private DoubleDigit hourDigits;
        [SerializeField] private DoubleDigit minuteDigits;
        [SerializeField] private DoubleDigit secondDigits;

        [Header("Buttons")] 
        [SerializeField] private GameObject playPauseParent;
        [SerializeField] private PlayPauseButton playPauseButton;

        [Header("Ring")] 
        [SerializeField] private Image ring;

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
        private bool _isPaused;
        private double _currentTime;
        private float _totalTime; // In seconds

        // Pause Fade Animation
        private bool _isFading;
        private float _accumulatedFadeTime;
        [SerializeField] float _fadeDuration = 0.25f;
        private float _fadeProgress;
        private Color _startingColor;
        private Color _endingColor;
        private bool _isFadeComplete;
        [SerializeField] private float _pauseHoldDuration = 1f;

        // Specific to our ring shader
        private static readonly int RingColor = Shader.PropertyToID("Color_297012532bf444df807f8743bdb7e4fd");
        private DoubleDigit selectedDigit;
        private DoubleDigit lastSelectedDigit;

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
            _isPaused = false;
            _isFading = false;
            playPauseParent.gameObject.SetActive(true);

            // Update visuals
            ring.fillAmount = 1f;
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
                // Check exit condition
                if (_currentTime <= 0)
                {
                    _isRunning = false;

                    Complete();

                    // Early exit
                    return;
                }

                // Decrement timer
                ring.fillAmount = (float) _currentTime / _totalTime;
                UpdateDigitValues(TimeSpan.FromSeconds(_currentTime));
                _currentTime -= Time.deltaTime;
            }
            else if (_isPaused)
            {
                AnimatePausedDigits();
            }
        }

        private void AnimatePausedDigits()
        {
            _accumulatedFadeTime += Time.deltaTime;

            if (_isFadeComplete)
            {
                if (_accumulatedFadeTime > _pauseHoldDuration)
                {
                    _isFadeComplete = false;
                    _accumulatedFadeTime = 0;
                }
            }
            else
            {
                _fadeProgress = _accumulatedFadeTime / _fadeDuration;

                if (_isFading)
                {
                    SetDigitColor(Color.Lerp(_startingColor, _endingColor, _fadeProgress));
                }
                else
                {
                    SetDigitColor(Color.Lerp(_endingColor, _startingColor, _fadeProgress));
                }

                if (_fadeProgress >= 1)
                {
                    // Flip state
                    _isFading = !_isFading;
                    _accumulatedFadeTime = 0f;

                    _isFadeComplete = true;
                }
            }
        }

        /// <summary>
        /// Marks timer as complete, fills progress to full, and updates play/pause visuals
        /// </summary>
        private void Complete()
        {
            ring.fillAmount = 1f;
            playPauseParent.gameObject.SetActive(false);
            ring.material.SetColor(RingColor, completedColor);
        }

        /// <summary>
        /// Runs the pomodoro countdown timer
        /// </summary>
        public void Play()
        {
            if (_isPaused)
            {
                SetDigitColor(Color.black);
            }

            _isRunning = true;
            _isPaused = false;
            ring.material.SetColor(RingColor, runningColor);
            ClearSelection();
            
            LockEditing();
        }

        /// <summary>
        /// Prevents the timer from running
        /// </summary>
        public void Pause()
        {
            _isRunning = false;
            _isPaused = true;

            // Digit fade
            _accumulatedFadeTime = 0;
            _isFadeComplete = true;
            _isFading = true;
            _accumulatedFadeTime = 0f;
            _startingColor = Color.black;
            _endingColor = new Color(0.75f, 0.75f, 0.75f);

            ring.material.SetColor(RingColor, setupColor);
        }

        /// <summary>
        /// Unity OnClick
        /// </summary>
        public void Restart()
        {
            Pause();
            SetDigitColor(Color.black);
            Initialize();
            ClearSelection();
            UnlockEditing();
        }

        private void LockEditing()
        {
            hourDigits.Lock();
            minuteDigits.Lock();
            secondDigits.Lock();
        }

        private void UnlockEditing()
        {
            hourDigits.Unlock();
            minuteDigits.Unlock();
            secondDigits.Unlock();
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

        public void IncrementOne(Digits digits)
        {
            SetDigit(digits, GetDigitValue(digits) + 1);
        }

        public void DecrementOne(Digits digits)
        {
            SetDigit(digits, GetDigitValue(digits) - 1);
        }

        private void SetDigitColor(Color newColor)
        {
            hourDigits.SetTextColor(newColor);
            minuteDigits.SetTextColor(newColor);
            secondDigits.SetTextColor(newColor);
        }

        // Getters
        public bool IsRunning()
        {
            return _isRunning;
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

        public void SetSelection(DoubleDigit currentDigit)
        {
            lastSelectedDigit = selectedDigit;
            selectedDigit = currentDigit;

            // Deselect previous digit selection
            if (lastSelectedDigit != null && lastSelectedDigit != currentDigit)
            {
                lastSelectedDigit.Deselect();
            }
        }

        public void ClearSelection()
        {
            SetSelection(null);
        }
    }
}
