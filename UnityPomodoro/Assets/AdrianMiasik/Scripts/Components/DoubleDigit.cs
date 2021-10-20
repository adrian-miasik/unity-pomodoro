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

        private DigitFormat format;
        private DigitFormat.Digits digit;
        private bool isInteractable;
        private bool isSelected;

        // Color
        private Theme cachedTheme;
        
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

        public void Initialize(DigitFormat _format, DigitFormat.Digits _digit, int _value, Theme _theme)
        {
            format = _format;
            digit = _digit;
            cachedTheme = _theme;
            cachedTheme.RegisterColorHook(this);
            
            // Disable run time caret interactions - We want to run input through this classes input events
            caret = input.textViewport.GetChild(0).GetComponent<TMP_SelectionCaret>();
            if (caret)
            {
                // Prevent input field from getting selection focus
                caret.raycastTarget = false;
            }

            UpdateVisuals(_value);
        }

        private void UpdateVisuals(int _value)
        {
            SetSquircleColor(color);
            SetDigitsLabel(_value);
            HideArrows();
            pulseWobble.Stop();
        }

        public DigitFormat.Digits GetDigit()
        {
            return digit;
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

            startingColor = instanceMaterial.GetColor(SquircleColor);

            endingColor = _color;
            accumulatedColorTime = 0;
            isColorAnimating = true;
        }

        public void SetDigitsLabel(int _digits)
        {
            // If this digit value is actually different...
            if (_digits.ToString("D2") != input.text)
            {
                // Update the digit
                input.text = _digits.ToString("D2");

                if (format.GetTimer().state == PomodoroTimer.States.RUNNING)
                {
                    OnDigitChange?.Invoke();
                }
            }
        }

        // Unity Event
        public void PlayTickAnimation()
        {
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

        public Selectable GetSelectable()
        {
            return selectable;
        }
        
        private void HideArrows()
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
        
        // Unity Events
        public void SetValue(int _value)
        {
            if (format == null)
                return;
            
            format.SetDigit(digit, string.IsNullOrEmpty(_value.ToString()) ? 0 : int.Parse(_value.ToString()));
            UpdateArrows();
            UpdateVisuals(_value);
        }
        
        public void IncrementOne()
        {
            if (format == null) 
                return;
            
            format.IncrementOne(digit);
            SetValue(format.GetDigitValue(digit));
            SetDigitsLabel(format.GetDigitValue(digit));
            pulseWobble.Stop();
            pulseWobble.Play();
            UpdateArrows();
            accumulatedSelectionTime = 0;
        }

        public void DecrementOne()
        {
            if (format == null) 
                return;
            
            format.DecrementOne(digit);
            SetValue(format.GetDigitValue(digit));
            SetDigitsLabel(format.GetDigitValue(digit));
            pulseWobble.Stop();
            pulseWobble.Play();
            UpdateArrows();
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
            SetSquircleColor(cachedTheme.GetCurrentColorScheme().backgroundHighlight);

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
            SetSquircleColor(cachedTheme.GetCurrentColorScheme().background);
            
            // Disable caret selection
            caret.raycastTarget = false;
            input.DeactivateInputField();
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
    }
}