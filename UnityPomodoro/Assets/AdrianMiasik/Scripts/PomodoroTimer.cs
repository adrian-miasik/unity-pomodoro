using System;
using System.Collections.Generic;
using AdrianMiasik.Components;
using AdrianMiasik.Components.Core;
using AdrianMiasik.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AdrianMiasik
{
    public class PomodoroTimer : MonoBehaviour, IColorHook
    {
        [SerializeField] private Theme theme;

        // TODO: Support more timer digit formats
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
        [SerializeField] private Background background; // Used to pull select focus

        [Header("Digits")]
        [SerializeField] private DoubleDigit hourDigits;
        [SerializeField] private DoubleDigit minuteDigits;
        [SerializeField] private DoubleDigit secondDigits;

        [Header("Text")] 
        [SerializeField] private TextMeshProUGUI text;

        [Header("Buttons")] 
        [SerializeField] private BooleanToggle infoToggle;
        [SerializeField] private ClickButton leftButtonClick;
        [SerializeField] private ClickButton rightButtonClick;
        [SerializeField] private RightButton rightButton;
        [SerializeField] private BooleanSlider breakSlider;
        [SerializeField] private CreditsBubble creditsBubble;
        [SerializeField] private BooleanSlider themeSlider;
        private readonly List<ITimerState> timerElements = new List<ITimerState>();

        [Header("Ring")] 
        [SerializeField] private Image ring;

        [Header("Completion")]
        [SerializeField] private Animation completion; // Wrap mode doesn't matter
        [SerializeField] private AnimationCurve completeRingPulseDiameter = AnimationCurve.Linear(0, 0.9f, 1, 0.975f);
        public UnityEvent OnRingPulse;
        public UnityEvent OnTimerCompletion;
        
        [Header("Work Data")]
        [SerializeField] private int hours = 0;
        [SerializeField] private int minutes = 25;
        [SerializeField] private int seconds = 0;

        [Header("Break Data")]
        [SerializeField] private bool _isOnBreak;
        [SerializeField] private int breakHours;
        [SerializeField] private int breakMinutes = 5;
        [SerializeField] private int breakSeconds;
        
        [Header("Hotkeys")] 
        [SerializeField] private HotkeyDetector hotkeyDetector;

        // Digit Selection
        [SerializeField] private List<DoubleDigit> selectedDigits = new List<DoubleDigit>();

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
        
        void OnApplicationFocus(bool hasFocus)
        {
            // Prevent application from making noise when not in focus
            AudioListener.volume = !hasFocus ? 0 : 1;
        }

        private void Start()
        {
            // Single entry point
            Initialize();
        }
        
        /// <summary>
        /// Setup view, calculate time, initialize components, transition in, and animate.
        /// </summary>
        private void Initialize()
        {
            // Setup view
            theme.RegisterColorHook(this);
            infoContainer.gameObject.SetActive(false);
            contentContainer.gameObject.SetActive(true);

            // Initialize components - digits
            TimeSpan ts = TimeSpan.FromHours(hours) + TimeSpan.FromMinutes(minutes) + TimeSpan.FromSeconds(seconds);
            hourDigits.Initialize(Digits.HOURS, this, (int)ts.TotalHours);
            minuteDigits.Initialize(Digits.MINUTES, this, ts.Minutes);
            secondDigits.Initialize(Digits.SECONDS, this, ts.Seconds);

            // Initialize components - buttons
            creditsBubble.Initialize();
            rightButton.Initialize(this);
            infoToggle.Initialize(false, theme);
            breakSlider.Initialize(false, theme);
            themeSlider.Initialize(false, theme);

            // Initialize components - misc
            hotkeyDetector.Initialize(this);
            background.Initialize(theme);

            // Register elements that need updating per timer state change
            timerElements.Add(rightButton);

            // Calculate time
            _currentTime = ts.TotalSeconds;
            _totalTime = (float)ts.TotalSeconds;

            // Transition to setup state
            SwitchState(States.SETUP);

            // Animate in
            PlaySpawnAnimation();
        }
        
        /// <summary>
        /// Switches the timer to the provided state and handles all visual changes.
        /// Basically handles our transitions between timer states. <see cref="PomodoroTimer.States"/>
        /// </summary>
        /// <param name="desiredState">The state you want to transition to</param>
        private void SwitchState(States desiredState)
        {
            state = desiredState;

            // Update the registered timer elements
            foreach (ITimerState element in timerElements)
            {
                element.StateUpdate(state, theme);
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
                    text.text = !_isOnBreak ? "Set a work time" : "Set a break time";

                    // Show digits and hide completion label
                    digitContainer.gameObject.SetActive(true);
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
                    
                    ClearSelection();

                    // Lock Editing
                    hourDigits.Lock();
                    minuteDigits.Lock();
                    secondDigits.Lock();
                    break;

                case States.PAUSED:
                    text.text = "Paused";
                    
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

                    // Hide digits and reveal completion label
                    spawnAnimation.Stop();
                    digitContainer.gameObject.SetActive(false);
                    completion.gameObject.SetActive(true);

                    OnRingPulse.Invoke();
                    break;
            }
            
            ColorUpdate(theme.GetCurrentColorScheme());
        }
        
        // Unity Event
        public void PlaySpawnAnimation()
        {
            spawnAnimation.Stop();
            spawnAnimation.Play();
        }
        
        /// <summary>
        /// Sets the color of our digits to the provided color. <see cref="newColor"/>
        /// </summary>
        /// <param name="newColor"></param>
        private void SetDigitColor(Color newColor)
        {
            hourDigits.SetTextColor(newColor);
            minuteDigits.SetTextColor(newColor);
            secondDigits.SetTextColor(newColor);
        }
        
        /// <summary>
        /// Removes any digit selection, and selects the background by default.
        /// </summary>
        public void ClearSelection()
        {
            SetSelection(null);
            background.Select();
        }
        
        /// <summary>
        /// Sets the selection to a single double digit and calculates text visibility based on new selection data.
        /// If you'd like to select multiple digits, See AddSelection()
        /// </summary>
        /// <param name="currentDigit"></param>
        public void SetSelection(DoubleDigit currentDigit)
        {
            foreach (DoubleDigit digit in selectedDigits)
            {
                // Deselect previous digit selections
                if (digit != currentDigit)
                {
                    digit.Deselect();
                }
            }

            selectedDigits.Clear();
            if (currentDigit != null)
            {
                selectedDigits.Add(currentDigit);
            }
            
            CalculateTextState();
        }
        
        /// <summary>
        /// Determines state text visibility depending on the selected digits and timer state.
        /// </summary>
        private void CalculateTextState()
        {
            // Hide/show state text
            if (selectedDigits.Count <= 0)
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
                        ring.fillAmount = (float)_currentTime / _totalTime;
                        hourDigits.SetDigitsLabel((int)TimeSpan.FromSeconds(_currentTime).TotalHours);
                        minuteDigits.SetDigitsLabel(TimeSpan.FromSeconds(_currentTime).Minutes);
                        secondDigits.SetDigitsLabel(TimeSpan.FromSeconds(_currentTime).Seconds);
                    }
                    else
                    {
                        SwitchState(States.COMPLETE);
                        OnTimerCompletion?.Invoke();
                    }

                    break;

                case States.COMPLETE:
                    AnimateRingPulse();
                    break;
            }
        }
        
        /// <summary>
        /// Animates our digits to flash on and off
        /// </summary>
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
        /// Animates our ring visuals to pulse
        /// </summary>
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
            if (hasRingPulseBeenInvoked && _accumulatedRingPulseTime >
                completeRingPulseDiameter[completeRingPulseDiameter.length - 1].time)
            {
                _accumulatedRingPulseTime = 0;
                hasRingPulseBeenInvoked = false;
            }
        }
        
        /// <summary>
        /// Calculates the new time value based on the timer mode it's in (_isOnBreak)
        /// </summary>
        private void UpdateDigits()
        {
            TimeSpan ts;
            if (!_isOnBreak)
            {
                ts = TimeSpan.FromHours(hours) + TimeSpan.FromMinutes(minutes) + TimeSpan.FromSeconds(seconds);
            }
            else
            {
                ts = TimeSpan.FromHours(breakHours) + TimeSpan.FromMinutes(breakMinutes) +
                     TimeSpan.FromSeconds(breakSeconds);
            }

            hourDigits.SetDigitsLabel((int)ts.TotalHours);
            minuteDigits.SetDigitsLabel(ts.Minutes);
            secondDigits.SetDigitsLabel(ts.Seconds);
            _currentTime = ts.TotalSeconds;
            _totalTime = (float)ts.TotalSeconds;
        }

        /// <summary>
        /// Shows info, hides main content, and shows credits bubble
        /// </summary>
        public void ShowInfo()
        {
            // Hide main content, show info
            contentContainer.gameObject.SetActive(false);
            infoContainer.gameObject.SetActive(true);
            
            creditsBubble.Lock();
            creditsBubble.FadeIn();
        }

        /// <summary>
        /// Shows main content, hides info, and hides credits bubble
        /// </summary>
        public void HideInfo()
        {
            // Hide info, show main content
            infoContainer.gameObject.SetActive(false);
            contentContainer.gameObject.SetActive(true);
            
            creditsBubble.Unlock();
            creditsBubble.FadeOut();
        }
        
        /// <summary>
        /// Transitions timer into States.RUNNING mode
        /// </summary>
        public void Play()
        {
            SwitchState(States.RUNNING);
        }

        /// <summary>
        /// Transitions timer into States.PAUSED mode
        /// </summary>
        public void Pause()
        {
            SwitchState(States.PAUSED);
        }

        /// <summary>
        /// Transitions timer into States.SETUP mode in break mode
        /// </summary>
        public void SwitchToBreakTimer()
        {
            _isOnBreak = true;
            SwitchState(States.SETUP);
            UpdateDigits();
        }

        /// <summary>
        /// Transitions timer into States.SETUP mode in work mode
        /// </summary>
        public void SwitchToWorkTimer()
        {
            _isOnBreak = false;
            SwitchState(States.SETUP);
            UpdateDigits();
        }

        /// <summary>
        /// Toggles the timer mode to it's opposite mode (break/work) and transitions timer into States.SETUP
        /// </summary>
        /// <param name="isCompleted"></param>
        public void Restart(bool isCompleted)
        {
            if (isCompleted)
            {
                _isOnBreak = !_isOnBreak;
            }

            SwitchState(States.SETUP);
            UpdateDigits();
        }
        
        /// <summary>
        /// Increments the provided digit by one. (+1)
        /// </summary>
        /// <param name="digits"></param>
        public void IncrementOne(Digits digits)
        {
            SetDigit(digits, GetDigitValue(digits) + 1);
        }

        /// <summary>
        /// Decrements the provided digit by one. (-1)
        /// </summary>
        /// <param name="digits"></param>
        public void DecrementOne(Digits digits)
        {
            SetDigit(digits, GetDigitValue(digits) - 1);
        }

        /// <summary>
        /// Activates the play/pause button to toggle the timer state (States.SETUP, etc...)
        /// </summary>
        public void TriggerPlayPause()
        {
            ClearSelection();
            foreach (DoubleDigit digit in selectedDigits)
            {
                digit.Deselect();
                digit.Lock();
            }
            rightButtonClick.OnPointerClick(null);
        }

        /// <summary>
        /// Activates the boolean slider to toggle between work/break
        /// </summary>
        public void TriggerTimerSwitch()
        {
            breakSlider.OnPointerClick(null);
        }
        
        /// <summary>
        /// Activates the restart button to trigger a restart
        /// </summary>
        public void TriggerTimerRestart()
        {
            leftButtonClick.OnPointerClick(null);
        }

        /// <summary>
        /// Selects all the digits
        /// </summary>
        public void SelectAll()
        {
            // Only allow 'select all' to work when we are in setup state
            if (state != States.SETUP)
            {
                return;
            }
            
            ClearSelection();

            AddSelection(hourDigits);
            AddSelection(minuteDigits);
            AddSelection(secondDigits);

            foreach (DoubleDigit digit in selectedDigits)
            {
                digit.Highlight();
            }
            
            // Since we are highlighting (instead of selecting), we bypass the text state logic hence we 
            // invoke it again here.
            CalculateTextState();
        }

        /// <summary>
        /// Adds the provided digit to our selection list
        /// </summary>
        /// <param name="digitToAddToSelection"></param>
        private void AddSelection(DoubleDigit digitToAddToSelection)
        {
            if (!selectedDigits.Contains(digitToAddToSelection))
            {
                selectedDigits.Add(digitToAddToSelection);
            }
        }
        
        // Getters
        
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

        public string GetTimerString()
        {
            return hourDigits.GetDigitsLabel() + ":" +
                   minuteDigits.GetDigitsLabel() + ":" +
                   secondDigits.GetDigitsLabel();
        }
        
        public bool GetIsOnBreak()
        {
            return _isOnBreak;
        }

        public Theme GetTheme()
        {
            return theme;
        }
        
        /// <summary>
        /// Returns True if you can add one to this digit without hitting it's ceiling, otherwise returns False.
        /// </summary>
        /// <param name="digits"></param>
        /// <returns></returns>
        public bool CanIncrementOne(Digits digits)
        {
            if (GetDigitValue(digits) + 1 > GetDigitMax(digits))
            {
                return false;
            }

            return true;
        }

        /// Returns True if you can subtract one to this digit without hitting it's floor, otherwise returns False.
        public bool CanDecrementOne(Digits digits)
        {
            if (GetDigitValue(digits) - 1 < GetDigitMin())
            {
                return false;
            }

            return true;
        }

        // Setters
        
        /// <summary>
        /// Sets the provided digit to it's provided value. (Will validate to make sure it's with bounds though)
        /// </summary>
        /// <param name="digit"></param>
        /// <param name="newValue"></param>
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
        
        /// <summary>
        /// Sets the value of the timer using the provided formatted string.
        /// </summary>
        /// <param name="formattedString">Expected format of ("00:25:00")</param>
        public void SetTimerValue(string formattedString)
        {
            // Only allow 'Set Timer Value' to work when we are in the setup state
            if (state != States.SETUP)
            {
                return;
            }
            
            List<string> sections = new List<string>();
            string value = String.Empty;

            // Determine how many sections there are
            for (int i = 0; i < formattedString.Length; i++)
            {
                // If this character is a separator...
                if (formattedString[i].ToString() == ":")
                {
                    // Save section
                    if (value != String.Empty)
                    {
                        sections.Add(value);
                        value = string.Empty;
                    }

                    continue;
                }

                // Add to section
                value += formattedString[i].ToString();
            }

            // Last digit in string won't have a separator so we add the section in once the loop is complete
            sections.Add(value);

            // Compare sections with timer format
            if (sections.Count != 3)
            {
                Debug.LogWarning("The provided string does not match the pomodoro timer layout");
                return;
            }

            // Set timer sections
            for (int i = 0; i < sections.Count; i++)
            {
                switch (i)
                {
                    case 0:
                        SetHours(sections[i]);
                        break;
                    case 1:
                        SetMinutes(sections[i]);
                        break;
                    case 2:
                        SetSeconds(sections[i]);
                        break;
                }
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
        
        public void ColorUpdate(ColorScheme currentColors)
        {
            text.color = currentColors.selection;
            
            switch (state)
            {
                case States.SETUP:
                    ring.material.SetColor(RingColor,
                        !_isOnBreak ? theme.GetCurrentColorScheme().modeOne : theme.GetCurrentColorScheme().modeTwo);
                    SetDigitColor(Color.black);
                    break;
                case States.RUNNING:
                    ring.material.SetColor(RingColor, theme.GetCurrentColorScheme().running);
                    SetDigitColor(Color.black);
                    break;
                case States.PAUSED:
                    ring.material.SetColor(RingColor, 
                        !_isOnBreak ? theme.GetCurrentColorScheme().modeOne : theme.GetCurrentColorScheme().modeTwo);
                    break;
                case States.COMPLETE:
                    ring.material.SetColor(RingColor, theme.GetCurrentColorScheme().complete);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}