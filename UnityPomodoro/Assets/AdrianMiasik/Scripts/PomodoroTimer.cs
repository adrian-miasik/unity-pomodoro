using System;
using System.Collections.Generic;
using System.Linq;
using AdrianMiasik.Android;
using AdrianMiasik.Components.Core;
using AdrianMiasik.Components.Core.Containers;
using AdrianMiasik.Components.Core.Items;
using AdrianMiasik.Components.Core.Settings;
using AdrianMiasik.Components.Specific;
using AdrianMiasik.Components.Specific.Pages;
using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
using AdrianMiasik.UWP;
using LeTai.Asset.TranslucentImage;
using TMPro;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AdrianMiasik
{
    /// <summary>
    /// Our main class / component. Responsible for controlling the main timer logic, configuring settings,
    /// initializing, and manipulating our components.
    /// </summary>
    public class PomodoroTimer : MonoBehaviour, IColorHook
    {
        /// <summary>
        /// The different modes the timer could be using.
        /// </summary>
        public enum States
        {
            /// <summary>
            /// Digits are editable by the user, time does not progress.
            /// </summary>
            SETUP,
            /// <summary>
            /// Digits are not allowed to be interacted with, and the time left is decreasing over time.
            /// </summary>
            RUNNING,
            /// <summary>
            /// Digits are not allowed to be interacted with, time does not progress.
            /// </summary>
            PAUSED,
            /// <summary>
            /// Digits are no longer visible, completion label is visible.
            /// </summary>
            COMPLETE
        }
        
        /// <summary>
        /// The timer's current state. See enum <see cref="States"/>
        /// </summary>
        public States m_state = States.SETUP;

        [Header("Unity Pomodoro - Managers")]
        [SerializeField] private ThemeManager m_themeManager; // Responsible class for executing and keeping track of themed elements and active themes.
        [SerializeField] private HotkeyDetector m_hotkeyDetector; // Responsible class for our keyboard shortcuts / bindings
        [SerializeField] private ConfirmationDialogManager m_confirmationDialogManager;
        [SerializeField] private UWPNotifications m_uwpNotifications; // Responsible class for UWP notifications and toasts
        [SerializeField] private AndroidNotifications m_androidNotifications;

        [Header("Basic - Components")]
        [SerializeField] private TextMeshProUGUI m_text; // Text used to display current state
        [SerializeField] private Image m_ring; // Ring used to display timer progress
        [SerializeField] private Image m_ringBackground; // Theming
        
        [Header("Unity Pomodoro - Components")]
        [SerializeField] private Background m_background; // Used to pull select focus
        [SerializeField] private BlurOverlay m_overlay; // Used to blur background on sidebar focus and confirmation dialog pop-ups.
        [SerializeField] private TranslucentImageSource m_translucentImageSource; // Necessary reference for blur
        [SerializeField] private CompletionLabel m_completionLabel; // Used to prompt the user the timer is finished
        [SerializeField] private DigitFormat m_digitFormat; // Responsible class for manipulating our digits and formats
        [SerializeField] private ToggleSprite m_menuToggleSprite; // Used to toggle our sidebar menu
        [SerializeField] private ClickButtonSVGIcon m_leftButtonSVGClick; // Used to restart the timer
        [SerializeField] private RightButton m_rightButton; // Used to play/pause timer + timer sprite state element 
        [SerializeField] private ToggleSlider m_breakSlider; // Used for switching timer between mode one and mode two
        [SerializeField] private CreditsGhost m_creditsGhost; // Used to display project contributors
        [SerializeField] private ThemeSlider m_themeSlider; // Used to change between light / dark mode
        [SerializeField] private Sidebar m_sidebarMenu; // Used to change and switch between our pages / panel contents (Such as main, settings, and about)
        [SerializeField] private TomatoCounter m_tomatoCounter; // Responsible class for counting work / break timers and providing a long break
        [SerializeField] private EndTimestampGhost m_endTimestampGhost; // Responsible for displaying the local end time for the current running timer.
        [SerializeField] private SkipButton m_skipButton;
        private readonly List<ITimerState> timerElements = new List<ITimerState>();
        
        [Header("Animations")] 
        [SerializeField] private AnimationCurve m_spawnRingProgress;
        private bool m_animateRingProgress;
        private float m_accumulatedRingAnimationTime;
        [SerializeField] private Animation m_spawnAnimation; // The timers introduction animation (plays on timer restarts)
        [SerializeField] private AnimationCurve m_completeRingPulseDiameter = AnimationCurve.Linear(0, 0.9f, 1, 0.975f);
        [SerializeField] private float m_pauseFadeDuration = 0.1f;
        [SerializeField] private float m_pauseHoldDuration = 0.75f; // How long to wait between fade completions?
        [SerializeField] private AnimationCurve m_ringTickWidth;

        /// <summary>
        /// A UnityEvent that gets invoked when the spawn animation is complete.
        /// </summary>
        [Header("Unity Events")]
        public UnityEvent m_onSpawnCompletion;
        
        /// <summary>
        /// A UnityEvent that gets invoked when the ring / timer alarm pulses.
        /// </summary>
        public UnityEvent m_onRingPulse;
        
        /// <summary>
        /// A UnityEvent that gets invoked when the timer finishes. (<see cref="States.COMPLETE"/>)
        /// </summary>
        public UnityEvent m_onTimerCompletion;

        [Header("Cache")]
        [SerializeField] private List<DoubleDigit> m_selectedDigits = new List<DoubleDigit>(); // Contains our currently selected digits

        [Header("Pages")] 
        [SerializeField] private SidebarPages m_sidebarPages;

        // Time
        private double currentTime; // Current time left (In seconds)
        private float totalTime; // Total time (In seconds)
        private bool isTimerBeingSetup = true; // First time playing
        private TimeSpan focusLoss;

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
        public static readonly int CircleColor = Shader.PropertyToID("Color_297012532bf444df807f8743bdb7e4fd");

        // Pulse Ring Complete Animation
        private bool disableCompletionAnimation;
        private float accumulatedRingPulseTime;
        private bool hasRingPulseBeenInvoked;

        [Header("Loaded Settings")]
        [SerializeField] private SystemSettings loadedSystemSettings;
        [SerializeField] private TimerSettings loadedTimerSettings;
        private bool haveSettingsBeenConfigured;

        private void OnApplicationFocus(bool hasFocus)
        {
            if (loadedSystemSettings.m_muteSoundWhenOutOfFocus)
            {
                // Prevent application from making noise when not in focus
                AudioListener.volume = !hasFocus ? 0 : 1;
            }
            else
            {
                AudioListener.volume = 1;
            }

            if (m_state != States.RUNNING)
            {
                // Early Exit
                return;
            }
            
            if (!hasFocus)
            {
                // Cache the time we lost focus.
                focusLoss = DateTime.Now.TimeOfDay;
                
                Application.targetFrameRate = 0;
            }
            else
            {
                // Calculate the new time progress based on our focus loss time.
                double secondsPassedSinceFocusLoss = DateTime.Now.TimeOfDay.TotalSeconds - focusLoss.TotalSeconds;
                currentTime -= secondsPassedSinceFocusLoss;
                
                Application.targetFrameRate = Screen.currentResolution.refreshRate;
            }
        }

        private void OnValidate()
        {
            if (!haveSettingsBeenConfigured)
            {
                return;
            }
            
            // Updating settings visuals
            m_themeSlider.Refresh();
            m_sidebarPages.RefreshSettingsPage();
        }

        void Awake()
        {
            // Single entry point
            ConfigureSettings();
            Initialize();
        }

        /// <summary>
        /// Loads / Creates settings file for the system and timer.
        /// </summary>
        private void ConfigureSettings()
        {
            SystemSettings systemSettings = UserSettingsSerializer.LoadSystemSettings();
            TimerSettings timerSettings = UserSettingsSerializer.LoadTimerSettings();

            // System Settings
            if (systemSettings == null)
            {
                Debug.Log("No System settings found. Created new System settings successfully!");
                
                // Create new settings
                SystemSettings defaultSystemSettings = new SystemSettings();
                defaultSystemSettings.m_darkMode = false;

                // Apply mute out of focus
#if UNITY_STANDALONE_OSX
                defaultSystemSettings.m_muteSoundWhenOutOfFocus = false;
#elif UNITY_STANDALONE_LINUX
                defaultSystemSettings.m_muteSoundWhenOutOfFocus = false;
#elif UNITY_STANDALONE_WIN
                defaultSystemSettings.m_muteSoundWhenOutOfFocus = false;
#elif UNITY_WSA // UWP
                defaultSystemSettings.m_muteSoundWhenOutOfFocus = true; // Set to true since our UWP Notification will pull focus back to our app
#elif UNITY_ANDROID
                defaultSystemSettings.m_muteSoundWhenOutOfFocus = false; // Doesn't quite matter for mobile
#elif UNITY_IOS
                defaultSystemSettings.m_muteSoundWhenOutOfFocus = false; // Doesn't quite matter for mobile.
#endif
                
                // All platforms have analytics on by default. (User can opt-out though via settings panel)
                defaultSystemSettings.m_enableUnityAnalytics = true;
                
                // Cache
                systemSettings = defaultSystemSettings;
                UserSettingsSerializer.SaveSystemSettings(systemSettings);
            }

            loadedSystemSettings = systemSettings;

            // Apply theme changes
            m_themeManager.Register(this);
            if (loadedSystemSettings.m_darkMode)
            {
                m_themeManager.SetToDarkMode();
            }
            else
            {
                m_themeManager.SetToLightMode();
            }
            
            // Timer Settings
            // If we don't have any saved settings...
            if (timerSettings == null)
            {
                Debug.Log("No Timer settings found. Created new Timer settings successfully!");
                
                // Create new settings
                TimerSettings defaultTimerSettings = new TimerSettings();
                defaultTimerSettings.m_longBreaks = true;
                defaultTimerSettings.m_format = DigitFormat.SupportedFormats.HH_MM_SS;
                defaultTimerSettings.m_pomodoroCount = 4;

                // Cache
                timerSettings = defaultTimerSettings;
                UserSettingsSerializer.SaveTimerSettings(timerSettings);
            }
            
            loadedTimerSettings = timerSettings;

            // Set target frame rate to refresh rate to prevent possible unnecessary GPU usage.
            Application.targetFrameRate = Screen.currentResolution.refreshRate;
            
            haveSettingsBeenConfigured = true;

#if ENABLE_CLOUD_SERVICES_ANALYTICS
            // Update analytics
            ToggleUnityAnalytics(this.loadedSystemSettings.m_enableUnityAnalytics, true);
#endif
        }

#if ENABLE_CLOUD_SERVICES_ANALYTICS
        /// <summary>
        /// Enables / Disables our Unity Analytics Service and setting option.
        /// Note: To fully disable analytics, we need to set initializeOnStartup to false and restart our app.
        /// There seems to be no way to "turn off" the analytics service once it's been initialized and turned on.
        /// The disable analytics code does not return our intended values, hence the hard restart required.
        /// </summary>
        /// <param name="enableUnityAnalytics"></param>
        /// <param name="isBootingUp"></param>
        public void ToggleUnityAnalytics(bool enableUnityAnalytics, bool isBootingUp)
        {
            // Apply and save
            GetSystemSettings().m_enableUnityAnalytics = enableUnityAnalytics;
            UserSettingsSerializer.SaveSystemSettings(GetSystemSettings());
            
            // Set if service needs to start on init...
            Analytics.initializeOnStartup = enableUnityAnalytics;
#if UNITY_ANALYTICS_EVENT_LOGS
            Debug.LogWarning("Unity Analytics - Initialize on startup: " + Analytics.initializeOnStartup);
#endif

            if (enableUnityAnalytics)
            {
                // Enable analytics
                StartServices(isBootingUp);
            }
            else
            {
                // Disable analytics
                Analytics.enabled = false;
                PerformanceReporting.enabled = false;
                Analytics.limitUserTracking = true;
                Analytics.deviceStatsEnabled = false;

#if UNITY_ANALYTICS_EVENT_LOGS
                Debug.LogWarning("Unity Analytics - Stopped Service. " +
                                 "(Service will still cache some events into it's buffer it seems, but won't upload " +
                                 "them.)");
#endif
            }
        }
        
        /// <summary>
        /// Starts up our Unity Analytics service.
        /// </summary>
        /// <param name="isBootingUp"></param>
        async void StartServices(bool isBootingUp)
        {
            try
            {
                // Debug.LogWarning("Unity Analytics - Starting Up Service...");
                
                await UnityServices.InitializeAsync();
                List<string> consentIdentifiers = await Events.CheckForRequiredConsents();
                
#if UNITY_ANALYTICS_EVENT_LOGS
                Debug.LogWarning("Unity Analytics - Service Started.");
#endif
                
                // Enable analytics
                Analytics.enabled = true;
                PerformanceReporting.enabled = true;
                Analytics.limitUserTracking = false;
                Analytics.deviceStatsEnabled = true;

                if (isBootingUp)
                {
                    // Send enabled event log
                    Dictionary<string, object> parameters = new Dictionary<string, object>()
                    {
                        { "testingKey", "testingValue123Init" },
                    };
                    Events.CustomData("analyticsInitialized", parameters);
                    Events.Flush();
                }
                else
                {
                    // Send enabled event log
                    Dictionary<string, object> parameters = new Dictionary<string, object>()
                    {
                        { "testingKey", "testingValue123Enabled" },
                    };
                    Events.CustomData("analyticsEnabled", parameters);
                    Events.Flush();
                }
            }
            catch (ConsentCheckException e)
            {
               
            }
        }
#endif

        /// <summary>
        /// Setup view, calculate time, initialize components, transition in, and animate.
        /// </summary>
        private void Initialize()
        {
            InitializeManagers();

            // Overrides
            m_breakSlider.OverrideFalseColor(m_themeManager.GetTheme().GetCurrentColorScheme().m_modeOne);
            m_breakSlider.OverrideTrueColor(m_themeManager.GetTheme().GetCurrentColorScheme().m_modeTwo);
            m_themeSlider.OverrideFalseColor(m_themeManager.GetTheme().m_light.m_backgroundHighlight);
            m_themeSlider.OverrideTrueColor(new Color(0.59f, 0.33f, 1f));
            m_menuToggleSprite.OverrideFalseColor(m_themeManager.GetTheme().GetCurrentColorScheme().m_foreground);
            m_menuToggleSprite.OverrideTrueColor(Color.clear);
            
            InitializeComponents();
            
            // Switch view
            m_sidebarPages.SwitchToTimerPage();

            // Calculate time
            CalculateTimeValues();

            // Transition to setup state
            SwitchState(States.SETUP);

            // Animate in
            PlaySpawnAnimation();
        }

        /// <summary>
        /// Initializes our manager classes.
        /// </summary>
        private void InitializeManagers()
        {
            m_hotkeyDetector.Initialize(this);
            m_confirmationDialogManager.Initialize(this);
            
            // UWP Toast / Notification
            m_uwpNotifications.Initialize(GetSystemSettings());
            m_onTimerCompletion.AddListener(m_uwpNotifications.ShowToast);
            
#if UNITY_ANDROID
            // Android Notification
            m_androidNotifications.Initialize(this);
#endif
            
            // Register elements that need updating per timer state change
#if UNITY_ANDROID
            timerElements.Add(m_androidNotifications);
#endif
        }

        /// <summary>
        /// Hooks up our button functionality, initializes our pomodoro timer components, and registers our
        /// ITimerState interfaced elements.
        /// </summary>
        private void InitializeComponents()
        {
            // Restart Button
            m_leftButtonSVGClick.m_onClick.AddListener(() =>
            {
                TryRestart(false);
            });

            // Play / Pause Button
            m_rightButton.m_playOnClick.AddListener(Play);
            m_rightButton.m_pauseOnClick.AddListener(Pause);
            m_rightButton.m_snoozeOnClick.AddListener(() =>
            {
                Restart(true);
                m_breakSlider.Press();
            });

            // Switch Timer / Mode Slider
            m_breakSlider.m_onSetToTrueClick.AddListener(TrySwitchToBreakTimer);
            m_breakSlider.m_onSetToFalseClick.AddListener(TrySwitchToWorkTimer);
            
            m_menuToggleSprite.m_onSetToTrueClick.AddListener(m_sidebarMenu.Open);
            m_menuToggleSprite.m_onSetToFalseClick.AddListener(m_sidebarMenu.Close);
            
            // Components
            m_background.Initialize(this);
            m_overlay.Initialize(this);
            m_digitFormat.Initialize(this, GetTimerSettings().m_format);
            m_completionLabel.Initialize(this);
            m_themeSlider.Initialize(this);
            m_creditsGhost.Initialize(this);
            m_rightButton.Initialize(this);
            m_menuToggleSprite.Initialize(this, false);
            m_breakSlider.Initialize(this, false);
            m_sidebarMenu.Initialize(this);
            m_endTimestampGhost.Initialize(this);

            m_skipButton.Initialize(this);
            if (GetTimerSettings().m_longBreaks)
            {
                m_tomatoCounter.Initialize(this, GetTimerSettings().m_pomodoroCount);
                m_completionLabel.MoveAnchors(true);
            }
            else
            {
                m_tomatoCounter.gameObject.SetActive(false);
                m_completionLabel.MoveAnchors(false);
            }
            
            m_sidebarPages.Initialize(this);

            // Register elements that need updating per timer state change
            timerElements.Add(m_rightButton);
            timerElements.Add(m_completionLabel);
            timerElements.Add(m_endTimestampGhost);
            timerElements.Add(m_skipButton);
        }
        
        /// <summary>
        /// Unity's OnDestroy(). Deregisters self from <see cref="Theme"/> on destruction.
        /// </summary>
        public void OnDestroy()
        {
            // Make sure to deregister this when and if we do destroy the timer
            m_themeManager.GetTheme().Deregister(this);
        }
        
        /// <summary>
        /// Switches the timer to the provided state and handles all visual changes.
        /// Basically handles our transitions between timer states. <see cref="PomodoroTimer.States"/>
        /// </summary>
        /// <param name="desiredState">The state you want to transition to</param>
        public void SwitchState(States desiredState)
        {
            m_state = desiredState;
            Theme theme = m_themeManager.GetTheme();

            // Update the registered timer elements
            foreach (ITimerState element in timerElements)
            {
                element.StateUpdate(m_state, theme);
            }
            
            UpdateRingColor(theme);

            // Do transition logic
            switch (m_state)
            {
                case States.SETUP:
                    m_digitFormat.SetDigitColor(theme.GetCurrentColorScheme().m_foreground);
                    
                    // Show timer context
                    m_text.gameObject.SetActive(true);
                    
                    // Complete ring
                    m_ring.fillAmount = 1f;
                    m_ring.material.SetFloat(RingDiameter, 0.9f);
                    if (!m_digitFormat.m_isOnBreak)
                    {
                        m_text.text = "Set a work time";
                    }
                    else
                    {
                        m_text.text = !IsOnLongBreak() ? "Set a break time" : "Set a long break time";
                    }

                    // Show digits and hide completion label
                    m_digitFormat.Show();

                    isFading = false;
                    accumulatedRingPulseTime = 0;

                    ClearSelection();
                    
                    // Unlock editing
                    m_digitFormat.Unlock();
                    break;

                case States.RUNNING:
                    m_animateRingProgress = false;

                    m_digitFormat.SetDigitColor(theme.GetCurrentColorScheme().m_foreground);
                    
                    m_text.text = "Running";
                    
                    // Deselection
                    ClearSelection();
                    foreach (DoubleDigit digit in m_selectedDigits)
                    {
                        digit.Deselect();
                    }

                    // Lock Editing
                    m_digitFormat.Lock();
                    
                    break;

                case States.PAUSED:
                    m_text.text = "Paused";
                    ResetDigitFadeAnim();
                    break;

                case States.COMPLETE:

                    // Hide state text
                    m_text.gameObject.SetActive(false);

                    // Complete ring
                    m_ring.fillAmount = 1f;

                    // Hide digits and reveal completion
                    m_spawnAnimation.Stop();
                    m_digitFormat.Hide();

                    m_onRingPulse.Invoke();
                    break;
            }
        }

        private void ResetDigitFadeAnim()
        {
            accumulatedFadeTime = 0;
            isFadeComplete = true;
            isFading = true;
            accumulatedFadeTime = 0f;
        }
        
        /// <summary>
        /// Plays our timer spawning animation.
        /// <remarks>Used as a UnityEvent</remarks>
        /// </summary>
        public void PlaySpawnAnimation()
        {
            m_spawnAnimation.Stop();
            m_spawnAnimation.Play();

            m_accumulatedRingAnimationTime = 0;
            m_animateRingProgress = true;
        }
        
        /// <summary>
        /// Removes any digit selection, and selects the background (our default selection).
        /// </summary>
        public void ClearSelection()
        {
            SetSelection(null);
            m_background.Select();
        }
        
        /// <summary>
        /// Sets the selection to a single <see cref="DoubleDigit"/> and calculates text visibility based on new
        /// selection data. If you'd like to select multiple digits: See <see cref="AddSelection"/>.
        /// </summary>
        /// <param name="currentDigit"></param>
        public void SetSelection(DoubleDigit currentDigit)
        {
            foreach (DoubleDigit digit in m_selectedDigits)
            {
                // Deselect previous digit selections
                if (digit != currentDigit)
                {
                    digit.Deselect();
                }
            }

            m_selectedDigits.Clear();
            if (currentDigit != null)
            {
                m_selectedDigits.Add(currentDigit);
            }
            
            CalculateContextVisibility();
        }
        
        /// <summary>
        /// Determines timer context visibility depending on the selected digits and timer state.
        /// </summary>
        private void CalculateContextVisibility()
        {
            // Hide/show timer context
            if (m_selectedDigits.Count <= 0)
            {
                if (m_state != States.COMPLETE)
                {
                    m_text.gameObject.SetActive(true);
                    if (GetTimerSettings().m_longBreaks)
                    {
                        m_tomatoCounter.gameObject.SetActive(true);
                    }
                }
            }
            else
            {
                m_text.gameObject.SetActive(false);
                if (GetTimerSettings().m_longBreaks)
                {
                    m_tomatoCounter.gameObject.SetActive(false);
                }
            }
        }

        private void Update()
        {
            if (m_animateRingProgress)
            {
                m_ring.fillAmount = m_spawnRingProgress.Evaluate(m_accumulatedRingAnimationTime);
                m_accumulatedRingAnimationTime += Time.deltaTime;

                // If completed...
                if (m_accumulatedRingAnimationTime > m_spawnRingProgress.keys[m_spawnRingProgress.length-1].time)
                {
                    m_ring.fillAmount = 1;
                    m_animateRingProgress = false;
                    
                    m_onSpawnCompletion?.Invoke();
                }
            }
            
            switch (m_state)
            {
                case States.PAUSED:
                    AnimatePausedDigits();
                    break;

                case States.RUNNING:
                    Tick();
                    break;

                case States.COMPLETE:
                    if (disableCompletionAnimation)
                    {
                        return;
                    }
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
                m_ring.fillAmount = (float)currentTime / totalTime;
                m_digitFormat.ShowTime(TimeSpan.FromSeconds(currentTime));
            }
            else
            {
                CompleteTimer();
            }
        }
        
        private void CompleteTimer()
        {
            OnTimerComplete();
            m_onTimerCompletion?.Invoke();
        }

        private void OnTimerComplete()
        {
            SwitchState(States.COMPLETE);
            
            m_confirmationDialogManager.TryClearCurrentDialogPopup();
            
            // If timer completion was based on work/mode one timer
            // (We don't add tomatoes for breaks)
            if (!IsOnBreak() && !IsOnLongBreak() && m_state != States.SETUP)
            {
                if (GetTimerSettings().m_longBreaks)
                {
                    m_tomatoCounter.FillTomato();
                }
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
                if (accumulatedFadeTime > m_pauseHoldDuration)
                {
                    isFadeComplete = false;
                    accumulatedFadeTime = 0;
                }
            }
            else
            {
                fadeProgress = accumulatedFadeTime / m_pauseFadeDuration;

                m_digitFormat.SetDigitColor(isFading
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
            float ringDiameter = m_completeRingPulseDiameter.Evaluate(accumulatedRingPulseTime);

            // Set diameter
            m_ring.material.SetFloat(RingDiameter, ringDiameter);
            m_completionLabel.SetScale(ringDiameter);
            
            // Scale tomatoes too
            m_tomatoCounter.SetHorizontalScale(Vector3.one * ringDiameter);

            if (!hasRingPulseBeenInvoked)
            {
                m_onRingPulse.Invoke();
                hasRingPulseBeenInvoked = true;
            }

            // Ignore wrap mode and replay completion animation from start
            if (hasRingPulseBeenInvoked && accumulatedRingPulseTime >
                m_completeRingPulseDiameter[m_completeRingPulseDiameter.length - 1].time)
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
            TimeSpan ts = m_digitFormat.GetTime();
            currentTime = ts.TotalSeconds;
            totalTime = (float)ts.TotalSeconds;
            m_digitFormat.SetTime(ts);
            m_digitFormat.RefreshDigitVisuals();
        }

        public double GetCurrentTime()
        {
            return currentTime;
        }
        
        /// <summary>
        /// Shows about content, hides main content, and shows credits bubble.
        /// </summary>
        public void ShowAbout()
        {
            m_sidebarPages.SwitchToAboutPage();
                
            // Special behaviour that's used to display/open up credits bubble when on this page
            m_creditsGhost.Lock();
            m_creditsGhost.FadeIn();
        }

        /// <summary>
        /// Shows main content, hides info, and hides credits bubble
        /// </summary>
        public void ShowMainContent()
        {
            m_sidebarPages.SwitchToTimerPage();
            
            m_digitFormat.GenerateFormat();
            m_digitFormat.ShowTime(TimeSpan.FromSeconds(currentTime)); // Update visuals to current time
            
            if (m_state != States.SETUP)
            {
                m_digitFormat.Lock();
            }
            else
            {
                m_digitFormat.Unlock();
            }
            
            // Reset digit animation timings when opening/re-opening this page
            if (m_state == States.PAUSED)
            {
                ResetDigitFadeAnim();
            }
        }
        
        public void ShowSettings()
        {
            m_sidebarPages.SwitchToSettingsPage();
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

            if (GetTimerSettings().m_longBreaks)
            {
                // Remove long break once user has started it via Play
                if (IsOnBreak() && IsOnLongBreak())
                {
                    m_tomatoCounter.ConsumeTomatoes();
                    m_digitFormat.DeactivateLongBreak();
                }
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
        /// Completes the current running timer so the user can move on to the next one.
        /// <remarks>Intended to be used as a UnityEvent on the Skip button.</remarks>
        /// </summary>
        public void Skip()
        {
            CompleteTimer();
            m_digitFormat.CorrectTickAnimVisuals();
        }
        
        /// <summary>
        /// Attempts to transition timer into <see cref="States.SETUP"/> and sets our <see cref="DigitFormat"/> to break
        /// mode, will prompt user with confirmation dialog if necessary.
        /// <remarks>Used as a <see cref="UnityEvent"/> on our timer switch.</remarks>
        /// </summary>
        public void TrySwitchToBreakTimer()
        {
            if (!isTimerBeingSetup && m_state != States.COMPLETE)
            {
                m_confirmationDialogManager.SpawnConfirmationDialog(() =>
                {
                    SwitchTimer(true);
                }, () =>
                {
                    // Switch To Break Timer is triggered by Unity Event via break slider, this simply reset's 
                    // our bool back to it's original setting prior to interaction.
                    m_breakSlider.Initialize(this, IsOnBreak()); // Re-init component
                });
            }
            else
            {
                SwitchTimer(true);
            }
        }
        
        public void SwitchTimer(bool isOnBreak)
        {
            m_digitFormat.m_isOnBreak = isOnBreak;
            SwitchState(States.SETUP);
            isTimerBeingSetup = true;
            CalculateTimeValues();
        }

        /// <summary>
        /// Attempts to transition timer into <see cref="States.SETUP"/> and sets our <see cref="DigitFormat"/> to work
        /// mode, will prompt user with confirmation dialog if necessary.
        /// <remarks>Used as a <see cref="UnityEvent"/> on our timer switch.</remarks>
        /// </summary>
        public void TrySwitchToWorkTimer()
        {
            if (!isTimerBeingSetup && m_state != States.COMPLETE)
            {
                m_confirmationDialogManager.SpawnConfirmationDialog(() =>
                {
                    SwitchTimer(false);
                }, (() =>
                {
                    m_breakSlider.Initialize(this, IsOnBreak()); // Re-init component
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
        public void TryRestart(bool isComplete)
        {
            if (!isTimerBeingSetup && m_state != States.COMPLETE)
            {
                m_confirmationDialogManager.SpawnConfirmationDialog((() =>
                {
                    Restart(isComplete);
                }));
            }
            else
            {
                Restart(isComplete);
            }
        }
        
        /// <summary>
        /// Directly toggles the timer mode to it's opposite mode (break/work) and transitions timer into States.SETUP.
        /// Note: If you want to verify this action, <see cref="TryRestart"/>. <see cref="TryRestart"/> will prompt the
        /// user with a confirmation dialog if necessary.
        /// </summary>
        /// <param name="isCompleted"></param>
        public void Restart(bool isCompleted)
        {
            if (isCompleted)
            {
                m_digitFormat.FlipIsOnBreakBool();
            }

            SwitchState(States.SETUP);
            isTimerBeingSetup = true;
            CalculateTimeValues();

            // Stop digit tick animation
            m_digitFormat.ResetTextPositions();
            ShowTickAnimation();
            
            PlaySpawnAnimation();
        }

        #region Button/Keyboard OnClick Events
        /// <summary>
        /// Presses the play/pause button.
        /// </summary>
        public void TriggerPlayPause()
        {
            m_rightButton.OnPointerClick();
        }

        /// <summary>
        /// Presses the boolean slider to toggle between our <see cref="DigitFormat"/> to work/break mode.
        /// </summary>
        public void TriggerTimerSwitch()
        {
            m_breakSlider.OnPointerClick(null);
        }
        
        /// <summary>
        /// Presses the restart button.
        /// </summary>
        public void TriggerTimerRestart()
        {
            m_leftButtonSVGClick.OnPointerClick(null);
        }

        /// <summary>
        /// Presses the boolean slider to toggle between light/dark themes.
        /// </summary>
        public void TriggerThemeSwitch()
        {
            m_themeSlider.Interact();
        }
        #endregion

        /// <summary>
        /// Selects all the digits in our <see cref="DigitFormat"/>.
        /// </summary>
        public void SelectAll()
        {
            // Only allow 'select all' to work when we are in setup state
            if (m_state != States.SETUP)
            {
                return;
            }
            
            ClearSelection();

            foreach (DoubleDigit digit in m_digitFormat.GetDigits())
            {
                AddSelection(digit);
            }

            foreach (DoubleDigit digit in m_selectedDigits)
            {
                digit.Highlight();
            }
            
            // Since we are highlighting (instead of selecting), we bypass the text state logic hence we 
            // invoke it again here.
            CalculateContextVisibility();
        }

        /// <summary>
        /// Adds the provided <see cref="DoubleDigit"/> to our selection list.
        /// </summary>
        /// <param name="digitToAddToSelection">What <see cref="DoubleDigit"/> do you want to add to our selection?
        /// </param>
        private void AddSelection(DoubleDigit digitToAddToSelection)
        {
            if (!m_selectedDigits.Contains(digitToAddToSelection))
            {
                m_selectedDigits.Add(digitToAddToSelection);
            }
        }
        
        /// <summary>
        /// Returns our current timer values in a <see cref="String"/>.
        /// <example>Such as "00:24:35" (without the quotation marks)</example>
        /// </summary>
        /// <returns>Our current timer value.</returns>
        public string GetTimerString()
        {
            return m_digitFormat.GetTimerString();
        }
        
        /// <summary>
        /// Returns our currently selected <see cref="DigitFormat.SupportedFormats"/>'s index value.
        /// </summary>
        /// <returns>A number representing our enum index. See <see cref="DigitFormat.SupportedFormats"/></returns>
        public int GetDigitFormatIndex()
        {
            return m_digitFormat.GetFormatIndex();
        }

        /// <summary>
        /// Returns a list of our current selected element.
        /// <remarks>Will return a list of <see cref="Selectable"/>'s, not specifically <see cref="DoubleDigit"/>'s.
        /// </remarks>
        /// </summary>
        /// <returns></returns>
        public List<Selectable> GetSelections()
        {
            return m_selectedDigits.Select(doubleDigit => doubleDigit.GetSelectable()).ToList();
        }
        
        /// <summary>
        /// Is our <see cref="DigitFormat"/> in break mode?
        /// </summary>
        /// <returns></returns>
        public bool IsOnBreak()
        {
            return m_digitFormat.m_isOnBreak;
        }

        /// <summary>
        /// Is our <see cref="DigitFormat"/> in long break mode?
        /// </summary>
        public bool IsOnLongBreak()
        {
            return m_digitFormat.m_isOnLongBreak;
        }

        /// <summary>
        /// Is our <see cref="AboutPanel"/> currently open and visible?
        /// </summary>
        /// <returns></returns>
        public bool IsAboutPageOpen()
        {
            return m_sidebarPages.IsAboutPageOpen();
        }
        
        /// <summary>
        /// Is our <see cref="Sidebar"/> currently open and visible?
        /// </summary>
        /// <returns></returns>
        public bool IsSidebarOpen()
        {
            return m_sidebarMenu.IsOpen();
        }

        /// <summary>
        /// Is our timer / digit format currently open and visible?
        /// </summary>
        /// <returns></returns>
        public bool IsMainContentOpen()
        {
            return m_sidebarPages.IsTimerPageOpen();
        }
        
        /// <summary>
        /// Sets our <see cref="DigitFormat"/> timer to the provided string. Intended to be used when pasting
        /// values in from our clipboard.
        /// <remarks>This will only work if the timer is in <see cref="States.SETUP"/> mode.</remarks>
        /// </summary>
        /// <param name="timeString"></param>
        public void SetTimerValue(string timeString)
        {
            // Only allow 'Set Timer Value' to work when we are in the setup state
            if (m_state != States.SETUP)
            {
                return;
            }
            
            m_digitFormat.SetTimerValue(timeString);
        }

        /// <summary>
        /// Sets our background's selection navigation to the provided <see cref="Navigation"/>.
        /// <remarks>Intended to change focus to our digits when attempting to select left / right
        /// from the background.</remarks>
        /// </summary>
        /// <param name="backgroundNav"></param>
        public void SetBackgroundNavigation(Navigation backgroundNav)
        {
            m_background.SetSelectionNavigation(backgroundNav);
        }
        
        /// <summary>
        /// Attempts to change our <see cref="DigitFormat"/> to the provided <see cref="DigitFormat.SupportedFormats"/>,
        /// will prompt user with confirmation dialog if necessary.
        /// </summary>
        /// <param name="desiredFormat"></param>
        public void TryChangeFormat(DigitFormat.SupportedFormats desiredFormat)
        {
            if (!isTimerBeingSetup && m_state == States.RUNNING)
            {
                m_digitFormat.SwitchFormat(desiredFormat);
                m_confirmationDialogManager.SpawnConfirmationDialog(GenerateFormat, () =>
                {
                    GetTimerSettings().m_format = desiredFormat;
                    UserSettingsSerializer.SaveTimerSettings(GetTimerSettings());
                    m_sidebarPages.SetSettingDigitFormatDropdown(m_digitFormat.GetPreviousFormatSelection());
                });
            }
            else
            {
                m_digitFormat.SwitchFormat(desiredFormat);
                GenerateFormat();
                GetTimerSettings().m_format = desiredFormat;
                UserSettingsSerializer.SaveTimerSettings(GetTimerSettings());
            }
        }

        /// <summary>
        /// Changes the format directly
        /// </summary>
        private void GenerateFormat()
        {
            m_digitFormat.GenerateFormat();
            Restart(false); 
            
            if (m_sidebarPages.IsSettingsPageOpen())
            {
                m_sidebarPages.UpdateSettingsDigitFormatDropdown();
            }
        }
        
        /// <summary>
        /// Applies our <see cref="Theme"/> changes to our referenced components when necessary.
        /// </summary>
        /// <param name="theme">The theme to apply on our referenced components.</param>
        public void ColorUpdate(Theme theme)
        {
            ColorScheme currentColors = theme.GetCurrentColorScheme();
            
            // State text
            m_text.color = currentColors.m_backgroundHighlight;
            
            // Ring background
            m_ringBackground.material.SetColor(RingColor, theme.GetCurrentColorScheme().m_backgroundHighlight);

            // Left Button Background
            Image leftContainerTarget = m_leftButtonSVGClick.m_containerTarget.GetComponent<Image>();
            if (leftContainerTarget != null)
            {
                leftContainerTarget.material.SetColor(CircleColor, theme.GetCurrentColorScheme().m_backgroundHighlight);
            }
            
            // Left Button Icon
            m_leftButtonSVGClick.m_icon.color = currentColors.m_foreground;

            // Paused Digits
            startingColor = theme.GetCurrentColorScheme().m_foreground;
            endingColor = theme.GetCurrentColorScheme().m_backgroundHighlight;

            // Reset paused digit anim
            ResetDigitFadeAnim();
            
            m_menuToggleSprite.OverrideFalseColor(theme.GetCurrentColorScheme().m_foreground);
            m_menuToggleSprite.ColorUpdate(theme);

            UpdateRingColor(theme);
        }

        private void UpdateRingColor(Theme theme)
        {
            switch (m_state)
            {
                case States.SETUP:
                    // Ring
                    m_ring.material.SetColor(RingColor,
                        !m_digitFormat.m_isOnBreak ? theme.GetCurrentColorScheme().m_modeOne : theme.GetCurrentColorScheme().m_modeTwo);

                    break;
                
                case States.RUNNING:
                    // Ring
                    m_ring.material.SetColor(RingColor, theme.GetCurrentColorScheme().m_running);

                    break;
                
                case States.PAUSED:
                    // Ring
                    m_ring.material.SetColor(RingColor, 
                        !m_digitFormat.m_isOnBreak ? theme.GetCurrentColorScheme().m_modeOne : theme.GetCurrentColorScheme().m_modeTwo);
                    break;
                
                case States.COMPLETE:
                    // Ring
                    m_ring.material.SetColor(RingColor, theme.GetCurrentColorScheme().m_complete);
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        /// <summary>
        /// Triggers a <see cref="IColorHook"/> ColorUpdate() on our <see cref="CreditsGhost"/>.
        /// </summary>
        public void ColorUpdateCreditsBubble()
        {
            m_creditsGhost.ColorUpdate(m_themeManager.GetTheme());
        }
        
        /// <summary>
        /// Sets the users setting preference to enable/disable long breaks.
        /// </summary>
        /// <remarks>Intended to be used as a UnityEvent. Otherwise you can directly do this
        /// on the public property in the settings object.</remarks>
        /// <param name="state">Do you want the user to be able to collect tomatoes/pomodoros and unlock the
        /// long break mode?</param>
        public void SetSettingLongBreaks(bool state = true)
        {
            // Apply and save
            GetTimerSettings().m_longBreaks = state;
            UserSettingsSerializer.SaveTimerSettings(GetTimerSettings());
            
            // Apply component swap
            if (state)
            {
                if (!m_tomatoCounter.IsInitialized())
                {
                    m_tomatoCounter.Initialize(this, loadedTimerSettings.m_pomodoroCount);
                }
                else
                {
                    // Only rebuild tomatoes
                    m_tomatoCounter.SetPomodoroCount(loadedTimerSettings.m_pomodoroCount, m_tomatoCounter.GetTomatoProgress());
                }

                m_tomatoCounter.gameObject.SetActive(true);
            }
            else
            {
                m_tomatoCounter.gameObject.SetActive(false);
                DeactivateLongBreak();
                IfSetupTriggerRebuild();
            }
            m_completionLabel.MoveAnchors(state);
        }

        /// <summary>
        /// Triggers a rebuild of our timer if we are in SETUP mode.
        /// <remarks>Intended when in long break mode and you wipe tomato progress.</remarks>
        /// </summary>
        public void IfSetupTriggerRebuild()
        {
            if (m_state == States.SETUP)
            {
                SwitchTimer(m_digitFormat.m_isOnBreak);
            }
        }
        
        /// <summary>
        /// Sets the users setting preference to enable/disable the EndTimestampBubble (located at the bottom right).
        /// </summary>
        /// <param name="state"></param>
        private void SetSettingEndTimestampBubble(bool state)
        {
            // TODO: Feature setting
        }

        /// <summary>
        /// Enables long break on our <see cref="DigitFormat"/>.
        /// </summary>
        public void ActivateLongBreak()
        {
            m_digitFormat.ActivateLongBreak();
        }

        /// <summary>
        /// Disables long break on our <see cref="DigitFormat"/>. (Note: <see cref="DigitFormat"/> could still be
        /// in a work / break mode)
        /// </summary>
        public void DeactivateLongBreak()
        {
            m_digitFormat.DeactivateLongBreak();
        }

        /// <summary>
        /// Fades in/out our credits bubble.
        /// </summary>
        /// <param name="fadeIn">Do you want the credits bubble to fade in? (Providing `False` will make
        /// the credit's bubble fade out.)</param>
        public void FadeCreditsBubble(bool fadeIn)
        {
            if (fadeIn)
            {
                m_creditsGhost.FadeIn();
                m_creditsGhost.Lock();
            }
            else
            {
                m_creditsGhost.FadeOut();
                m_creditsGhost.Unlock();
            }
        }

        /// <summary>
        /// Positions our <see cref="CreditsGhost"/> to stay within the bounds of the sidebar.
        /// </summary>
        /// <param name="desiredWidthPercentage"></param>
        /// <param name="rightOffsetInPixels"></param>
        public void ConformCreditsBubbleToSidebar(float desiredWidthPercentage, float rightOffsetInPixels = -10)
        {
            m_creditsGhost.SetWidth(desiredWidthPercentage);
            m_creditsGhost.SetRightOffset(rightOffsetInPixels);
        }

        /// <summary>
        /// Positions our <see cref="CreditsGhost"/> back to it's original position. (Not conforming to the sidebar)
        /// </summary>
        public void ResetCreditsBubbleSidebarConformity()
        {
            m_creditsGhost.ResetWidth();
            m_creditsGhost.ResetRightOffset();
        }
        
        /// <summary>
        /// Does this timer currently have any pomodoro/tomato progression?
        /// </summary>
        /// <returns></returns>
        public bool HasTomatoProgression()
        {
            return m_tomatoCounter.HasProgression() || m_digitFormat.m_isOnLongBreak;
        }

        /// <summary>
        /// Changes the current timer to the provided value.
        /// <remarks>Intended to be used by our Media Creator.</remarks>
        /// </summary>
        /// <param name="currentTimeInSeconds"></param>
        public void SetCurrentTime(float currentTimeInSeconds)
        {
            currentTime = currentTimeInSeconds;
            m_digitFormat.CorrectTickAnimVisuals();
            m_digitFormat.ShowTime(TimeSpan.FromSeconds(currentTime));
        }
        
        public void SetPomodoroCount(int desiredPomodoroCount, int pomodoroProgress)
        {
            m_tomatoCounter.SetPomodoroCount(desiredPomodoroCount, pomodoroProgress);
            
            // Check if user achieved long break with new settings...
            if (pomodoroProgress == desiredPomodoroCount)
            {
                // Trigger long break and rebuild.
                ActivateLongBreak();
                IfSetupTriggerRebuild();
            }
            // It's also possible our user is already in the long break state screen, and their new 
            // numbers might not be valid. Check for this too...
            else if (IsOnLongBreak() && m_state == States.SETUP)
            {
                // De-activate their long break since their pomodoro count changed / is no longer valid.
                DeactivateLongBreak();
                IfSetupTriggerRebuild();
            }            
        }

        public int GetTomatoProgress()
        {
            return m_tomatoCounter.GetTomatoProgress();
        }

        public int GetTomatoCount()
        {
            return m_tomatoCounter.GetTomatoCount();
        }

        // TODO: Remove piper methods - We want explicit methods instead of reaching in. Separation of concerns.
        [Obsolete]
        public ConfirmationDialogManager GetConfirmDialogManager()
        {
            return m_confirmationDialogManager;
        }

        public TranslucentImageSource GetTranslucentImageSource()
        {
            return m_translucentImageSource;
        }
        
        // TODO: Remove PomodoroTimer dependency
        public Theme GetTheme()
        {
            return m_themeManager.GetTheme();
        }

        public void ShowOverlay()
        {
            ClearSelection();
            m_overlay.Show();
        }

        public void HideOverlay()
        {
            m_overlay.Hide();
        }
        
        public void CloseSidebar()
        {
            m_sidebarMenu.Close();
        }

#if UNITY_EDITOR
        // TODO: Find a more consistent way of doing all these. Such as a ShowInstantly() method or something.
        // Creator Media Methods: Only intended for instant visual changes
        public void MCHideCreditsBubble()
        {
            m_creditsGhost.FadeOut(true);
        }

        public void MCShowCreditsBubble()
        {
            m_creditsGhost.FadeIn(true);
        }

        public void MCShowEndTimestampBubble()
        {
            m_endTimestampGhost.FadeIn(true);
        }

        public void MCDisableCompletionAnimation()
        {
            m_completionLabel.HideCompletionAnimation();
            disableCompletionAnimation = true;
        }

        public void MCEnableBreakSlider()
        {
            m_breakSlider.SetVisualToEnable();
        }
        
        public void MCShowSidebar()
        {
            m_sidebarMenu.gameObject.SetActive(true);
            ConformCreditsBubbleToSidebar(m_sidebarMenu.CalculateSidebarWidth());
            ShowOverlay();
        }

        public void MCDisableBreakSlider()
        {
            m_breakSlider.SetVisualToDisable();
        }

        public void MCHideSidebar()
        {
            m_sidebarMenu.gameObject.SetActive(false); 
            HideOverlay();
        }

        public void MCEnableDarkModeToggle()
        {
            m_themeSlider.SetVisualToEnable();
        }

        public void MCDisableDarkModeToggle()
        {
            m_themeSlider.SetVisualToDisable();
        }

        public void MCEnableThemeToggleAnimation()
        {
            m_themeSlider.EnableAnimation();
        }
#endif


        // Settings
        public SystemSettings GetSystemSettings()
        {
            return loadedSystemSettings;
        }

        public TimerSettings GetTimerSettings()
        {
            return loadedTimerSettings;
        }
        
        public void ShowTickAnimation()
        {
            m_digitFormat.ShowTickAnimation();
        }
    }
}
