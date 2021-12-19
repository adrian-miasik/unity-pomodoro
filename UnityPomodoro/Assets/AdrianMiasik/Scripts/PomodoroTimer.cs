using System;
using System.Collections.Generic;
using System.Linq;
using AdrianMiasik.Components;
using AdrianMiasik.Components.Core;
using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
using AdrianMiasik.UWP;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AdrianMiasik
{
    public class PomodoroTimer : MonoBehaviour, IColorHook
    {
        public enum States
        {
            SETUP,
            RUNNING,
            PAUSED,
            COMPLETE
        }

        public States state = States.SETUP; // The timers current state
        
        [Header("Basic - Components")]
        [SerializeField] private TextMeshProUGUI text; // Text used to display current state
        [SerializeField] private Image ring; // Ring used to display timer progress
        [SerializeField] private Image ringBackground; // Theming
        
        [Header("Unity Pomodoro - Components")]
        [SerializeField] private Background background; // Used to pull select focus
        [SerializeField] private CompletionLabel completionLabel; // Used to prompt the user the timer is finished
        [SerializeField] private DigitFormat digitFormat; // Responsible class for manipulating our digits and formats
        [SerializeField] private BooleanToggle menuToggle; // Used to toggle our sidebar menu
        [SerializeField] private ClickButtonIcon leftButtonClick; // Used to restart the timer
        // TODO: Consolidate right buttons to a single class
        [SerializeField] private ClickButtonIcon rightButtonClick; // Used to play/pause the timer
        [SerializeField] private RightButton rightButton; // Additional timer state element 
        [SerializeField] private BooleanSlider breakSlider; // Used for switching timer between mode one and mode two
        [SerializeField] private CreditsBubble creditsBubble; // Used to display project contributors
        [SerializeField] private ThemeSlider themeSlider; // Used to change between light / dark mode
        [SerializeField] private BooleanToggle halloweenToggle; // Halloween theme toggle during Halloween week (Disabled by default) // TODO: Re-implement
        [SerializeField] private HotkeyDetector hotkeyDetector; // Responsible class for our keyboard shortcuts / bindings
        [SerializeField] private Sidebar sidebarMenu; // Used to change and switch between our pages / panel contents (Such as main, settings, and about)
        [SerializeField] private NotificationManager notifications; // Responsible class for UWP notifications and toasts
        private readonly List<ITimerState> timerElements = new List<ITimerState>();
        
        [Header("Animations")] 
        [SerializeField] private Animation spawnAnimation; // The timers introduction animation (plays on timer restarts)
        [SerializeField] private Animation completion; // The animation used to manipulate the completionLabel component (Wrap mode doesn't matter) TODO: Implement into completion label class instead
        [SerializeField] private AnimationCurve completeRingPulseDiameter = AnimationCurve.Linear(0, 0.9f, 1, 0.975f);
        [SerializeField] private float pauseFadeDuration = 0.1f;
        [SerializeField] private float pauseHoldDuration = 0.75f; // How long to wait between fade completions?
        [SerializeField] private AnimationCurve ringTickWidth;
        
        [Header("Unity Events")]
        // TODO: Rename UnityEvents to lowerCamelCase
        public UnityEvent OnRingPulse; // Invoked when the ring / timer alarm pulses
        public UnityEvent OnTimerCompletion; // Invoked when the timer finishes

        [Header("Cache")]
        [SerializeField] private List<DoubleDigit> selectedDigits = new List<DoubleDigit>(); // Contains our currently selected digits
        
        [Header("Pages / Panels - Deprecated")]
        // TODO: Create a page / panel solution
        [SerializeField] private GameObject mainContainer; // Our main body page
        [SerializeField] private SettingsPanel settingsContainer; // Our settings page
        [SerializeField] private AboutPanel aboutContainer; // Our about page

        [Header("External Extra - Deprecated")]
        // TODO: Move to theme class
        [SerializeField] private Theme theme; // Current active theme
        // TODO: Create dialog manager class
        [SerializeField] private TwoChoiceDialog confirmationDialogPrefab; // Prefab reference

        // Time
        private double currentTime; // Current time left (In seconds)
        private float totalTime; // Total time left (In seconds)
        private bool isTimerBeingSetup = true; // First time playing

        // Pause Fade Animation
        private bool isFading;
        private float accumulatedFadeTime;
        private float fadeProgress;
        private Color startingColor;
        private Color endingColor;
        private bool isFadeComplete;

        // Shader Properties
        private static readonly int RingColor = Shader.PropertyToID("Color_297012532bf444df807f8743bdb7e4fd");
        private static readonly int RingDiameter = Shader.PropertyToID("Vector1_98525729712540259c19ac6e37e93b62");
        private static readonly int CircleColor = Shader.PropertyToID("Color_297012532bf444df807f8743bdb7e4fd");

        // Cache
        // TODO: Move to settings class
        private bool muteSoundWhenOutOfFocus; // We want this to be true only for Windows platform due to UWP notifications
        // TODO: Move to dialog manager class
        private TwoChoiceDialog currentDialogPopup;
        
        // Pulse Ring Complete Animation
        private float accumulatedRingPulseTime;
        private bool hasRingPulseBeenInvoked;
        
        // Pulse Tick Ring Animation
        private readonly bool isRingTickAnimationEnabled = false; // TODO: Re-implement this animation? / Expose in settings?
        private float cachedSeconds;
        private bool isRingTickAnimating;

        private void OnApplicationFocus(bool _hasFocus)
        {
            if (muteSoundWhenOutOfFocus)
            {
                // Prevent application from making noise when not in focus
                AudioListener.volume = !_hasFocus ? 0 : 1;
            }
            else
            {
                AudioListener.volume = 1;
            }
        }

        private void Start()
        {
            // Single entry point
            ConfigureSettings();
            Initialize();
        }

        /// <summary>
        /// Configures our default settings based on Operating System using platform specific define directives
        /// </summary>
        private void ConfigureSettings()
        {
            // Set mute setting default
#if UNITY_STANDALONE_OSX
            SetMuteSoundWhenOutOfFocus();
#elif UNITY_STANDALONE_LINUX
            SetMuteSoundWhenOutOfFocus();
#elif UNITY_STANDALONE_WIN
            SetMuteSoundWhenOutOfFocus();
#elif UNITY_WSA // UWP
            SetMuteSoundWhenOutOfFocus(true); // Set to true since our UWP Notification will pull focus back to our app
#endif
        }

        /// <summary>
        /// Setup view, calculate time, initialize components, transition in, and animate.
        /// </summary>
        private void Initialize()
        {
            // Setup pages and panels
            settingsContainer.Hide();
            aboutContainer.Hide();
            mainContainer.gameObject.SetActive(true);

            // Overrides
            themeSlider.OverrideFalseColor(theme.light.backgroundHighlight);
            themeSlider.OverrideTrueColor(new Color(0.59f, 0.33f, 1f));
            menuToggle.OverrideFalseColor(theme.GetCurrentColorScheme().foreground);
            menuToggle.OverrideTrueColor(Color.clear);

            // TODO: Re-implement halloween theme?
            // Halloween Theme Toggle
            // Check if it's October...
            if (DateTime.Now.Month == 10)
            {
                // Check if it's Halloween week...
                for (int _i = 25; _i <= 31; _i++)
                {
                    // Is today Halloween week...
                    if (DateTime.Now.Day == _i)
                    {
                        halloweenToggle.gameObject.SetActive(true);
                        halloweenToggle.OverrideTrueColor(new Color(1f, 0.59f, 0f));
                        halloweenToggle.Initialize(this, false);
                        break;
                    }
                }
            }

            // Initialize components
            hotkeyDetector.Initialize(this);
            notifications.Initialize(this);
            background.Initialize(this);
            digitFormat.Initialize(this);
            completionLabel.Initialize(this);
            themeSlider.Initialize(this, !theme.isLightModeOn);
            creditsBubble.Initialize(this);
            rightButton.Initialize(this);
            menuToggle.Initialize(this, false);
            breakSlider.Initialize(this, false);
            sidebarMenu.Initialize(this);

            // Register elements that need updating per timer state change
            timerElements.Add(rightButton);

            // Calculate time
            CalculateTimeValues();

            // Transition to setup state
            SwitchState(States.SETUP);

            // Animate in
            PlaySpawnAnimation();

            // Setup theme
            theme.RegisterColorHook(this);

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
                    digitFormat.SetDigitColor(theme.GetCurrentColorScheme().foreground);
                    
                    // Show state text
                    text.gameObject.SetActive(true);

                    // Complete ring
                    ring.fillAmount = 1f;
                    ring.material.SetFloat(RingDiameter, 0.9f);
                    text.text = !digitFormat.isOnBreak ? "Set a work time" : "Set a break time";

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
                    digitFormat.SetDigitColor(theme.GetCurrentColorScheme().foreground);
                    
                    text.text = "Running";
                    
                    // Deselection
                    ClearSelection();
                    foreach (DoubleDigit _digit in selectedDigits)
                    {
                        _digit.Deselect();
                    }

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
            
            // TODO: Redundant?
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
                    Tick();
                    break;

                case States.COMPLETE:
                    AnimateRingPulse();
                    break;
            }
        }

        private void Tick()
        {
            if (currentTime > 0)
            {
                // Decrement timer
                currentTime -= Time.deltaTime;
                        
                // Update visuals
                ring.fillAmount = (float)currentTime / totalTime;
                digitFormat.ShowTime(TimeSpan.FromSeconds(currentTime));
     
                AnimateRingTickPulse();
            }
            else
            {
                SwitchState(States.COMPLETE);
                OnTimerComplete();
                OnTimerCompletion?.Invoke();
            }
        }

        private void OnTimerComplete()
        {
            if (currentDialogPopup != null)
            {
                currentDialogPopup.Close();
            }
        }

        /// <summary>
        /// Animates our ring width to pulse with each second change
        /// </summary>
        private void AnimateRingTickPulse()
        {
            if (!isRingTickAnimationEnabled)
            {
                return;
            }
            
            if (cachedSeconds != TimeSpan.FromSeconds(currentTime).Seconds)
            {
                isRingTickAnimating = true;
            }

            if (isRingTickAnimating)
            {
                accumulatedRingPulseTime += Time.deltaTime;
                ring.material.SetFloat(RingDiameter, ringTickWidth.Evaluate(accumulatedRingPulseTime));
            }
                            
            cachedSeconds = TimeSpan.FromSeconds(currentTime).Seconds;
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
                fadeProgress = accumulatedFadeTime / pauseFadeDuration;

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
        /// Updates our core timer values based on the digit format time span
        /// </summary>
        private void CalculateTimeValues()
        {
            TimeSpan _ts = digitFormat.GetTime();
            currentTime = _ts.TotalSeconds;
            totalTime = (float)_ts.TotalSeconds;
            digitFormat.SetTime(_ts);
            digitFormat.RefreshDigitVisuals();
        }
        
        // TODO: Create a panel/page class
        /// <summary>
        /// Shows about content, hides main content, and shows credits bubble
        /// </summary>
        public void ShowAbout()
        {
            if (!aboutContainer.IsInitialized())
            {
                aboutContainer.Initialize(this);
            }

            // Prevent tick animations from pausing when switching to info page
            // TODO: Do this for settings page too?
            digitFormat.CorrectTickAnimVisuals();
            
            // Hide other content
            mainContainer.gameObject.SetActive(false);
            settingsContainer.Hide();
            
            // Show about page content
            aboutContainer.Show();

            // Special behaviour that's used to display/open up credits bubble when on this page
            if (!creditsBubble.IsRunning())
            {
                creditsBubble.Lock();
                creditsBubble.FadeIn();
            }
        }

        /// <summary>
        /// Shows main content, hides info, and hides credits bubble
        /// </summary>
        public void ShowMainContent()
        {
            // Hide other content
            aboutContainer.Hide();
            settingsContainer.Hide();
            
            // Show main content
            mainContainer.gameObject.SetActive(true);
            digitFormat.GenerateFormat();
            digitFormat.ShowTime(TimeSpan.FromSeconds(currentTime)); // Update visuals to current time
            
            // Reset digit animation timings when opening/re-opening this page
            if (state == States.PAUSED)
            {
                ResetDigitFadeAnim();
            }

            // Hide / close out credits bubble
            creditsBubble.Unlock();
            creditsBubble.FadeOut();
        }

        public void ShowSettings()
        {
            if (!settingsContainer.IsInitialized())
            {
                settingsContainer.Initialize(this);
            }

            // Hide other content
            aboutContainer.Hide();
            mainContainer.gameObject.SetActive(false);
            
            // Show settings content
            settingsContainer.Show();
            
            // Hide / close out credits bubble
            creditsBubble.Unlock();
            creditsBubble.FadeOut();
        }
        
        /// <summary>
        /// Transitions timer into States.RUNNING mode
        /// </summary>
        public void Play()
        {
            if (isTimerBeingSetup)
            {
                isTimerBeingSetup = false;
                CalculateTimeValues();
            }

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
            if (!isTimerBeingSetup && state != States.COMPLETE)
            {
                SpawnConfirmationDialog(() =>
                {
                    SwitchTimer(true);
                }, () =>
                {
                    // Switch To Break Timer is triggered by Unity Event via break slider, this simply reset's 
                    // our bool back to it's original setting prior to interaction.
                    breakSlider.Initialize(this, IsOnBreak()); // Re-init component
                });
            }
            else
            {
                SwitchTimer(true);
            }
        }

        private void SwitchTimer(bool _isOnBreak)
        {
            digitFormat.isOnBreak = _isOnBreak;
            SwitchState(States.SETUP);
            isTimerBeingSetup = true;
            CalculateTimeValues();
        }

        /// <summary>
        /// Transitions timer into States.SETUP mode in work mode
        /// </summary>
        public void SwitchToWorkTimer()
        {
            if (!isTimerBeingSetup && state != States.COMPLETE)
            {
                SpawnConfirmationDialog(() =>
                {
                    SwitchTimer(false);
                }, (() =>
                {
                    breakSlider.Initialize(this, IsOnBreak()); // Re-init component
                }));
            }
            else
            {
                SwitchTimer(false);
            }
        }

        /// <summary>
        /// Attempts to restart the timer, will prompt user with confirmation dialog if necessary.
        /// </summary>
        public void TryRestart(bool _isComplete)
        {
            if (!isTimerBeingSetup && state != States.COMPLETE)
            {
                SpawnConfirmationDialog((() =>
                {
                    Restart(_isComplete);
                }));
            }
            else
            {
                Restart(_isComplete);
            }
        }
        
        /// <summary>
        /// Directly toggles the timer mode to it's opposite mode (break/work) and transitions timer into States.SETUP.
        /// Note: If you want to verify this action, <see cref="TryRestart"/>. <see cref="TryRestart"/> will prompt the
        /// user with a confirmation dialog if necessary.
        /// </summary>
        /// <param name="_isCompleted"></param>
        public void Restart(bool _isCompleted)
        {
            if (_isCompleted)
            {
                digitFormat.FlipIsOnBreakBool();
            }

            SwitchState(States.SETUP);
            isTimerBeingSetup = true;
            CalculateTimeValues();

            // Stop digit tick animation
            digitFormat.ResetTextPositions();
            
            PlaySpawnAnimation();
        }

        #region Button/Keyboard OnClick Events
        /// <summary>
        /// Activates the play/pause button to toggle the timer state (States.SETUP, etc...)
        /// </summary>
        public void TriggerPlayPause()
        {
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
            themeSlider.Interact();
        }
        #endregion

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
        public string GetTimerString()
        {
            return digitFormat.GetTimerString();
        }
        
        public int GetDigitFormat()
        {
            return digitFormat.GetFormat();
        }
        
        public List<Selectable> GetSelections()
        {
            return selectedDigits.Select(_doubleDigit => _doubleDigit.GetSelectable()).ToList();
        }
        
        public bool IsOnBreak()
        {
            return digitFormat.isOnBreak;
        }

        public bool IsAboutPageOpen()
        {
            return aboutContainer.IsInfoPageOpen();
        }
        
        public bool IsSidebarOpen()
        {
            return sidebarMenu.IsOpen();
        }
        
        // Setters
        public void SetTimerValue(string _timeString)
        {
            digitFormat.SetTimerValue(_timeString);
        }
        
        public void SetBackgroundNavigation(Navigation _backgroundNav)
        {
            background.SetSelectionNavigation(_backgroundNav);
        }

        /// <summary>
        /// Attempts to change the digit format using enum index, will prompt user with confirmation dialog if necessary.
        /// </summary>
        /// <param name="_i"></param>
        public void TryChangeFormat(Int32 _i)
        {
            TryChangeFormat((DigitFormat.SupportedFormats)_i);
        }

        /// <summary>
        /// Attempts to change the digit format, will prompt user with confirmation dialog if necessary.
        /// </summary>
        /// <param name="_desiredFormat"></param>
        public void TryChangeFormat(DigitFormat.SupportedFormats _desiredFormat)
        {
            if (!isTimerBeingSetup)
            {
                digitFormat.SwitchFormat(_desiredFormat);
                SpawnConfirmationDialog(() =>
                {
                    GenerateFormat(_desiredFormat);
                }, () =>
                {
                    settingsContainer.SetDropdown(digitFormat.GetPreviousFormatSelection());
                });
            }
            else
            {
                digitFormat.SwitchFormat(_desiredFormat);
                GenerateFormat(_desiredFormat);
            }
        }

        /// <summary>
        /// Changes the format directly
        /// </summary>
        /// <param name="_desiredFormat"></param>
        private void GenerateFormat(DigitFormat.SupportedFormats _desiredFormat)
        {
            digitFormat.GenerateFormat();
            Restart(false); // Restart timer directly, don't verify user intent
                                      // since we're doing that in this scope.
            
            if (settingsContainer.IsPageOpen())
            {
                settingsContainer.UpdateDropdown();
            }
        }

        /// <summary>
        /// Apply our color updates to relevant components
        /// </summary>
        /// <param name="_theme"></param>
        public void ColorUpdate(Theme _theme)
        {
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
            leftButtonClick.icon.color = _currentColors.foreground;

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
            
            menuToggle.OverrideFalseColor(theme.GetCurrentColorScheme().foreground);
            menuToggle.ColorUpdate(theme);

            switch (state)
            {
                case States.SETUP:
                    // Ring
                    ring.material.SetColor(RingColor,
                        !digitFormat.isOnBreak ? _theme.GetCurrentColorScheme().modeOne : _theme.GetCurrentColorScheme().modeTwo);

                    break;
                
                case States.RUNNING:
                    // Ring
                    ring.material.SetColor(RingColor, _theme.GetCurrentColorScheme().running);

                    break;
                
                case States.PAUSED:
                    // Ring
                    ring.material.SetColor(RingColor, 
                        !digitFormat.isOnBreak ? _theme.GetCurrentColorScheme().modeOne : _theme.GetCurrentColorScheme().modeTwo);
                    break;
                
                case States.COMPLETE:
                    // Ring
                    ring.material.SetColor(RingColor, _theme.GetCurrentColorScheme().complete);
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // TODO: Create theme manager class?
        public Theme GetTheme()
        {
            return theme;
        }
        
        // Unity Event
        public void SetToLightMode()
        {
            theme.SetToLightMode();
        }

        // Unity Event
        public void SetToDarkMode()
        {
            theme.SetToDarkMode();
        }
        
        // Unity Event
        public void SwitchTheme(Theme _desiredTheme)
        {
            // Transfer elements to new theme (So theme knows which elements to color update)
            theme.TransferColorElements(theme, _desiredTheme);
            
            // Swap our theme
            theme = _desiredTheme;
            
            // Apply our changes
            theme.ApplyColorChanges();
        }
        
        public void ColorUpdateCreditsBubble()
        {
            creditsBubble.ColorUpdate(theme);
        }

        // TODO: Create settings class / scriptable object
        public bool MuteSoundWhenOutOfFocus()
        {
            return muteSoundWhenOutOfFocus;
        }

        public void SetMuteSoundWhenOutOfFocus(bool _state = false)
        {
            muteSoundWhenOutOfFocus = _state;
        }

        private void SpawnConfirmationDialog(Action _onSubmit, Action _onCancel = null)
        {
            if (currentDialogPopup == null)
            {
                currentDialogPopup = Instantiate(confirmationDialogPrefab, transform);
                currentDialogPopup.Initialize(this, _onSubmit, _onCancel);
            }
        }

        public void ClearDialogPopup(TwoChoiceDialog _dialog)
        {
            if (_dialog == currentDialogPopup)
            {
                currentDialogPopup = null;
            }
        }
    }
}