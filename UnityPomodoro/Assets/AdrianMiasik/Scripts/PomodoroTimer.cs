using System;
using System.Collections.Generic;
using AdrianMiasik.Components;
using AdrianMiasik.Components.Core;
using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AdrianMiasik
{
    public class PomodoroTimer : MonoBehaviour, IColorHook
    {
        [SerializeField] private Theme theme;
        
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
        [SerializeField] private InformationPanel infoContainer; // info content
        private bool isInfoPageOpen;
        
        [Header("Background")] 
        [SerializeField] private Background background; // Used to pull select focus

        [Header("Digits")] 
        [SerializeField] private DigitFormat digitFormat;

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
        [SerializeField] private Image ringBackground;

        [Header("Completion")]
        [SerializeField] private Animation completion; // Wrap mode doesn't matter
        [SerializeField] private AnimationCurve completeRingPulseDiameter = AnimationCurve.Linear(0, 0.9f, 1, 0.975f);
        public UnityEvent OnRingPulse;
        public UnityEvent OnTimerCompletion;
        
        [Header("Work Data")]
        [SerializeField] private int hours;
        [SerializeField] private int minutes = 25;
        [SerializeField] private int seconds;

        [Header("Break Data")]
        [SerializeField] private bool isOnBreak;
        [SerializeField] private int breakHours;
        [SerializeField] private int breakMinutes = 5;
        [SerializeField] private int breakSeconds;
        
        [Header("Hotkeys")] 
        [SerializeField] private HotkeyDetector hotkeyDetector;

        // Digit Selection
        [SerializeField] private List<DoubleDigit> selectedDigits = new List<DoubleDigit>();

        // Time
        private double currentTime;
        private float totalTime; // In seconds

        // Pause Fade Animation
        [Header("Fade Animation")] 
        [SerializeField] private float fadeDuration = 0.1f;
        [SerializeField] private float pauseHoldDuration = 0.75f; // How long to wait between fade completions?
        private bool isFading;
        private float accumulatedFadeTime;
        private float fadeProgress;
        private Color startingColor;
        private Color endingColor;
        private bool isFadeComplete;

        // Pulse Ring Complete Animation
        private float accumulatedRingPulseTime;
        private bool hasRingPulseBeenInvoked;
        
        // Pulse Tick Ring Animation
        private float cachedSeconds;
        private bool isRingTickAnimating;
        [SerializeField] private AnimationCurve ringTickWidth;

        // Shader Properties
        private static readonly int RingColor = Shader.PropertyToID("Color_297012532bf444df807f8743bdb7e4fd");
        private static readonly int RingDiameter = Shader.PropertyToID("Vector1_98525729712540259c19ac6e37e93b62");
        private static readonly int CircleColor = Shader.PropertyToID("Color_297012532bf444df807f8743bdb7e4fd");
        
        private void OnValidate()
        {
            // Prevent values from going over their limit
            if (!isOnBreak)
            {
                hours = Mathf.Clamp(hours, digitFormat.GetDigitMin(), digitFormat.GetDigitMax(DigitFormat.Digits.HOURS));
                minutes = Mathf.Clamp(minutes, digitFormat.GetDigitMin(), digitFormat.GetDigitMax(DigitFormat.Digits.MINUTES));
                seconds = Mathf.Clamp(seconds, digitFormat.GetDigitMin(), digitFormat.GetDigitMax(DigitFormat.Digits.SECONDS));
            }
            else
            {
                breakHours = Mathf.Clamp(breakHours, digitFormat.GetDigitMin(), digitFormat.GetDigitMax(DigitFormat.Digits.HOURS));
                breakMinutes = Mathf.Clamp(breakMinutes, digitFormat.GetDigitMin(), digitFormat.GetDigitMax(DigitFormat.Digits.MINUTES));
                breakSeconds = Mathf.Clamp(breakSeconds, digitFormat.GetDigitMin(), digitFormat.GetDigitMax(DigitFormat.Digits.SECONDS));
            }
        }

        private void OnApplicationFocus(bool _hasFocus)
        {
            // Prevent application from making noise when not in focus
            AudioListener.volume = !_hasFocus ? 0 : 1;
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
            infoContainer.gameObject.SetActive(false);
            contentContainer.gameObject.SetActive(true);

            // Calculate time
            TimeSpan _ts = TimeSpan.FromHours(hours) + TimeSpan.FromMinutes(minutes) + TimeSpan.FromSeconds(seconds);
            cachedSeconds = _ts.Seconds;
            currentTime = _ts.TotalSeconds;
            totalTime = (float)_ts.TotalSeconds;
            
            // Initialize digits
            digitFormat.Initialize(this, theme);
            digitFormat.SetFormatTime(_ts);

            // Initialize buttons
            creditsBubble.Initialize(this);
            rightButton.Initialize(this);
            infoToggle.Initialize(false, theme);
            breakSlider.Initialize(false, theme);
            themeSlider.Initialize(false, theme);

            // Initialize misc
            hotkeyDetector.Initialize(this);
            background.Initialize(theme);

            // Register elements that need updating per timer state change
            timerElements.Add(rightButton);
            
            // Transition to setup state
            SwitchState(States.SETUP);

            // Animate in
            PlaySpawnAnimation();
            
            // Setup theme
            theme.RegisterColorHook(this);
            if (theme.isLightModeOn)
            {
                themeSlider.Disable();
            }
            else
            {
                themeSlider.Enable();
            }
            
            // Apply theme
            theme.ApplyColorChanges();
        }
        
        /// <summary>
        /// Switches the timer to the provided state and handles all visual changes.
        /// Basically handles our transitions between timer states. <see cref="PomodoroTimer.States"/>
        /// </summary>
        /// <param name="_desiredState">The state you want to transition to</param>
        private void SwitchState(States _desiredState)
        {
            state = _desiredState;

            // Update the registered timer elements
            foreach (ITimerState _element in timerElements)
            {
                _element.StateUpdate(state, theme);
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
                    text.text = !isOnBreak ? "Set a work time" : "Set a break time";

                    // Show digits and hide completion label
                    digitFormat.Show();
                    GameObject _completionGO;
                    (_completionGO = completion.gameObject).SetActive(false);

                    // Reset
                    _completionGO.transform.localScale = Vector3.one;
                    isFading = false;
                    accumulatedRingPulseTime = 0;

                    ClearSelection();
                    
                    // Unlock editing
                    digitFormat.Unlock();
                    break;

                case States.RUNNING:
                    text.text = "Running";
                    
                    ClearSelection();

                    // Lock Editing
                    digitFormat.Lock();
                    break;

                case States.PAUSED:
                    text.text = "Paused";
                    ResetDigitFadeAnim();
                    break;

                case States.COMPLETE:
                    // Hide state text
                    text.gameObject.SetActive(false);

                    // Complete ring
                    ring.fillAmount = 1f;

                    // Hide digits and reveal completion label
                    spawnAnimation.Stop();
                    digitFormat.Hide();
                    completion.gameObject.SetActive(true);

                    OnRingPulse.Invoke();
                    break;
            }
            
            ColorUpdate(theme);
        }

        private void ResetDigitFadeAnim()
        {
            accumulatedFadeTime = 0;
            isFadeComplete = true;
            isFading = true;
            accumulatedFadeTime = 0f;
        }
        
        // Unity Event
        public void PlaySpawnAnimation()
        {
            spawnAnimation.Stop();
            spawnAnimation.Play();
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
        /// <param name="_currentDigit"></param>
        public void SetSelection(DoubleDigit _currentDigit)
        {
            foreach (DoubleDigit _digit in selectedDigits)
            {
                // Deselect previous digit selections
                if (_digit != _currentDigit)
                {
                    _digit.Deselect();
                }
            }

            selectedDigits.Clear();
            if (_currentDigit != null)
            {
                selectedDigits.Add(_currentDigit);
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
                    if (currentTime > 0)
                    {
                        // Decrement timer
                        currentTime -= Time.deltaTime;

                        if (cachedSeconds != TimeSpan.FromSeconds(currentTime).Seconds)
                        {
                            isRingTickAnimating = true;
                        }

                        if (isRingTickAnimating)
                        {
                            accumulatedRingPulseTime += Time.deltaTime;
                            ring.material.SetFloat(RingDiameter, ringTickWidth.Evaluate(accumulatedRingPulseTime));
                        }

                        // Update visuals
                        ring.fillAmount = (float)currentTime / totalTime;
                        digitFormat.SetFormatTime(TimeSpan.FromSeconds(currentTime));

                        cachedSeconds = TimeSpan.FromSeconds(currentTime).Seconds;
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
            accumulatedFadeTime += Time.deltaTime;

            if (isFadeComplete)
            {
                if (accumulatedFadeTime > pauseHoldDuration)
                {
                    isFadeComplete = false;
                    accumulatedFadeTime = 0;
                }
            }
            else
            {
                fadeProgress = accumulatedFadeTime / fadeDuration;

                digitFormat.SetDigitColor(isFading
                    ? Color.Lerp(startingColor, endingColor, fadeProgress)
                    : Color.Lerp(endingColor, startingColor, fadeProgress));

                if (fadeProgress >= 1)
                {
                    // Flip state
                    isFading = !isFading;
                    accumulatedFadeTime = 0f;

                    isFadeComplete = true;
                }
            }
        }
        
        /// <summary>
        /// Animates our ring visuals to pulse
        /// </summary>
        private void AnimateRingPulse()
        {
            // Calculate diameter
            accumulatedRingPulseTime += Time.deltaTime;
            float _ringDiameter = completeRingPulseDiameter.Evaluate(accumulatedRingPulseTime);

            // Set diameter
            ring.material.SetFloat(RingDiameter, _ringDiameter);
            completion.gameObject.transform.localScale = Vector3.one * _ringDiameter;

            if (!hasRingPulseBeenInvoked)
            {
                OnRingPulse.Invoke();
                hasRingPulseBeenInvoked = true;
            }

            // Ignore wrap mode and replay completion animation from start
            if (hasRingPulseBeenInvoked && accumulatedRingPulseTime >
                completeRingPulseDiameter[completeRingPulseDiameter.length - 1].time)
            {
                accumulatedRingPulseTime = 0;
                hasRingPulseBeenInvoked = false;
            }
        }
        
        /// <summary>
        /// Calculates the new time value based on the timer mode it's in (_isOnBreak)
        /// </summary>
        private void UpdateDigits()
        {
            TimeSpan _ts;
            if (!isOnBreak)
            {
                _ts = TimeSpan.FromHours(hours) + TimeSpan.FromMinutes(minutes) + TimeSpan.FromSeconds(seconds);
            }
            else
            {
                _ts = TimeSpan.FromHours(breakHours) + TimeSpan.FromMinutes(breakMinutes) +
                     TimeSpan.FromSeconds(breakSeconds);
            }

            digitFormat.SetFormatTime(_ts);
            
            currentTime = _ts.TotalSeconds;
            totalTime = (float)_ts.TotalSeconds;
        }

        /// <summary>
        /// Shows info, hides main content, and shows credits bubble
        /// </summary>
        public void ShowInfo()
        {
            // Prevent tick animations from pausing when switching to info page
            digitFormat.CorrectTickAnimVisuals();
            
            // Hide main content, show info
            contentContainer.gameObject.SetActive(false);
            infoContainer.gameObject.SetActive(true);
            isInfoPageOpen = true;
            infoContainer.ColorUpdate(theme);

            if (!creditsBubble.IsRunning())
            {
                creditsBubble.Lock();
                creditsBubble.FadeIn();   
            }
        }

        /// <summary>
        /// Shows main content, hides info, and hides credits bubble
        /// </summary>
        public void HideInfo()
        {
            // Hide info, show main content
            infoContainer.gameObject.SetActive(false);
            isInfoPageOpen = false;
            contentContainer.gameObject.SetActive(true);
            
            creditsBubble.Unlock();
            creditsBubble.FadeOut();
        }
        
        public bool IsInfoPageOpen()
        {
            return isInfoPageOpen;
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
            isOnBreak = true;
            SwitchState(States.SETUP);
            UpdateDigits();
        }

        /// <summary>
        /// Transitions timer into States.SETUP mode in work mode
        /// </summary>
        public void SwitchToWorkTimer()
        {
            isOnBreak = false;
            SwitchState(States.SETUP);
            UpdateDigits();
        }

        /// <summary>
        /// Toggles the timer mode to it's opposite mode (break/work) and transitions timer into States.SETUP
        /// </summary>
        /// <param name="_isCompleted"></param>
        public void Restart(bool _isCompleted)
        {
            if (_isCompleted)
            {
                isOnBreak = !isOnBreak;
            }

            SwitchState(States.SETUP);
            UpdateDigits();
            
            // Stop digit tick animation
            digitFormat.ResetTextPositions();
        }
        
        /// <summary>
        /// Increments the provided digit by one. (+1)
        /// </summary>
        /// <param name="_digits"></param>
        public void IncrementOne(DigitFormat.Digits _digits)
        {
            SetDigit(_digits, GetDigitValue(_digits) + 1);
        }

        /// <summary>
        /// Decrements the provided digit by one. (-1)
        /// </summary>
        /// <param name="_digits"></param>
        public void DecrementOne(DigitFormat.Digits _digits)
        {
            SetDigit(_digits, GetDigitValue(_digits) - 1);
        }

        /// <summary>
        /// Activates the play/pause button to toggle the timer state (States.SETUP, etc...)
        /// </summary>
        public void TriggerPlayPause()
        {
            ClearSelection();
            foreach (DoubleDigit _digit in selectedDigits)
            {
                _digit.Deselect();
                _digit.Lock();
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
        /// Activates the boolean slider to toggle between light/dark themes
        /// </summary>
        public void TriggerThemeSwitch()
        {
            themeSlider.OnPointerClick(null);
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

            foreach (DoubleDigit _digit in digitFormat.GetDigits())
            {
                AddSelection(_digit);
            }

            foreach (DoubleDigit _digit in selectedDigits)
            {
                _digit.Highlight();
            }
            
            // Since we are highlighting (instead of selecting), we bypass the text state logic hence we 
            // invoke it again here.
            CalculateTextState();
        }

        /// <summary>
        /// Adds the provided digit to our selection list
        /// </summary>
        /// <param name="_digitToAddToSelection"></param>
        private void AddSelection(DoubleDigit _digitToAddToSelection)
        {
            if (!selectedDigits.Contains(_digitToAddToSelection))
            {
                selectedDigits.Add(_digitToAddToSelection);
            }
        }
        
        // Getters
        
        public int GetDigitValue(DigitFormat.Digits _digits)
        {
            if (!isOnBreak)
            {
                switch (_digits)
                {
                    case DigitFormat.Digits.HOURS:
                        return hours;
                    case DigitFormat.Digits.MINUTES:
                        return minutes;
                    case DigitFormat.Digits.SECONDS:
                        return seconds;
                    default:
                        Debug.LogWarning("Digit value not supported");
                        return 0;
                }
            }

            switch (_digits)
            {
                case DigitFormat.Digits.HOURS:
                    return breakHours;
                case DigitFormat.Digits.MINUTES:
                    return breakMinutes;
                case DigitFormat.Digits.SECONDS:
                    return breakSeconds;
                default:
                    Debug.LogWarning("Digit value not supported");
                    return 0;
            }
        }

        public string GetTimerString()
        {
            return digitFormat.GetTimerString();
        }
        
        public bool GetIsOnBreak()
        {
            return isOnBreak;
        }

        public Theme GetTheme()
        {
            return theme;
        }
        
        /// <summary>
        /// Returns True if you can add one to this digit without hitting it's ceiling, otherwise returns False.
        /// </summary>
        /// <param name="_digits"></param>
        /// <returns></returns>
        public bool CanIncrementOne(DigitFormat.Digits _digits)
        {
            if (GetDigitValue(_digits) + 1 > digitFormat.GetDigitMax(_digits))
            {
                return false;
            }

            return true;
        }

        /// Returns True if you can subtract one to this digit without hitting it's floor, otherwise returns False.
        public bool CanDecrementOne(DigitFormat.Digits _digits)
        {
            if (GetDigitValue(_digits) - 1 < digitFormat.GetDigitMin())
            {
                return false;
            }

            return true;
        }

        // Setters
        
        // TODO: Move to DigitFormat class
        /// <summary>
        /// Sets the provided digit to it's provided value. (Will validate to make sure it's with bounds though)
        /// </summary>
        /// <param name="_digit"></param>
        /// <param name="_newValue"></param>
        public void SetDigit(DigitFormat.Digits _digit, int _newValue)
        {
            if (!isOnBreak)
            {
                switch (_digit)
                {
                    case DigitFormat.Digits.HOURS:
                        hours = _newValue;
                        break;
                    case DigitFormat.Digits.MINUTES:
                        minutes = _newValue;
                        break;
                    case DigitFormat.Digits.SECONDS:
                        seconds = _newValue;
                        break;
                }
            }
            else
            {
                switch (_digit)
                {
                    case DigitFormat.Digits.HOURS:
                        breakHours = _newValue;
                        break;
                    case DigitFormat.Digits.MINUTES:
                        breakMinutes = _newValue;
                        break;
                    case DigitFormat.Digits.SECONDS:
                        breakSeconds = _newValue;
                        break;
                }
            }

            OnValidate();
        }
        
        // TODO: Move to DigitFormat class
        /// <summary>
        /// Sets the value of the timer using the provided formatted string.
        /// </summary>
        /// <param name="_formattedString">Expected format of ("00:25:00")</param>
        public void SetTimerValue(string _formattedString)
        {
            // Only allow 'Set Timer Value' to work when we are in the setup state
            if (state != States.SETUP)
            {
                return;
            }
            
            List<string> _sections = new List<string>();
            string _value = String.Empty;

            // Determine how many sections there are
            // ReSharper disable once InconsistentNaming
            // ReSharper disable once ForCanBeConvertedToForeach
            for (int i = 0; i < _formattedString.Length; i++)
            {
                // If this character is a separator...
                if (_formattedString[i].ToString() == ":")
                {
                    // Save section
                    if (_value != String.Empty)
                    {
                        _sections.Add(_value);
                        _value = string.Empty;
                    }

                    continue;
                }

                // Add to section
                _value += _formattedString[i].ToString();
            }

            // Last digit in string won't have a separator so we add the section in once the loop is complete
            _sections.Add(_value);

            // Compare sections with timer format
            if (_sections.Count != 3) // TODO: Support more digit formats
            {
                Debug.LogWarning("The provided string does not match the pomodoro timer layout");
                return;
            }

            // Set timer sections
            // ReSharper disable once InconsistentNaming
            for (int i = 0; i < _sections.Count; i++)
            {
                switch (i)
                {
                    case 0:
                        SetHours(_sections[i]);
                        break;
                    case 1:
                        SetMinutes(_sections[i]);
                        break;
                    case 2:
                        SetSeconds(_sections[i]);
                        break;
                }
            }
        }

        [Obsolete("Please use SetDigit() instead. This will be deprecated / removed in future releases.")]
        public void SetHours(string _hours)
        {
            SetDigit(DigitFormat.Digits.HOURS, string.IsNullOrEmpty(_hours) ? 0 : int.Parse(_hours));
        }
        
        [Obsolete("Please use SetDigit() instead. This will be deprecated / removed in future releases.")]
        public void SetMinutes(string _minutes)
        {
            SetDigit(DigitFormat.Digits.MINUTES, string.IsNullOrEmpty(_minutes) ? 0 : int.Parse(_minutes));
        }

        [Obsolete("Please use SetDigit() instead. This will be deprecated / removed in future releases.")]
        public void SetSeconds(string _seconds)
        {
            SetDigit(DigitFormat.Digits.SECONDS, string.IsNullOrEmpty(_seconds) ? 0 : int.Parse(_seconds));
        }
        
        public void ColorUpdate(Theme _theme)
        {
            // TODO: check if info page is visible
            infoContainer.ColorUpdate(_theme);

            ColorScheme _currentColors = _theme.GetCurrentColorScheme();
            
            // State text
            text.color = _currentColors.backgroundHighlight;
            
            // Ring background
            ringBackground.material.SetColor(RingColor, _theme.GetCurrentColorScheme().backgroundHighlight);

            // Left Button Background
            Image _leftContainerTarget = leftButtonClick.containerTarget.GetComponent<Image>();
            if (_leftContainerTarget != null)
            {
                _leftContainerTarget.material.SetColor(CircleColor, _theme.GetCurrentColorScheme().backgroundHighlight);
            }
            
            // Left Button Icon
            SVGImage _leftVisibilityTarget = leftButtonClick.visibilityTarget.GetComponent<SVGImage>();
            if (_leftVisibilityTarget != null)
            {
                _leftVisibilityTarget.color = _currentColors.foreground;
            }
            
            // Right Button Background
            Image _rightContainerTarget = rightButtonClick.containerTarget.GetComponent<Image>();
            if (_rightContainerTarget != null)
            {
                _rightContainerTarget.material.SetColor(CircleColor, _currentColors.backgroundHighlight);
            }
            
            // Paused Digits
            startingColor = theme.GetCurrentColorScheme().foreground;
            endingColor = theme.GetCurrentColorScheme().backgroundHighlight;
            
            // Reset paused digit anim
            ResetDigitFadeAnim();

            switch (state)
            {
                case States.SETUP:
                    // Ring
                    ring.material.SetColor(RingColor,
                        !isOnBreak ? _theme.GetCurrentColorScheme().modeOne : _theme.GetCurrentColorScheme().modeTwo);

                    break;
                
                case States.RUNNING:
                    // Ring
                    ring.material.SetColor(RingColor, _theme.GetCurrentColorScheme().running);

                    break;
                
                case States.PAUSED:
                    // Ring
                    ring.material.SetColor(RingColor, 
                        !isOnBreak ? _theme.GetCurrentColorScheme().modeOne : _theme.GetCurrentColorScheme().modeTwo);
                    break;
                
                case States.COMPLETE:
                    // Ring
                    ring.material.SetColor(RingColor, _theme.GetCurrentColorScheme().complete);
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}