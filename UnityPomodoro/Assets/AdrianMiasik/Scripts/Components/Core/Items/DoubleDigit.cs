using AdrianMiasik.Components.Base;
using AdrianMiasik.Components.Core.Containers;
using AdrianMiasik.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AdrianMiasik.Components.Core.Items
{
    /// <summary>
    /// A <see cref="ThemeElement"/> used as an interactable input field (lockable) that's designed to hold
    /// two numerical values. Can be interacted by the user via the increment / decrement arrows.
    /// <remarks>Has two layers of selections. (Itself -> Input Field)</remarks>
    /// (Also see <see cref="DigitFormat"/>)
    /// </summary>
    public class DoubleDigit : ThemeElement, ISelectHandler, IPointerClickHandler, ISubmitHandler
    {
        [Header("References")] 
        [SerializeField] private Selectable m_selectable;
        [SerializeField] private Image m_background;

        [SerializeField] private ClickButtonSVGIcon m_upArrow;
        [SerializeField] private TMP_InputField m_input;
        [SerializeField] private ClickButtonSVGIcon m_downArrow;
        
        [Header("Animations")] 
        [SerializeField] private float m_animationDuration = 1f;
        [SerializeField] private AnimationCurve m_animationRamp = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private Animation m_pulseWobble;
        [SerializeField] private Animation m_tick;
        
        /// <summary>
        /// The <see cref="DigitFormat.Digits"/> these two numerical are suppose to represent.
        /// </summary>
        [HideInInspector] public DigitFormat.Digits m_digit;
        
        // Cache
        private DigitFormat format;
        private bool isInteractable = true;
        private bool isSelected;

        // Color animation
        private Color startingColor;
        private Color endingColor;
        private bool isColorAnimating;
        private float accumulatedColorTime;
        private float progress;

        // Deselection
        private float accumulatedSelectionTime;
        private float deselectionDuration = 3f; // How long to wait before deselecting automatically on touch devices

        // Controls
        private TMP_SelectionCaret caret;
        private bool ignoreFirstClick = true;

        // Shaders
        private Material instanceMaterial;
        private static readonly int SquircleColor = Shader.PropertyToID("Color_297012532bf444df807f8743bdb7e4fd");
        
        /// <summary>
        /// Invoked when this <see cref="DoubleDigit"/> is selected.
        /// </summary>
        public UnityEvent m_onSelection;
        
        /// <summary>
        /// Invoked when this value changes to something different.
        /// <remarks>Invoked only when the timer is running.</remarks>
        /// </summary>
        public UnityEvent m_onDigitChange;

        /// <summary>
        /// Disables TextMeshPro's run-time caret, and hides the associated increment / decrement arrows.
        /// </summary>
        /// <param name="pomodoroTimer"></param>
        /// <param name="digitFormat"></param>
        /// <param name="digit"></param>
        public void Initialize(PomodoroTimer pomodoroTimer, DigitFormat digitFormat, DigitFormat.Digits digit)
        {
            base.Initialize(pomodoroTimer);
            format = digitFormat;
            m_digit = digit;

            // Disable run time caret interactions - We want to run input through this classes input events
            caret = m_input.textViewport.GetChild(0).GetComponent<TMP_SelectionCaret>();
            if (caret)
            {
                // Prevent input field from getting selection focus
                caret.raycastTarget = false;
            }
            
            HideArrows();
        }

        /// <summary>
        /// Sets the inputs fields current value to the provided one, checks if arrows need to be visible/hidden, and
        /// stops our pulse wobble animation if one is playing.
        /// </summary>
        /// <param name="value">What value should this <see cref="DoubleDigit"/> display?</param>
        public void UpdateVisuals(int value)
        {
            SetTextLabel(value);
            UpdateArrows();
            m_pulseWobble.Stop();
        }
        
        private void Update()
        {
            if (isColorAnimating)
            {
                accumulatedColorTime += Time.deltaTime;
                progress = accumulatedColorTime / m_animationDuration;
                if (progress >= 1)
                {
                    isColorAnimating = false;
                }

                float evaluatedTime = m_animationRamp.Evaluate(progress);

                // Animate and modify
                instanceMaterial.SetColor(SquircleColor, Color.Lerp(startingColor, endingColor, evaluatedTime));

                // Apply
                m_background.material = instanceMaterial;
            }

            if (isSelected)
            {
                if (Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace))
                {
                    if (!m_input.isFocused)
                    {
                        SetValue(0);
                        UpdateVisuals(format.GetDigitValue(m_digit));
                    }
                }
                
                // Scroll input
                if (Input.mouseScrollDelta.y > 0)
                {
                    if (format.CanIncrementOne(m_digit))
                    {
                        m_upArrow.OnPointerClick(null);
                    }
                }
                else if (Input.mouseScrollDelta.y < 0)
                {
                    if (format.CanDecrementOne(m_digit))
                    {
                        m_downArrow.OnPointerClick(null);
                    }
                }

                // Arrow keys : Up arrow
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    m_upArrow.OnPointerClick(null);
                    m_upArrow.Hold();
                }
                else if (Input.GetKeyUp(KeyCode.UpArrow))
                {
                    m_upArrow.Release();
                }

                // Arrow keys : Down arrow
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    m_downArrow.OnPointerClick(null);
                    m_downArrow.Hold();
                }
                else if (Input.GetKeyUp(KeyCode.DownArrow))
                {
                    m_downArrow.Release();
                }

                // Automatic digit deselection on mobile
                if (Input.touchSupported && !m_input.isFocused)
                {
                    accumulatedSelectionTime += Time.deltaTime;

                    if (accumulatedSelectionTime > deselectionDuration)
                    {
                        accumulatedSelectionTime = 0;
                        DeselectInput();
                        format.ClearTimerSelection();
                    }
                }
            }
        }

        private void SetSquircleColor(Color color)
        {
            // Create instance material
            if (instanceMaterial == null)
            {
                instanceMaterial = new Material(m_background.material);
            }

            startingColor = Timer.GetTheme().GetCurrentColorScheme().m_backgroundHighlight;
            // startingColor = instanceMaterial.GetColor(SquircleColor);

            endingColor = color;
            accumulatedColorTime = 0;
            isColorAnimating = true;
        }

        /// <summary>
        /// Plays our tick animation.
        /// <remarks>Used as a <see cref="UnityEvent"/> onValueChanged.</remarks>
        /// </summary>
        public void PlayTickAnimation()
        {
            // Prevent milliseconds from animating on tick (due to how fast it ticks)
            if (m_digit == DigitFormat.Digits.MILLISECONDS)
            {
                return;
            }
            
            m_tick.enabled = true;
            m_tick.Stop();
            m_tick.Play();
        }

        public void HideTickAnimation()
        {
            m_tick.enabled = false;
        }

        /// <summary>
        /// Resets our input field's default viewport positions & anchors.
        /// </summary>
        public void ResetTextPosition()
        {
            m_input.textComponent.rectTransform.anchorMin = Vector2.zero;
            m_input.textComponent.rectTransform.anchorMax = Vector2.one;
        }
        
        /// <summary>
        /// Returns the input fields text.
        /// </summary>
        /// <returns></returns>
        public string GetDigitsLabel()
        {
            return m_input.text;
        }
        
        /// <summary>
        /// Makes the increment & decrement arrow hidden.
        /// </summary>
        public void HideArrows()
        {
            m_upArrow.Hide();
            m_downArrow.Hide();
        }
        
        private void ShowArrows()
        {
            if (format != null)
            {
                if (format.CanIncrementOne(m_digit))
                {
                    m_upArrow.Show();
                }

                if (format.CanDecrementOne(m_digit))
                {
                    m_downArrow.Show();        
                }
            }
        }

        /// <summary>
        /// Prevents the input field from being interacted with.
        /// </summary>
        public void Lock()
        {
            isInteractable = false;
            m_input.interactable = false;
        }

        /// <summary>
        /// Allows our input field to be interacted with.
        /// </summary>
        public void Unlock()
        {
            isInteractable = true;
            m_input.interactable = true;
        }
        
        /// <summary>
        /// Sets the input field text to the provided color.
        /// </summary>
        /// <param name="newColor">The color you want the input fields text to be.</param>
        public void SetTextColor(Color newColor)
        {
            m_input.textComponent.color = newColor;
        }
        
        /// <summary>
        /// Sets the digit data to the provided input.
        /// This is different from the current visuals of this digit.
        /// If you are looking to modify the digit visual value see <see cref="SetTextLabel"/>.
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(int value)
        {
            if (format == null)
                return;
            
            format.SetDigit(m_digit, value);
        }
        
        /// <summary>
        /// Sets the digit label to the provided input.
        /// This is different from setting the digit time.
        /// If you are looking to modify the users set/edited time see <see cref="SetValue"/>.
        /// Also if you are looking to update arrows as well as the label see <seealso cref="UpdateVisuals"/>.
        /// </summary>
        /// <param name="value"></param>
        public void SetTextLabel(int value)
        {
            // If this digit value is actually different...
            if (value.ToString("D2") != m_input.text)
            {
                // Update the digit
                m_input.text = value.ToString("D2");

                if (format.GetTimerState() == PomodoroTimer.States.RUNNING)
                {
                    m_onDigitChange?.Invoke();
                }
            }
        }
        
        /// <summary>
        /// Add one to this digit. (+1)
        /// <remarks>Used as a UnityEvent on our (top) increment arrow.</remarks>
        /// </summary>
        public void IncrementOne()
        {
            if (format == null) 
                return;
            
            format.IncrementOne(m_digit);
            UpdateVisuals(format.GetDigitValue(m_digit));
            m_pulseWobble.Play();
            accumulatedSelectionTime = 0;
        }

        /// <summary>
        /// Removes one from this digit. (-1)
        /// </summary>
        public void DecrementOne()
        {
            if (format == null) 
                return;
            
            format.DecrementOne(m_digit);
            UpdateVisuals(format.GetDigitValue(m_digit));
            m_pulseWobble.Play();
            accumulatedSelectionTime = 0;
        }

        private void UpdateArrows()
        {
            // Up Arrow
            if (format.CanIncrementOne(m_digit))
            {
                m_upArrow.Show();
            }
            else
            {
                m_upArrow.Hide();
            }
            
            // Down Arrow
            if (format.CanDecrementOne(m_digit))
            {
                m_downArrow.Show();
            }
            else
            {
                m_downArrow.Hide();
            }
        }

        /// <summary>
        /// Highlights this <see cref="DoubleDigit"/> to display it's arrows and set's it's background color,
        /// but does not set this as the <see cref="PomodoroTimer"/>'s digit selection. If you'd like to do both
        /// see <see cref="OnSelect"/>
        /// </summary>
        public void Highlight()
        {
            OnSelect(false);
        }
        
        /// <summary>
        /// Highlights this <see cref="DoubleDigit"/> to display it's arrows and set's it's background color,
        /// and set this <see cref="DoubleDigit"/> as the <see cref="PomodoroTimer"/>'s current selection.
        /// </summary>
        /// <param name="eventData"></param>
        public void OnSelect(BaseEventData eventData)
        {
            OnSelect(true);
        }

        private void OnSelect(bool setSelection)
        {
            if (!isInteractable)
            {
                return;
            }

            isSelected = true;
            accumulatedSelectionTime = 0;
            
            ShowArrows();
            SetSquircleColor(Timer.GetTheme().GetCurrentColorScheme().m_backgroundHighlight);

            if (setSelection)
            {
                format.SetTimerSelection(this);
            }

            m_onSelection.Invoke();
        }

        /// <summary>
        /// De-Highlights this <see cref="DoubleDigit"/> to hide it's arrows, and reset it's background.
        /// </summary>
        public void Deselect()
        {
            isSelected = false;
            ignoreFirstClick = true;
            
            HideArrows();
            SetSquircleColor(Timer.GetTheme().GetCurrentColorScheme().m_background);
            
            // Disable caret selection
            caret.raycastTarget = false;
            m_input.DeactivateInputField();
        }

        /// <summary>
        /// <remarks>Used as a <see cref="UnityEvent"/> on our input field.</remarks>
        /// </summary>
        public void SetValueEndEdit()
        {
            m_input.text = string.IsNullOrEmpty(m_input.text) ? "00" : m_input.text;
            SetValue(int.Parse(m_input.text));
            UpdateVisuals(format.GetDigitValue(m_digit));
        }

        /// <summary>
        /// Deselect input field and select ourself only. (So user doesn't interact with the input field but our
        /// arrows instead)
        /// </summary>
        public void DeselectInput()
        {
            // Disable caret selection
            caret.raycastTarget = false;
            m_input.DeactivateInputField();

            accumulatedSelectionTime = 0;
            
            // Select self (Deselect input field)
            m_selectable.Select();
        }

        /// <summary>
        /// Selects this selectable, or the input fields selectable.
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerClick(PointerEventData eventData)
        {
            // First click is reserved for digit selection, second click is for editing the input field
            if (ignoreFirstClick)
            {
                ignoreFirstClick = false;
                return;
            }

            if (isSelected)
            {
                caret.raycastTarget = true;
                m_input.ActivateInputField();
            }
        }

        /// <summary>
        /// Interacts with this <see cref="DoubleDigit"/>.
        /// </summary>
        /// <param name="eventData"></param>
        public void OnSubmit(BaseEventData eventData)
        {
            if (!caret.raycastTarget)
            {
                caret.raycastTarget = true;
                m_input.ActivateInputField();
            }
            else
            {
                caret.raycastTarget = false;
                m_input.DeactivateInputField(true);
            }
        }
        
        /// <summary>
        /// Applies our <see cref="Theme"/> changes to our referenced components when necessary.
        /// </summary>
        /// <param name="theme">The theme to apply on our referenced components.</param>
        public override void ColorUpdate(Theme theme)
        {
            ColorScheme currentColors = theme.GetCurrentColorScheme();
            
            if (isSelected)
            {
                // Cancel animation
                isColorAnimating = false;
                
                // Instantly swap color
                instanceMaterial.SetColor(SquircleColor, currentColors.m_backgroundHighlight);
            }
            else
            {
                SetSquircleColor(currentColors.m_background);
            }
            
            // Up arrow
            m_upArrow.m_icon.color = currentColors.m_foreground;
            
            // Down arrow
            m_downArrow.m_icon.color = currentColors.m_foreground;
            
            // Digit text
            SetTextColor(currentColors.m_foreground);
        }

        /// <summary>
        /// Returns our <see cref="Selectable"/> component.
        /// </summary>
        /// <returns></returns>
        public Selectable GetSelectable()
        {
            return m_selectable;
        }
    }
}