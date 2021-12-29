using AdrianMiasik.Components.Core;
using AdrianMiasik.Components.Wrappers;
using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AdrianMiasik.Components
{
    public class DoubleDigit : MonoBehaviour, ISelectHandler, IPointerClickHandler, ISubmitHandler, IColorHook
    {
        [Header("References")] 
        [SerializeField] private Selectable m_selectable;
        [SerializeField] private Image m_background;

        [SerializeField] private ClickButtonIcon m_upArrow;
        [SerializeField] private TMP_InputField m_input;
        [SerializeField] private ClickButtonIcon m_downArrow;
        
        [Header("Animations")] 
        [SerializeField] private float m_animationDuration = 1f;
        [SerializeField] private AnimationCurve m_animationRamp = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private Animation m_pulseWobble;
        [SerializeField] private Animation m_tick;
        
        [HideInInspector] public DigitFormat.Digits m_digit;
        
        // Cache
        private PomodoroTimer timer;
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

        // Unity Events
        public UnityEvent m_onSelection;
        public UnityEvent m_onDigitChange; // Invoked only when timer is running

        public void Initialize(PomodoroTimer pomodoroTimer, DigitFormat digitFormat, DigitFormat.Digits digit)
        {
            format = digitFormat;
            m_digit = digit;
            timer = pomodoroTimer;
            timer.GetTheme().RegisterColorHook(this);
            
            // Disable run time caret interactions - We want to run input through this classes input events
            caret = m_input.textViewport.GetChild(0).GetComponent<TMP_SelectionCaret>();
            if (caret)
            {
                // Prevent input field from getting selection focus
                caret.raycastTarget = false;
            }
            
            HideArrows();
            ColorUpdate(timer.GetTheme());
        }

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

            startingColor = timer.GetTheme().GetCurrentColorScheme().m_backgroundHighlight;
            // startingColor = instanceMaterial.GetColor(SquircleColor);

            endingColor = color;
            accumulatedColorTime = 0;
            isColorAnimating = true;
        }

        // Unity Event
        public void PlayTickAnimation()
        {
            // Prevent milliseconds from animating on tick (due to how fast it ticks)
            if (m_digit == DigitFormat.Digits.MILLISECONDS)
            {
                return;
            }
            
            m_tick.Stop();
            m_tick.Play();
        }

        public bool IsTickAnimationPlaying()
        {
            return m_tick.isPlaying;
        }

        public void ResetTextPosition()
        {
            m_input.textComponent.rectTransform.anchorMin = Vector2.zero;
            m_input.textComponent.rectTransform.anchorMax = Vector2.one;
        }
        
        public string GetDigitsLabel()
        {
            return m_input.text;
        }
        
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

        public void Lock()
        {
            isInteractable = false;
            m_input.interactable = false;
        }

        public void Unlock()
        {
            isInteractable = true;
            m_input.interactable = true;
        }
        
        public void SetTextColor(Color newColor)
        {
            m_input.textComponent.color = newColor;
        }
        
        /// <summary>
        /// Sets the digit data to the provided input.
        /// This is different from the current visuals of this digit.
        /// If you are looking to modify the digit visual value <see cref="SetTextLabel"/>
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
        /// If you are looking to modify the users set/edited time <see cref="SetValue"/>.
        /// Also if you are looking to update arrows as well as the label <seealso cref="UpdateVisuals"/>
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
        
        public void IncrementOne()
        {
            if (format == null) 
                return;
            
            format.IncrementOne(m_digit);
            UpdateVisuals(format.GetDigitValue(m_digit));
            m_pulseWobble.Play();
            accumulatedSelectionTime = 0;
        }

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

        public void Highlight()
        {
            OnSelect(false);
        }
        
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
            SetSquircleColor(timer.GetTheme().GetCurrentColorScheme().m_backgroundHighlight);

            if (setSelection)
            {
                format.SetTimerSelection(this);
            }

            m_onSelection.Invoke();
        }

        public void Deselect()
        {
            isSelected = false;
            ignoreFirstClick = true;
            
            HideArrows();
            SetSquircleColor(timer.GetTheme().GetCurrentColorScheme().m_background);
            
            // Disable caret selection
            caret.raycastTarget = false;
            m_input.DeactivateInputField();
        }

        // Unity Event
        public void SetValueEndEdit()
        {
            m_input.text = string.IsNullOrEmpty(m_input.text) ? "00" : m_input.text;
            SetValue(int.Parse(m_input.text));
            UpdateVisuals(format.GetDigitValue(m_digit));
        }

        public void DeselectInput()
        {
            // Disable caret selection
            caret.raycastTarget = false;
            m_input.DeactivateInputField();

            accumulatedSelectionTime = 0;
            
            // Select self (Deselect input field)
            m_selectable.Select();
        }

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

        public void ColorUpdate(Theme theme)
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

        public Selectable GetSelectable()
        {
            return m_selectable;
        }

        public void SetTextScale(float scale = 1.55f)
        {
            m_input.textComponent.gameObject.transform.localScale = Vector3.one * scale;
            caret.gameObject.transform.localScale = Vector3.one * scale;
        }
    }
}