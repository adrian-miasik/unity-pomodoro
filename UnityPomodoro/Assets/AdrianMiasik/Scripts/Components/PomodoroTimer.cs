using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using AdrianMiasik.Components.Core;
using AdrianMiasik.Components.Core.Containers;
using AdrianMiasik.Components.Core.Items;
using AdrianMiasik.Components.Specific;
using AdrianMiasik.Components.Specific.Settings;
using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
using AdrianMiasik.UWP;
using LeTai.Asset.TranslucentImage;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace AdrianMiasik.Components
{
    // TODO: Provide users with the ability to disable the long timers in the settings panel.
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

        [Header("Settings (for this specific timer)")] 
        [SerializeField] private Settings m_settings;

        [Header("Basic - Components")]
        [SerializeField] private TextMeshProUGUI m_text; // Text used to display current state
        [SerializeField] private Image m_ring; // Ring used to display timer progress
        [SerializeField] private Image m_ringBackground; // Theming
        
        [Header("Unity Pomodoro - Components")]
        [SerializeField] private Background m_background; // Used to pull select focus
        [SerializeField] private CompletionLabel m_completionLabel; // Used to prompt the user the timer is finished
        [SerializeField] private DigitFormat m_digitFormat; // Responsible class for manipulating our digits and formats
        [FormerlySerializedAs("m_menuToggle")] [SerializeField] private ToggleSprite m_menuToggleSprite; // Used to toggle our sidebar menu
        [SerializeField] private ClickButtonSVGIcon m_leftButtonSVGClick; // Used to restart the timer
        // TODO: Consolidate right buttons to a single class
        [SerializeField] private ClickButtonSVGIcon m_rightButtonSVGClick; // Used to play/pause the timer
        [SerializeField] private RightButton m_rightButton; // Additional timer state element 
        [SerializeField] private ToggleSlider m_breakSlider; // Used for switching timer between mode one and mode two
        [SerializeField] private CreditsBubble m_creditsBubble; // Used to display project contributors
        [SerializeField] private ThemeSlider m_themeSlider; // Used to change between light / dark mode
        [SerializeField] private ToggleSprite m_halloweenToggleSprite; // Halloween theme toggle during Halloween week (Disabled by default) // TODO: Re-implement
        [SerializeField] private Sidebar m_sidebarMenu; // Used to change and switch between our pages / panel contents (Such as main, settings, and about)
        [SerializeField] private TomatoCounter m_tomatoCounter; // Responsible class for counting work / break timers and providing a long break
        [SerializeField] private EndTimestampBubble m_endTimestampBubble; // Responsible for displaying the local end time for the current running timer.
        [SerializeField] private SkipButton m_skipButton;
        private readonly List<ITimerState> timerElements = new List<ITimerState>();
        
        [Header("Unity Pomodoro - Managers")]
        [SerializeField] private HotkeyDetector m_hotkeyDetector; // Responsible class for our keyboard shortcuts / bindings
        [SerializeField] private ConfirmationDialogManager m_confirmationDialogManager;
        [SerializeField] private NotificationManager m_notifications; // Responsible class for UWP notifications and toasts

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
        /// A <see cref="UnityEvent"/> that gets invoked when the spawn animation is complete.
        /// </summary>
        [Header("Unity Events")]
        public UnityEvent m_onSpawnCompletion;
        
        /// <summary>
        /// A <see cref="UnityEvent"/> that gets invoked when the ring / timer alarm pulses.
        /// </summary>
        public UnityEvent m_onRingPulse;
        
        /// <summary>
        /// A <see cref="UnityEvent"/> that gets invoked when the timer finishes. (<see cref="States.COMPLETE"/>)
        /// </summary>
        public UnityEvent m_onTimerCompletion;

        [Header("Cache")]
        [SerializeField] private List<DoubleDigit> m_selectedDigits = new List<DoubleDigit>(); // Contains our currently selected digits
        
        [Header("Pages / Panels - Deprecated")]
        // TODO: Create a page / panel solution
        [SerializeField] private GameObject m_mainContainer; // Our main body page
        [SerializeField] private SettingsPanel m_settingsContainer; // Our settings page
        [SerializeField] private AboutPanel m_aboutContainer; // Our about page

        [Header("External Extra - Deprecated")]
        // TODO: Move to theme class
        [SerializeField] private Theme m_theme; // Current active theme

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

        // Pulse Ring Complete Animation
        private bool disableCompletionAnimation;
        private float accumulatedRingPulseTime;
        private bool hasRingPulseBeenInvoked;
        
        // Pulse Tick Ring Animation
        private readonly bool isRingTickAnimationEnabled = false; // TODO: Re-implement this animation? / Expose in settings?
        private float cachedSeconds;
        private bool isRingTickAnimating;

        private void OnApplicationFocus(bool hasFocus)
        {
            if (m_settings.m_muteSoundWhenOutOfFocus)
            {
                // Prevent application from making noise when not in focus
                AudioListener.volume = !hasFocus ? 0 : 1;
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
        
        private void ConfigureSettings()
        {
            // Set theme to light
            GetTheme().m_darkMode = false;
            
            // Restart settings (Restore back to dev defaults dependent on OS)
            m_settings.ApplyPlatformDefaults();
        }

        /// <summary>
        /// Setup view, calculate time, initialize components, transition in, and animate.
        /// </summary>
        private void Initialize()
        {
            // Setup pages /s panels
            m_settingsContainer.Hide();
            m_aboutContainer.Hide();
            m_mainContainer.gameObject.SetActive(true);

            // Overrides
            m_themeSlider.OverrideFalseColor(m_theme.m_light.m_backgroundHighlight);
            m_themeSlider.OverrideTrueColor(new Color(0.59f, 0.33f, 1f));
            m_menuToggleSprite.OverrideFalseColor(m_theme.GetCurrentColorScheme().m_foreground);
            m_menuToggleSprite.OverrideTrueColor(Color.clear);

            // TODO: Re-implement halloween theme?
            // Halloween Theme Toggle
            // Check if it's October...
            if (DateTime.Now.Month == 10)
            {
                // Check if it's Halloween week...
                for (int i = 25; i <= 31; i++)
                {
                    // Is today Halloween week...
                    if (DateTime.Now.Day == i)
                    {
                        m_halloweenToggleSprite.gameObject.SetActive(true);
                        m_halloweenToggleSprite.OverrideTrueColor(new Color(1f, 0.59f, 0f));
                        m_halloweenToggleSprite.Initialize(this, false);
                        break;
                    }
                }
            }

            InitializeManagers();
            InitializeComponents();

            // Calculate time
            CalculateTimeValues();

            // Transition to setup state
            SwitchState(States.SETUP);

            // Animate in
            PlaySpawnAnimation();

            // Setup & apply theme
            m_theme.Register(this);
            m_theme.ApplyColorChanges();
        }

        private void InitializeManagers()
        {
            m_confirmationDialogManager.Initialize(this);
            m_notifications.Initialize(m_settings);
        }

        private void InitializeComponents()
        {
            // Hook up right button
            m_rightButton.m_playOnClick.AddListener(Play);
            m_rightButton.m_pauseOnClick.AddListener(Pause);
            m_rightButton.m_snoozeOnClick.AddListener(() =>
            {
                Restart(true);
                m_breakSlider.Press();
            });
            
            // Hook up left button
            m_leftButtonSVGClick.m_onClick.AddListener(() =>
            {
                TryRestart(false);
            });

            m_hotkeyDetector.Initialize(this);
            m_background.Initialize(this);
            m_digitFormat.Initialize(this);
            m_completionLabel.Initialize(this);
            m_themeSlider.Initialize(this);
            m_creditsBubble.Initialize(this);
            m_rightButton.Initialize(this);
            m_menuToggleSprite.Initialize(this, false);
            m_breakSlider.Initialize(this, false);
            m_sidebarMenu.Initialize(this);
            m_endTimestampBubble.Initialize(this);
            m_settingsContainer.Initialize(this, m_settings);
            m_skipButton.Initialize(this);
            
            if (m_settings.m_longBreaks)
            {
                m_tomatoCounter.Initialize(this);
                m_completionLabel.MoveAnchors(true);
            }
            else
            {
                m_tomatoCounter.gameObject.SetActive(false);
                m_completionLabel.MoveAnchors(false);
            }
            
            // Register elements that need updating per timer state change
            timerElements.Add(m_rightButton);
            timerElements.Add(m_completionLabel);
            timerElements.Add(m_endTimestampBubble);
            timerElements.Add(m_skipButton);
        }
        
        /// <summary>
        /// Unity's OnDestroy() - Deregister self from <see cref="Theme"/> on destruction.
        /// </summary>
        public void OnDestroy()
        {
            // Make sure to deregister this when and if we do destroy the timer
            GetTheme().Deregister(this);
        }
        
        /// <summary>
        /// Switches the timer to the provided state and handles all visual changes.
        /// Basically handles our transitions between timer states. <see cref="PomodoroTimer.States"/>
        /// </summary>
        /// <param name="desiredState">The state you want to transition to</param>
        public void SwitchState(States desiredState)
        {
            m_state = desiredState;

            // Update the registered timer elements
            foreach (ITimerState element in timerElements)
            {
                element.StateUpdate(m_state, m_theme);
            }
            
            UpdateRingColor(m_theme);

            // Do transition logic
            switch (m_state)
            {
                case States.SETUP:
                    m_digitFormat.SetDigitColor(m_theme.GetCurrentColorScheme().m_foreground);
                    
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

                    m_digitFormat.SetDigitColor(m_theme.GetCurrentColorScheme().m_foreground);
                    
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
                    if (m_settings.m_longBreaks)
                    {
                        m_tomatoCounter.gameObject.SetActive(true);
                    }
                }
            }
            else
            {
                m_text.gameObject.SetActive(false);
                if (m_settings.m_longBreaks)
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
     
                AnimateRingTickPulse();
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
                if (m_settings.m_longBreaks)
                {
                    m_tomatoCounter.FillTomato();
                }
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
            
            if (Math.Abs(cachedSeconds - TimeSpan.FromSeconds(currentTime).Seconds) > Mathf.Epsilon)
            {
                isRingTickAnimating = true;
            }

            if (isRingTickAnimating)
            {
                accumulatedRingPulseTime += Time.deltaTime;
                m_ring.material.SetFloat(RingDiameter, m_ringTickWidth.Evaluate(accumulatedRingPulseTime));
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
        
        // TODO: Create a panel/page class
        /// <summary>
        /// Shows about content, hides main content, and shows credits bubble.
        /// </summary>
        public void ShowAbout()
        {
            if (!m_aboutContainer.IsInitialized())
            {
                m_aboutContainer.Initialize(this);
            }

            // Prevent tick animations from pausing when switching to info page
            // TODO: Do this for settings page too?
            m_digitFormat.CorrectTickAnimVisuals();
            
            // Hide other content
            m_mainContainer.gameObject.SetActive(false);
            m_settingsContainer.Hide();
            
            // Show about page content
            m_aboutContainer.Show();

            // Special behaviour that's used to display/open up credits bubble when on this page
            if (!m_creditsBubble.IsRunning())
            {
                m_creditsBubble.Lock();
                m_creditsBubble.FadeIn();
            }
        }

        /// <summary>
        /// Shows main content, hides info, and hides credits bubble
        /// </summary>
        public void ShowMainContent()
        {
            // Hide other content
            m_aboutContainer.Hide();
            m_settingsContainer.Hide();
            
            // Show main content
            m_mainContainer.gameObject.SetActive(true);
            m_digitFormat.GenerateFormat();
            m_digitFormat.ShowTime(TimeSpan.FromSeconds(currentTime)); // Update visuals to current time
            
            if (m_state != States.SETUP)
            {
                m_digitFormat.Lock();
            }
            
            // Reset digit animation timings when opening/re-opening this page
            if (m_state == States.PAUSED)
            {
                ResetDigitFadeAnim();
            }
        }

        /// <summary>
        /// Unlocks control and fades out the credits bubble.
        /// </summary>
        public void CloseOutCreditsBubble()
        {
            // Hide / close out credits bubble
            m_creditsBubble.Unlock();
            m_creditsBubble.FadeOut();
        }

        public void ShowSettings()
        {
            // Hide other content
            m_aboutContainer.Hide();
            m_mainContainer.gameObject.SetActive(false);
            
            // Show settings content
            m_settingsContainer.Show();
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

            if (m_settings.m_longBreaks)
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
            
            PlaySpawnAnimation();
        }

        #region Button/Keyboard OnClick Events
        /// <summary>
        /// Presses the play/pause button.
        /// </summary>
        public void TriggerPlayPause()
        {
            m_rightButtonSVGClick.OnPointerClick(null);
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
        public int GetDigitFormat()
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
            return m_aboutContainer.IsInfoPageOpen();
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
            return m_mainContainer.gameObject.activeSelf;
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

        // TODO: move to DigitFormatDropdown.cs
        /// <summary>
        /// Attempts to change the digit format using enum index, will prompt user with confirmation dialog
        /// if necessary. See <see cref="DigitFormat.SupportedFormats"/>.
        /// </summary>
        /// <param name="i"></param>
        public void TryChangeFormat(Int32 i)
        {
            TryChangeFormat((DigitFormat.SupportedFormats)i);
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
                    m_settingsContainer.SetDropdown(m_digitFormat.GetPreviousFormatSelection());
                });
            }
            else
            {
                m_digitFormat.SwitchFormat(desiredFormat);
                GenerateFormat();
            }
        }

        /// <summary>
        /// Changes the format directly
        /// </summary>
        private void GenerateFormat()
        {
            m_digitFormat.GenerateFormat();
            Restart(false); 
            
            if (m_settingsContainer.IsPageOpen())
            {
                m_settingsContainer.UpdateDropdown();
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

            // Right Button Background
            Image rightContainerTarget = m_rightButtonSVGClick.m_containerTarget.GetComponent<Image>();
            if (rightContainerTarget != null)
            {
                rightContainerTarget.material.SetColor(CircleColor, currentColors.m_backgroundHighlight);
            }
            
            // Paused Digits
            startingColor = m_theme.GetCurrentColorScheme().m_foreground;
            endingColor = m_theme.GetCurrentColorScheme().m_backgroundHighlight;

            // Reset paused digit anim
            ResetDigitFadeAnim();
            
            m_menuToggleSprite.OverrideFalseColor(m_theme.GetCurrentColorScheme().m_foreground);
            m_menuToggleSprite.ColorUpdate(m_theme);

            UpdateRingColor(m_theme);
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

        // TODO: Create theme manager class?
        /// <summary>
        /// Returns our current active <see cref="Theme"/>
        /// </summary>
        /// <returns></returns>
        public Theme GetTheme()
        {
            return m_theme;
        }
        
        /// <summary>
        /// Sets our <see cref="Theme"/> preference to light mode, and update's all our necessary components.
        /// <remarks>Used as a UnityEvent on our <see cref="m_themeSlider"/>.</remarks>
        /// </summary>
        public void SetToLightMode()
        {
            m_theme.SetToLightMode();
        }

        /// <summary>
        /// Sets our <see cref="Theme"/> preference to dark mode, and update's all our necessary components.
        /// <remarks>Used as a UnityEvent on our <see cref="m_themeSlider"/>.</remarks>
        /// </summary>
        public void SetToDarkMode()
        {
            m_theme.SetToDarkMode();
        }
        
        /// <summary>
        /// Sets our current active <see cref="Theme"/> to the provided <see cref="Theme"/>. This will transfer
        /// all our <see cref="IColorHook"/> element's to the new <see cref="Theme"/> as well.
        /// </summary>
        /// <param name="desiredTheme"></param>
        public void SwitchTheme(Theme desiredTheme)
        {
            // Transfer elements to new theme (So theme knows which elements to color update)
            m_theme.TransferColorElements(m_theme, desiredTheme);
            
            // Swap our theme
            m_theme = desiredTheme;
            
            // Apply our changes
            m_theme.ApplyColorChanges();
        }
        
        /// <summary>
        /// Triggers a <see cref="IColorHook"/> ColorUpdate() on our <see cref="CreditsBubble"/>.
        /// </summary>
        public void ColorUpdateCreditsBubble()
        {
            m_creditsBubble.ColorUpdate(m_theme);
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
            // Change setting
            m_settings.m_longBreaks = state;
            
            // Apply component swap
            if (state)
            {
                m_tomatoCounter.Initialize(this);
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

        // TODO: System wide setting
        /// <summary>
        /// Sets the users setting preference to enable/disable the EndTimestampBubble (located at the bottom right).
        /// </summary>
        /// <param name="state"></param>
        private void SetSettingEndTimestampBubble(bool state)
        {
            // TODO: Feature setting
        }

        /// <summary>
        /// Sets our <see cref="DigitFormat"/> to long break mode.
        /// </summary>
        public void ActivateLongBreak()
        {
            m_digitFormat.ActivateLongBreak();
        }

        /// <summary>
        /// Sets our <see cref="DigitFormat"/> to not use long break mode. (<see cref="DigitFormat"/> could still be
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
                m_creditsBubble.FadeIn();
                m_creditsBubble.Lock();
            }
            else
            {
                if (!IsAboutPageOpen())
                {
                    m_creditsBubble.FadeOut();
                    m_creditsBubble.Unlock();
                }
            }
        }

        public void ConformCreditsBubbleToSidebar(float desiredWidthPercentage, float rightOffsetInPixels = -10)
        {
            m_creditsBubble.SetWidth(desiredWidthPercentage);
            m_creditsBubble.SetRightOffset(rightOffsetInPixels);
        }

        public void ResetCreditsBubbleSidebarConformity()
        {
            m_creditsBubble.ResetWidth();
            m_creditsBubble.ResetRightOffset();
        }
        
        public bool HasTomatoProgression()
        {
            return m_tomatoCounter.HasProgression() || m_digitFormat.m_isOnLongBreak;
        }

        public void SetCurrentTime(float currentTimeInSeconds)
        {
            currentTime = currentTimeInSeconds;
            m_digitFormat.CorrectTickAnimVisuals();
            m_digitFormat.ShowTime(TimeSpan.FromSeconds(currentTime));
        }

        public void HideCreditsBubble()
        {
            m_creditsBubble.FadeOut(true);
        }

        public void ShowCreditsBubble()
        {
            m_creditsBubble.FadeIn(true);
        }

        public void ShowEndTimestampBubble()
        {
            m_endTimestampBubble.FadeIn(true);
        }

        public void DisableCompletionAnimation()
        {
            m_completionLabel.HideCompletionAnimation();
            disableCompletionAnimation = true;
        }

        public void EnableBreakSlider()
        {
            m_breakSlider.SetVisualToEnable();
        }
        
        public void ShowSidebar()
        {
            m_sidebarMenu.gameObject.SetActive(true);
            ConformCreditsBubbleToSidebar(m_sidebarMenu.CalculateSidebarWidth());
            m_sidebarMenu.ShowOverlay();
        }

        public void DisableBreakSlider()
        {
            m_breakSlider.SetVisualToDisable();
        }

        public void HideSidebar()
        {
            m_sidebarMenu.gameObject.SetActive(false);
            m_sidebarMenu.HideOverlay();
        }

        public void EnableDarkModeToggle()
        {
            m_themeSlider.SetVisualToEnable();
        }

        public void DisableDarkModeToggle()
        {
            m_themeSlider.SetVisualToDisable();
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

        // TODO: Remove piper method
        [Obsolete]
        public ConfirmationDialogManager GetConfirmDialogManager()
        {
            return m_confirmationDialogManager;
        }
    }
}