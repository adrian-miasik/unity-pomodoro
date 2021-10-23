using System;
using AdrianMiasik.Components.Core;
using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AdrianMiasik.Components
{
    public class DoubleDigit : MonoBehaviour, ISelectHandler, IPointerClickHandler, ISubmitHandler, IColorHook
    {
        [Header("References")] 
        [SerializeField] private Selectable selectable;
        [SerializeField] private Image background;

        [SerializeField] private ClickButton upArrow;
        [SerializeField] private TMP_InputField input;
        [SerializeField] private ClickButton downArrow;

        [Header("Color")] 
        [SerializeField] private float animationDuration = 0.25f;
        [SerializeField] private AnimationCurve animationRamp = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Animations")] 
        [SerializeField] private Animation pulseWobble;
        [SerializeField] private Animation tick;

        [HideInInspector] public DigitFormat.Digits digit;
        private DigitFormat format;
        private bool isInteractable = true;
        private bool isSelected;
        
        // Cache
        private PomodoroTimer timer;
        
        // Color animation
        [SerializeField] private Color color = Color.white;
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
        public UnityEvent OnSelection;
        public UnityEvent OnDigitChange; // Invoked only when timer is running

        public void Initialize(PomodoroTimer _timer, DigitFormat _format, DigitFormat.Digits _digit)
        {
            format = _format;
            digit = _digit;
            timer = _timer;
            timer.GetTheme().RegisterColorHook(this);
            
            // Disable run time caret interactions - We want to run input through this classes input events
            caret = input.textViewport.GetChild(0).GetComponent<TMP_SelectionCaret>();
            if (caret)
            {
                // Prevent input field from getting selection focus
                caret.raycastTarget = false;
            }
            
            HideArrows();
            ColorUpdate(timer.GetTheme());
        }

        public void UpdateVisuals(int _value)
        {
            SetTextLabel(_value);
            UpdateArrows();
            pulseWobble.Stop();
        }
        
        private void Update()
        {
            if (isColorAnimating)
            {
                accumulatedColorTime += Time.deltaTime;
                progress = accumulatedColorTime / animationDuration;
                if (progress >= 1)
                {
                    isColorAnimating = false;
                }

                float _evaluatedTime = animationRamp.Evaluate(progress);

                // Animate and modify
                instanceMaterial.SetColor(SquircleColor, Color.Lerp(startingColor, endingColor, _evaluatedTime));

                // Apply
                background.material = instanceMaterial;
            }

            if (isSelected)
            {
                if (Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace))
                {
                    if (!input.isFocused)
                    {
                        SetValue(0);
                        UpdateVisuals(format.GetDigitValue(digit));
                    }
                }
                
                // Scroll input
                if (Input.mouseScrollDelta.y > 0)
                {
                    if (format.CanIncrementOne(digit))
                    {
                        upArrow.OnPointerClick(null);
                    }
                }
                else if (Input.mouseScrollDelta.y < 0)
                {
                    if (format.CanDecrementOne(digit))
                    {
                        downArrow.OnPointerClick(null);
                    }
                }

                // Arrow keys : Up arrow
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    upArrow.OnPointerClick(null);
                    upArrow.Hold();
                }
                else if (Input.GetKeyUp(KeyCode.UpArrow))
                {
                    upArrow.Release();
                }

                // Arrow keys : Down arrow
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    downArrow.OnPointerClick(null);
                    downArrow.Hold();
                }
                else if (Input.GetKeyUp(KeyCode.DownArrow))
                {
                    downArrow.Release();
                }

                // Automatic digit deselection on mobile
                if (Input.touchSupported && !input.isFocused)
                {
                    accumulatedSelectionTime += Time.deltaTime;

                    if (accumulatedSelectionTime > deselectionDuration)
                    {
                        accumulatedSelectionTime = 0;
                        DeselectInput();
                        format.GetTimer().ClearSelection();
                    }
                }
            }
        }

        private void SetSquircleColor(Color _color)
        {
            // Create instance material
            if (instanceMaterial == null)
            {
                instanceMaterial = new Material(background.material);
            }

            startingColor = timer.GetTheme().GetCurrentColorScheme().backgroundHighlight;
            // startingColor = instanceMaterial.GetColor(SquircleColor);

            endingColor = _color;
            accumulatedColorTime = 0;
            isColorAnimating = true;
        }

        // Unity Event
        public void PlayTickAnimation()
        {
            // Prevent milliseconds from animating on tick (due to how fast it ticks)
            if (digit == DigitFormat.Digits.MILLISECONDS)
            {
                return;
            }
            
            tick.Stop();
            tick.Play();
        }

        public bool IsTickAnimationPlaying()
        {
            return tick.isPlaying;
        }

        public void ResetTextPosition()
        {
            input.textComponent.rectTransform.anchorMin = Vector2.zero;
            input.textComponent.rectTransform.anchorMax = Vector2.one;
        }
        
        public string GetDigitsLabel()
        {
            return input.text;
        }
        
        public void HideArrows()
        {
            upArrow.Hide();
            downArrow.Hide();
        }

        private void ShowArrows()
        {
            if (format != null)
            {
                if (format.CanIncrementOne(digit))
                {
                    upArrow.Show();
                }

                if (format.CanDecrementOne(digit))
                {
                    downArrow.Show();        
                }
            }
        }

        public void Lock()
        {
            isInteractable = false;
            input.interactable = false;
        }

        public void Unlock()
        {
            isInteractable = true;
            input.interactable = true;
        }
        
        public void SetTextColor(Color _newColor)
        {
            input.textComponent.color = _newColor;
        }
        
        /// <summary>
        /// Sets the digit data to the provided input.
        /// This is different from the current visuals of this digit.
        /// If you are looking to modify the digit visual value <see cref="SetTextLabel"/>
        /// </summary>
        /// <param name="_value"></param>
        public void SetValue(int _value)
        {
            if (format == null)
                return;
            
            format.SetDigit(digit, _value);
        }
        
        /// <summary>
        /// Sets the digit label to the provided input.
        /// This is different from setting the digit time.
        /// If you are looking to modify the users set/edited time <see cref="SetValue"/>.
        /// Also if you are looking to update arrows as well as the label <seealso cref="UpdateVisuals"/>
        /// </summary>
        /// <param name="_value"></param>
        public void SetTextLabel(int _value)
        {
            // If this digit value is actually different...
            if (_value.ToString("D2") != input.text)
            {
                // Update the digit
                input.text = _value.ToString("D2");

                if (format.GetTimer().state == PomodoroTimer.States.RUNNING)
                {
                    OnDigitChange?.Invoke();
                }
            }
        }
        
        public void IncrementOne()
        {
            if (format == null) 
                return;
            
            format.IncrementOne(digit);
            UpdateVisuals(format.GetDigitValue(digit));
            pulseWobble.Play();
            accumulatedSelectionTime = 0;
        }

        public void DecrementOne()
        {
            if (format == null) 
                return;
            
            format.DecrementOne(digit);
            UpdateVisuals(format.GetDigitValue(digit));
            pulseWobble.Play();
            accumulatedSelectionTime = 0;
        }

        private void UpdateArrows()
        {
            // Up Arrow
            if (format.CanIncrementOne(digit))
            {
                upArrow.Show();
            }
            else
            {
                upArrow.Hide();
            }
            
            // Down Arrow
            if (format.CanDecrementOne(digit))
            {
                downArrow.Show();
            }
            else
            {
                downArrow.Hide();
            }
        }

        public void Highlight()
        {
            OnSelect(null, false);
        }
        
        public void OnSelect(BaseEventData _eventData)
        {
            OnSelect(_eventData, true);
        }

        private void OnSelect(BaseEventData _eventData, bool _setSelection)
        {
            if (!isInteractable)
            {
                return;
            }

            isSelected = true;
            accumulatedSelectionTime = 0;
            
            ShowArrows();
            SetSquircleColor(timer.GetTheme().GetCurrentColorScheme().backgroundHighlight);

            if (_setSelection)
            {
                format.GetTimer().SetSelection(this);
            }

            OnSelection.Invoke();
        }

        public void Deselect()
        {
            isSelected = false;
            ignoreFirstClick = true;
            
            HideArrows();
            SetSquircleColor(timer.GetTheme().GetCurrentColorScheme().background);
            
            // Disable caret selection
            caret.raycastTarget = false;
            input.DeactivateInputField();
        }

        // Unity Event
        public void SetValueEndEdit()
        {
            input.text = string.IsNullOrEmpty(input.text) ? "00" : input.text;
            SetValue(int.Parse(input.text));
            UpdateVisuals(format.GetDigitValue(digit));
        }

        public void DeselectInput()
        {
            // Disable caret selection
            caret.raycastTarget = false;
            input.DeactivateInputField();

            accumulatedSelectionTime = 0;
            
            // Select self (Deselect input field)
            selectable.Select();
        }

        public void OnPointerClick(PointerEventData _eventData)
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
                input.ActivateInputField();
            }
        }

        public void OnSubmit(BaseEventData _eventData)
        {
            if (!caret.raycastTarget)
            {
                caret.raycastTarget = true;
                input.ActivateInputField();
            }
            else
            {
                caret.raycastTarget = false;
                input.DeactivateInputField(true);
            }
        }

        public void ColorUpdate(Theme _theme)
        {
            ColorScheme _currentColors = _theme.GetCurrentColorScheme();
            
            if (isSelected)
            {
                // Cancel animation
                isColorAnimating = false;
                
                // Instantly swap color
                instanceMaterial.SetColor(SquircleColor, _currentColors.backgroundHighlight);
            }
            else
            {
                SetSquircleColor(_currentColors.background);
            }
            
            // Up arrow
            SVGImage _upArrow = upArrow.visibilityTarget.GetComponent<SVGImage>();
            if (_upArrow != null)
            {
                _upArrow.color = _currentColors.foreground;
            }
            
            // Down arrow
            SVGImage _downArrow = downArrow.visibilityTarget.GetComponent<SVGImage>();
            if (_downArrow != null)
            {
                _downArrow.color = _currentColors.foreground;
            }

            // Digit text
            SetTextColor(_currentColors.foreground);
        }

        public Selectable GetSelectable()
        {
            return selectable;
        }
    }
}