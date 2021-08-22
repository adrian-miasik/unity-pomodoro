using System;
using System.Collections.Generic;
using System.Xml;
using AdrianMiasik.Components;
using AdrianMiasik.Components.Core;
using AdrianMiasik.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.WSA;
using Application = UnityEngine.Application;

namespace AdrianMiasik
{
    public class PomodoroTimer : MonoBehaviour
    {
        // Colors
        public static Color colorWork = new Color(0.05f, 0.47f, 0.95f); // or pause
        public static Color colorRelax = new Color(1f, 0.83f, 0.23f);
        public static Color colorRunning = new Color(0.35f, 0.89f, 0.4f);
        public static Color colorComplete = new Color(0.97f, 0.15f, 0.15f);
        public static Color colorDeselected = new Color(0.91f, 0.91f, 0.91f);

        public enum Digits
        {
            HOURS,
            MINUTES,
            SECONDS
        }

        public enum States
        {
            SETUP,
            RUNNING,
            PAUSED,
            COMPLETE
        }

        public States state = States.SETUP;

        [Header("Animations")] 
        [SerializeField] private Animation spawnAnimation;

        [Header("Containers")]
        [SerializeField] private GameObject contentContainer; // main content
        [SerializeField] private GameObject infoContainer; // info content
        [SerializeField] private GameObject digitContainer; // Used to toggle digit visibility

        [Header("Background")] 
        [SerializeField] private Selectable background; // Used to pull select focus

        [Header("Digits")]
        [SerializeField] private DoubleDigit hourDigits;
        [SerializeField] private DoubleDigit minuteDigits;
        [SerializeField] private DoubleDigit secondDigits;

        [Header("Text")] 
        [SerializeField] private TextMeshProUGUI text;

        [Header("Buttons")] 
        [SerializeField] private BooleanToggle infoToggle;
        [SerializeField] private RightButton rightButton;
        [SerializeField] private BooleanSlider breakSlider;
        [SerializeField] private CreditsBubble creditsBubble;
        private readonly List<ITimerState> timerElements = new List<ITimerState>();

        [Header("Ring")] 
        [SerializeField] private Image ring;

        [Header("Completion")]
        [SerializeField] private Animation completion; // Wrap mode doesn't matter
        [SerializeField] private AnimationCurve completeRingPulseDiameter = AnimationCurve.Linear(0, 0.9f, 1, 0.975f);
        public UnityEvent OnRingPulse;
        
        [Header("Data")]
        [SerializeField] private int hours = 0;
        [SerializeField] private int minutes = 25;
        [SerializeField] private int seconds = 0;

        [Header("Break Data")]
        [SerializeField] private bool _isOnBreak;
        [SerializeField] private int breakHours;
        [SerializeField] private int breakMinutes = 5;
        [SerializeField] private int breakSeconds;

        // UWP
        [Header("Toast")]
        [Multiline]
        public string customToast;

        // Digit Selection
        private DoubleDigit selectedDigit;
        private DoubleDigit lastSelectedDigit;

        // Time
        private double _currentTime;
        private float _totalTime; // In seconds

        // Pause Fade Animation
        [Header("Fade Animation")] 
        [SerializeField] float _fadeDuration = 0.25f;
        [SerializeField] private float _pauseHoldDuration = 1f; // How long to wait between fade completions?
        private bool _isFading;
        private float _accumulatedFadeTime;
        private float _fadeProgress;
        private Color _startingColor;
        private Color _endingColor;
        private bool _isFadeComplete;

        // Pulse Ring Animation
        private float _accumulatedRingPulseTime;
        private bool hasRingPulseBeenInvoked;

        // Ring Shader Properties
        private static readonly int RingColor = Shader.PropertyToID("Color_297012532bf444df807f8743bdb7e4fd");
        private static readonly int RingDiameter = Shader.PropertyToID("Vector1_98525729712540259c19ac6e37e93b62");

        private void OnValidate()
        {
            // Prevent values from going over their limit
            if (!_isOnBreak)
            {
                hours = Mathf.Clamp(hours, GetDigitMin(), GetDigitMax(Digits.HOURS));
                minutes = Mathf.Clamp(minutes, GetDigitMin(), GetDigitMax(Digits.MINUTES));
                seconds = Mathf.Clamp(seconds, GetDigitMin(), GetDigitMax(Digits.SECONDS));
            }
            else
            {
                breakHours = Mathf.Clamp(breakHours, GetDigitMin(), GetDigitMax(Digits.HOURS));
                breakMinutes = Mathf.Clamp(breakMinutes, GetDigitMin(), GetDigitMax(Digits.MINUTES));
                breakSeconds = Mathf.Clamp(breakSeconds, GetDigitMin(), GetDigitMax(Digits.SECONDS));
            }
        }

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            // Setup view
            infoContainer.gameObject.SetActive(false);
            contentContainer.gameObject.SetActive(true);
            
