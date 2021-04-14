using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AdrianMiasik
{
    public class DoubleDigit : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image background;
        [SerializeField] private TMP_InputField input;

        private Material _instanceMaterial;
        private static readonly int SquircleColor = Shader.PropertyToID("Color_297012532bf444df807f8743bdb7e4fd");

        private PomodoroTimer _timer;

        public void Initialize(PomodoroTimer timer)
        {
            _timer = timer;
        }

        public void SetDigits(int digits)
        {
            input.text = digits.ToString("D2");
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            // Create
            VerifyInstanceMaterial();

            // Modify
            _instanceMaterial.SetColor(SquircleColor, new Color(0.91f, 0.91f, 0.91f));

            // Apply
            background.material = _instanceMaterial;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // Create
            VerifyInstanceMaterial();

            // Modify
            _instanceMaterial.SetColor(SquircleColor, Color.clear);

            // Apply
            background.material = _instanceMaterial;
        }

        private void VerifyInstanceMaterial()
        {
            // Create instance material
            if (_instanceMaterial == null)
            {
                _instanceMaterial = new Material(background.material);
            }
        }

        public void Lock()
        {
            input.interactable = false;
        }

        public void Unlock()
        {
            input.interactable = true;
        }

        public void SetHours(string hours)
        {
            if (_timer != null)
            {
                _timer.SetHours(hours);
            }
        }

        public void SetMinutes(string minutes)
        {
            if (_timer != null)
            {
                _timer.SetMinutes(minutes);
            }
        }

        public void SetSeconds(string seconds)
        {
            if (_timer != null)
            {
                _timer.SetSeconds(seconds);
            }
        }
    }
}