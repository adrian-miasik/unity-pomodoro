using AdrianMiasik.Interfaces;
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
        public UnityEvent OnSetToTrueClick;
        public UnityEvent OnSetToFalseClick;

        private Theme theme;

        public void Initialize(bool isOn, Theme theme, bool invokeEvents = false)
        {
            this.isOn = isOn;
            this.theme = theme;
            
            theme.RegisterColorHook(this);
            UpdateToggle(invokeEvents);
        }

        // Unity Event
        public void UpdateToggle(bool invokeEvents)
        {
            if (isOn)
            {
                icon.sprite = trueSprite;
                icon.transform.rotation = Quaternion.Euler(new Vector3(0,0,trueZRotation));
                if (invokeEvents)
                {
                    OnSetToTrueClick.Invoke();
                }
            }
            else
            {
                icon.sprite = falseSprite;
                icon.transform.rotation = Quaternion.Euler(new Vector3(0,0,falseZRotation));
                if (invokeEvents)
                {
                    OnSetToFalseClick.Invoke();
                }
            }
            
            ColorUpdate(theme);
        }

        public void ColorUpdate(Theme theme)
        {
            switch (isOn)
            {
                case true:
                    icon.color = theme.GetCurrentColorScheme().close;
                    break;
                case false:
                    icon.color = theme.GetCurrentColorScheme().selection;
                    break;
            }
        }
    }
}