            // Initialize components - digits
            TimeSpan ts = TimeSpan.FromHours(hours) + TimeSpan.FromMinutes(minutes) + TimeSpan.FromSeconds(seconds);
            hourDigits.Initialize(Digits.HOURS, this, (int) ts.TotalHours);
            minuteDigits.Initialize(Digits.MINUTES, this, ts.Minutes);
            secondDigits.Initialize(Digits.SECONDS, this, ts.Seconds);

            // Initialize components - buttons
            infoToggle.Initialize(false);
            rightButton.Initialize(this);
            breakSlider.Initialize(false, colorDeselected, colorRelax);
            creditsBubble.Initialize();
            
            // Register elements that need updating per timer state change
            timerElements.Add(rightButton);

            // Calculate time
            _currentTime = ts.TotalSeconds;
            _totalTime = (float) ts.TotalSeconds;

            // Transition to setup state
            SwitchState(States.SETUP);

            // Animate in
            PlaySpawnAnimation();
        }

        private void SwitchState(States desiredState)
        {
            state = desiredState;

            // Update the registered timer elements
            foreach (ITimerState element in timerElements)
            {
                element.StateUpdate(state);
            }

            // Do transition logic
            switch (state)
            {
                case States.SETUP:
                    // Show state text
                    text.gameObject.SetActive(true);

                    // Complete ring
                    ring.fillAmount = 1f;
                    ring.material.SetFloat(RingDiameter, 0.9f);
                    if (!_isOnBreak)
                    {
                        text.text = "Set a work time";
                        ring.material.SetColor(RingColor, colorWork);
                    }
                    else
                    {
                        text.text = "Set a break time";
                        ring.material.SetColor(RingColor, colorRelax);
                    }

                    // Show digits and hide completion label
                    digitContainer.gameObject.SetActive(true);
                    SetDigitColor(Color.black);
                    completion.gameObject.SetActive(false);
                    
                    // Reset
                    _isFading = false;
                    _accumulatedRingPulseTime = 0;
                    completion.gameObject.transform.localScale = Vector3.one;

                    ClearSelection();

                    // Unlock editing
                    hourDigits.Unlock();
                    minuteDigits.Unlock();
                    secondDigits.Unlock();
                    break;

                case States.RUNNING:
                    text.text = "Running";
                    ring.material.SetColor(RingColor, colorRunning);

                    SetDigitColor(Color.black);
                    ClearSelection();

                    // Lock Editing
                    hourDigits.Lock();
                    minuteDigits.Lock();
                    secondDigits.Lock();
                    break;

                case States.PAUSED:
                    text.text = "Paused";
                    if (!_isOnBreak)
                    {
                        ring.material.SetColor(RingColor, colorWork);
                    }
                    else
                    {
                        ring.material.SetColor(RingColor, colorRelax);
                    }

                    // Digit fade reset
                    _accumulatedFadeTime = 0;
                    _isFadeComplete = true;
                    _isFading = true;
                    _accumulatedFadeTime = 0f;
                    _startingColor = Color.black;
                    _endingColor = new Color(0.75f, 0.75f, 0.75f);
                    break;

                case States.COMPLETE:
                    // Hide state text
                    text.gameObject.SetActive(false);

                    // Complete ring
                    ring.fillAmount = 1f;
                    ring.material.SetColor(RingColor, colorComplete);

                    // Hide digits and reveal completion label
                    spawnAnimation.Stop();
                    digitContainer.gameObject.SetActive(false);
                    completion.gameObject.SetActive(true);
                    
                    OnRingPulse.Invoke();
                    break;
            }
        }

