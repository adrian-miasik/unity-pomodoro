using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AdrianMiasik
{
    public class DoubleDigit : MonoBehaviour, ISelectHandler, IPointerClickHandler
    {
        [Header("References")] 
        [SerializeField] private Image background;
        [SerializeField] private ClickButton upArrow;
        [SerializeField] private TMP_InputField input;
        [SerializeField] private ClickButton downArrow;

        [Header("Color")] 
        [SerializeField] private float animationDuration = 0.25f;

        [SerializeField] private AnimationCurve animationRamp = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private PomodoroTimer.Digits digit;
        private PomodoroTimer timer;
        private bool isInteractable;
        private bool isSelected;
        
        // Color animation
        [SerializeField] private Color color = Color.white;
        [SerializeField] private Color selectionColor;
        private Color startingColor;
        private Color endingColor;
        private bool isColorAnimating;
        private float accumulatedTime;
        private float progress;
        private TMP_SelectionCaret caret;
        private bool ignoreFirstClick = true;
        
        private Material _instanceMaterial;
        private static readonly int SquircleColor = Shader.PropertyToID("Color_297012532bf444df807f8743bdb7e4fd");

        public void Initialize(PomodoroTimer.Digits digit, PomodoroTimer timer, int digits)
        {
            this.digit = digit;
            this.timer = timer;
            
            // Disable run time caret interactions - We want to run input through this classes input events
            caret = input.textViewport.GetChild(0).GetComponent<TMP_SelectionCaret>();
            if (caret)
            {
                // Prevent input field from getting selection focus
                caret.raycastTarget = false;
            }
            
            SetSquircleColor(color);
            SetDigitsLabel(digits);
            HideArrows();
        }

        private void Update()
        {
            if (isColorAnimating)
            {
                accumulatedTime += Time.deltaTime;
                progress = accumulatedTime / animationDuration;
                if (progress >= 1)
                {
                    isColorAnimating = false;
                }
                
                float evaluatedTime = animationRamp.Evaluate(progress);

                // Animate and modify
                _instanceMaterial.SetColor(SquircleColor, Color.Lerp(startingColor, endingColor, evaluatedTime));

                // Apply
                background.material = _instanceMaterial;
            }

            if (isSelected)
            {
                // Scroll input
                if (Input.mouseScrollDelta.y > 0)
                {
                    IncrementOne();
                }
                else if (Input.mouseScrollDelta.y < 0)
                {
                    DecrementOne();
                }
            }
        }
        
        private void SetSquircleColor(Color color)
        {
            // Create instance material
            if (_instanceMaterial == null)
            {
                _instanceMaterial = new Material(background.material);
            }
            startingColor = _instanceMaterial.GetColor(SquircleColor);
            
            endingColor = color;
            accumulatedTime = 0;
            isColorAnimating = true;
        }

        public void SetDigitsLabel(int digits)
        {
            input.text = digits.ToString("D2");
        }

        private void HideArrows()
        {
            upArrow.Hide();
            downArrow.Hide();
        }

        private void ShowArrows()
        {
            if (timer != null)
            {
                if (timer.CanIncrementOne(digit))
                {
                    upArrow.Show();
                }

                if (timer.CanDecrementOne(digit))
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
        
        public void SetTextColor(Color newColor)
        {
            input.textComponent.color = newColor;
        }

        // Unity Events
        public void SetHours(string hours)
        {
            if (timer != null)
            {
                timer.SetHours(hours);
                UpdateArrows();
            }
        }

        public void SetMinutes(string minutes)
        {
            if (timer != null)
            {
                timer.SetMinutes(minutes);
                UpdateArrows();
            }
        }

        public void SetSeconds(string seconds)
        {
            if (timer != null)
            {
                timer.SetSeconds(seconds);
                UpdateArrows();
            }
        }

        public void IncrementOne()
        {
            if (timer != null)
            {
                timer.IncrementOne(digit);
                SetDigitsLabel(timer.GetDigitValue(digit));
                UpdateArrows();
            }
        }

        public void DecrementOne()
        {
            if (timer != null)
            {
                timer.DecrementOne(digit);
                SetDigitsLabel(timer.GetDigitValue(digit));
                UpdateArrows();
            }
        }

        private void UpdateArrows()
        {
            // Up Arrow
            if (timer.CanIncrementOne(digit))
            {
                upArrow.Show();
            }
            else
            {
                upArrow.Hide();
            }
            
            // Down Arrow
            if (timer.CanDecrementOne(digit))
            {
                downArrow.Show();
            }
            else
            {
                downArrow.Hide();
            }
        }
        
        public void OnSelect(BaseEventData eventData)
        {
            if (!isInteractable)
            {
                return;
            }
            
            ShowArrows();
            SetSquircleColor(selectionColor);
            
            timer.SetSelection(this);
            isSelected = true;
        }

        public void Deselect()
        {
            HideArrows();
            SetSquircleColor(color);

            isSelected = false;
            ignoreFirstClick = true;

            // Disable caret selection
            caret.raycastTarget = false;
            input.DeactivateInputField();
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
                input.ActivateInputField();
            }
        }
    }
}