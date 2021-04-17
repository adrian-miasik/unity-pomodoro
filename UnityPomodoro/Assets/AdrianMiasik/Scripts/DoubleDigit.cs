using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AdrianMiasik
{
    public class DoubleDigit : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
        
        // Color animation
        [SerializeField] private Color color = Color.white;
        [SerializeField] private Color selectionColor;
        private Color startingColor;
        private Color endingColor;
        private bool isColorAnimating;
        private float accumulatedTime;
        private float progress;
        
        private Material _instanceMaterial;
        private static readonly int SquircleColor = Shader.PropertyToID("Color_297012532bf444df807f8743bdb7e4fd");

        public void Initialize(PomodoroTimer.Digits digit, PomodoroTimer timer, int digits)
        {
            this.digit = digit;
            this.timer = timer;
            
            SetSquircleColor(color);
            SetDigitsLabel(digits);
            HideArrows();
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
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            SetSquircleColor(selectionColor);
            
            if (isInteractable)
            {
                ShowArrows();
            }
        }

        private void ShowArrows()
        {
            upArrow.Show();
            downArrow.Show();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            SetSquircleColor(color);
            HideArrows();
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

        // Unity Events
        public void SetHours(string hours)
        {
            if (timer != null)
            {
                timer.SetHours(hours);
            }
        }

        public void SetMinutes(string minutes)
        {
            if (timer != null)
            {
                timer.SetMinutes(minutes);
            }
        }

        public void SetSeconds(string seconds)
        {
            if (timer != null)
            {
                timer.SetSeconds(seconds);
            }
        }

        public void IncrementOne()
        {
            if (timer != null)
            {
                timer.IncrementOne(digit);
                SetDigitsLabel(timer.GetDigitValue(digit));
            }
        }

        public void DecrementOne()
        {
            if (timer != null)
            {
                timer.DecrementOne(digit);
                SetDigitsLabel(timer.GetDigitValue(digit));
            }
        }
    }
}