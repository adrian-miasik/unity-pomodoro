using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AdrianMiasik
{
    public class DoubleDigit : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image background;
        [SerializeField] private ClickButton upArrow;
        [SerializeField] private TMP_InputField input;
        [SerializeField] private ClickButton downArrow;
        
        private PomodoroTimer.Digits digit;
        private PomodoroTimer timer;
        private int digits;
        private bool isInteractable;

        private Material _instanceMaterial;
        private static readonly int SquircleColor = Shader.PropertyToID("Color_297012532bf444df807f8743bdb7e4fd");

        public void Initialize(PomodoroTimer.Digits digit, PomodoroTimer timer, int digits)
        {
            this.digit = digit;
            this.timer = timer;
            this.digits = digits;

            SetDigitsLabel(digits);
            HideArrows();
        }

        public void SetDigitsLabel(int digits)
        {
            input.text = digits.ToString("D2");
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            SetSquircleColor(new Color(0.91f, 0.91f, 0.91f));
            
            if (isInteractable)
            {
                ShowArrows();
            }
        }
        
        private void SetSquircleColor(Color color)
        {
            // Create instance material
            if (_instanceMaterial == null)
            {
                _instanceMaterial = new Material(background.material);
            }

            // Modify
            _instanceMaterial.SetColor(SquircleColor, color);

            // Apply
            background.material = _instanceMaterial;
        }
        
        private void ShowArrows()
        {
            upArrow.Show();
            downArrow.Show();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            SetSquircleColor(Color.clear);
            HideArrows();
        }
        
        private void HideArrows()
        {
            upArrow.Hide();
            downArrow.Hide();
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