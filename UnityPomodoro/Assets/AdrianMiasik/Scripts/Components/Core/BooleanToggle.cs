using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AdrianMiasik.Components.Core
{
    public class BooleanToggle : Toggle, IColorHook
    {
        public SVGImage icon;
        
        // False / Off
        public Sprite falseSprite;
        public float falseZRotation;
        
        // True / On
        public Sprite trueSprite;
        public float trueZRotation;
        
        // Unity Events
        public UnityEvent onSetToTrueClick;
        public UnityEvent onSetToFalseClick;

        // Cache
        private PomodoroTimer timer;
        private bool isInitialized;

        // Override
        private bool overrideTrueColor;
        private Color overridenTrueColor;
        private bool overrideFalseColor;
        private Color overridenFalseColor;
        
        public void OverrideTrueColor(Color _color)
        {
            overrideTrueColor = true;
            overridenTrueColor = _color;
        }

        /// <summary>
        /// Note: Needs to be invoked before initialize.<see cref="Initialize"/>
        /// </summary>
        /// <param name="_color"></param>
        public void OverrideFalseColor(Color _color)
        {
            overrideFalseColor = true;
            overridenFalseColor = _color;
        }
        
        public void Initialize(PomodoroTimer _timer, bool _isOn, bool _invokeEvents = false)
        {
            timer = _timer;
            isOn = _isOn;
            isInitialized = true;
            
            _timer.GetTheme().RegisterColorHook(this);
            ColorUpdate(_timer.GetTheme());
            UpdateToggle(_invokeEvents);
        }

        // Unity Event
        public void UpdateToggle(bool _invokeEvents)
        {
            if (isOn)
            {
                icon.sprite = trueSprite;
                icon.transform.rotation = Quaternion.Euler(new Vector3(0,0,trueZRotation));
                if (_invokeEvents)
                {
                    onSetToTrueClick.Invoke();
                }
            }
            else
            {
                icon.sprite = falseSprite;
                icon.transform.rotation = Quaternion.Euler(new Vector3(0,0,falseZRotation));
                if (_invokeEvents)
                {
                    onSetToFalseClick.Invoke();
                }
            }

            if (isInitialized)
            {
                ColorUpdate(timer.GetTheme());
            }
        }
        
        public void ColorUpdate(Theme _theme)
        {
            switch (isOn)
            {
                case true:
                    icon.color = overrideTrueColor ? overridenTrueColor : _theme.GetCurrentColorScheme().close;
                    break;
                case false:
                    icon.color = overrideFalseColor ? overridenFalseColor : _theme.GetCurrentColorScheme().backgroundHighlight;
                    break;
            }
        }
    }
}
