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

        private Theme theme;

        public void Initialize(bool _isOn, Theme _theme, bool _invokeEvents = false)
        {
            isOn = _isOn;
            theme = _theme;
            
            _theme.RegisterColorHook(this);
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
            
            ColorUpdate(theme);
        }

        public void ColorUpdate(Theme _theme)
        {
            switch (isOn)
            {
                case true:
                    icon.color = _theme.GetCurrentColorScheme().close;
                    break;
                case false:
                    icon.color = _theme.GetCurrentColorScheme().backgroundHighlight;
                    break;
            }
        }
    }
}