        void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
            {
                AudioListener.volume = 0;
            }
            else
            {
                AudioListener.volume = 1;
            }
        }

        private void Update()
        {
            switch (state)
            {
                case States.PAUSED:
                    AnimatePausedDigits();
                    break;

                case States.RUNNING:
                    if (_currentTime > 0)
                    {
                        // Decrement timer
                        _currentTime -= Time.deltaTime;

                        // Update visuals
                        ring.fillAmount = (float) _currentTime / _totalTime;
                        hourDigits.SetDigitsLabel((int) TimeSpan.FromSeconds(_currentTime).TotalHours);
                        minuteDigits.SetDigitsLabel(TimeSpan.FromSeconds(_currentTime).Minutes);
                        secondDigits.SetDigitsLabel(TimeSpan.FromSeconds(_currentTime).Seconds);
                    }
                    else
                    {
                        SwitchState(States.COMPLETE);
                        
#if ENABLE_WINMD_SUPPORT
                        // Works
                        Toast xmlToast = Toast.Create(customToast);
                        xmlToast.Show();

                        // Works
                        // Toast toast = Toast.Create("", "Timer Complete!");
                        // toast.Show();

                        // Works
                        // UnityEngine.WSA.Toast.Create(UnityEngine.WSA.Toast.GetTemplate(UnityEngine.WSA.ToastTemplate.ToastText01)).Show();

                        // Didn't work
                        // UnityEngine.WSA.Toast.Create("Assets/AdrianMiasik/Scripts/MyToast.xml").Show();
#endif
                    }

                    break;

                case States.COMPLETE:
                    AnimateRingPulse();
                    break;
            }
        }

        public void Play()
        {
            SwitchState(States.RUNNING);
        }

        public void Pause()
        {
            SwitchState(States.PAUSED);
        }

        public void SwitchToBreakTimer()
        {
            _isOnBreak = true;
            SwitchState(States.SETUP);
            UpdateDigits();
        }

        public void SwitchToWorkTimer()
        {
            _isOnBreak = false;
            SwitchState(States.SETUP);
            UpdateDigits();
        }

        public void Restart(bool isCompleted)
        {
            if (isCompleted)
            {
                _isOnBreak = !_isOnBreak;
            }

            SwitchState(States.SETUP);
            UpdateDigits();
        }

        private void UpdateDigits()
        {
            TimeSpan ts;
            if (!_isOnBreak)
            {
                ts = TimeSpan.FromHours(hours) + TimeSpan.FromMinutes(minutes) + TimeSpan.FromSeconds(seconds);
            }
            else
            {
                ts = TimeSpan.FromHours(breakHours) + TimeSpan.FromMinutes(breakMinutes) + TimeSpan.FromSeconds(breakSeconds);
            }
            
            hourDigits.SetDigitsLabel((int) ts.TotalHours);
            minuteDigits.SetDigitsLabel(ts.Minutes);
            secondDigits.SetDigitsLabel(ts.Seconds);
            _currentTime = ts.TotalSeconds;
            _totalTime = (float) ts.TotalSeconds;
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

        private void AnimateRingPulse()
        {
            // Calculate diameter
            _accumulatedRingPulseTime += Time.deltaTime;
            float ringDiameter = completeRingPulseDiameter.Evaluate(_accumulatedRingPulseTime);
            
            // Set diameter
            ring.material.SetFloat(RingDiameter, ringDiameter);
            completion.gameObject.transform.localScale = Vector3.one * ringDiameter;

            if (!hasRingPulseBeenInvoked)
            {
                OnRingPulse.Invoke();
                hasRingPulseBeenInvoked = true;
            }

            // Ignore wrap mode and replay completion animation from start
            if (hasRingPulseBeenInvoked && _accumulatedRingPulseTime > completeRingPulseDiameter[completeRingPulseDiameter.length - 1].time)
            {
                _accumulatedRingPulseTime = 0;
                hasRingPulseBeenInvoked = false;
            }
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

        private void SetDigitColor(Color newColor)
        {
            hourDigits.SetTextColor(newColor);
            minuteDigits.SetTextColor(newColor);
            secondDigits.SetTextColor(newColor);
        }

        // Getters
        public bool IsOnBreak()
        {
            return _isOnBreak;
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
            if (!_isOnBreak)
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
                        Debug.LogWarning("Digit value not supported");
                        return 0;
                }
            }

            switch (digits)
            {
                case Digits.HOURS:
                    return breakHours;
                case Digits.MINUTES:
                    return breakMinutes;
                case Digits.SECONDS:
                    return breakSeconds;
                default:
                    Debug.LogWarning("Digit value not supported");
                    return 0;
            }
        }
        
        public void SetHours(string hours)
        {
            SetDigit(Digits.HOURS, string.IsNullOrEmpty(hours) ? 0 : int.Parse(hours));
        }

        public void SetMinutes(string minutes)
        {
            SetDigit(Digits.MINUTES, string.IsNullOrEmpty(minutes) ? 0 : int.Parse(minutes));
        }

        public void SetSeconds(string seconds)
        {
            SetDigit(Digits.SECONDS, string.IsNullOrEmpty(seconds) ? 0 : int.Parse(seconds));
        }
        
        public void IncrementOne(Digits digits)
        {
            SetDigit(digits, GetDigitValue(digits) + 1);
        }

        public void DecrementOne(Digits digits)
        {
            SetDigit(digits, GetDigitValue(digits) - 1);
        }

        private void SetDigit(Digits digit, int newValue)
        {
            if (!_isOnBreak)
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
                }
            }
            else
            {
                switch (digit)
                {
                    case Digits.HOURS:
                        breakHours = newValue;
                        break;
                    case Digits.MINUTES:
                        breakMinutes = newValue;
                        break;
                    case Digits.SECONDS:
                        breakSeconds = newValue;
                        break;
                }   
            }

            OnValidate();
            UpdateDigits();
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

            // Hide/show state text
            if (selectedDigit == null)
            {
                if (state != States.COMPLETE)
                {
                    text.gameObject.SetActive(true);
                }
            }
            else
            {
                text.gameObject.SetActive(false);
            }
        }

        public void ClearSelection()
        {
            SetSelection(null);
            background.Select();
        }

        public void ShowInfo()
        {
            // Hide main content, show info
            contentContainer.gameObject.SetActive(false);
            infoContainer.gameObject.SetActive(true);
            
            creditsBubble.Lock();
            creditsBubble.FadeIn();
        }

        public void HideInfo()
        {
            // Hide info, show main content
            infoContainer.gameObject.SetActive(false);
            contentContainer.gameObject.SetActive(true);
            
            creditsBubble.Unlock();
            creditsBubble.FadeOut();
        }

        public void PlaySpawnAnimation()
        {
            spawnAnimation.Stop();
            spawnAnimation.Play();
        }
    }
}