using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AdrianMiasik.Components.Core
{
    public class BooleanToggle : Toggle
    {
        public SVGImage icon;
        
        // False / Off
        public Sprite falseSprite;
        public Color falseColor;
        public float falseZRotation;
        
        // True / On
        public Sprite trueSprite;
        public Color trueColor;
        public float trueZRotation;
        
        // Unity Events
        public UnityEvent OnSetToTrueClick;
        public UnityEvent OnSetToFalseClick;

        public void Initialize(bool isToggled, bool invokeEvents = false)
        {
            isOn = isToggled;

            UpdateToggle(invokeEvents);
        }

        // Unity Event
        public void UpdateToggle(bool invokeEvents)
        {
            if (isOn)
            {
                icon.sprite = trueSprite;
                icon.color = trueColor;
                icon.transform.rotation = Quaternion.Euler(new Vector3(0,0,trueZRotation));
                if (invokeEvents)
                {
                    OnSetToTrueClick.Invoke();
                }
            }
            else
            {
                icon.sprite = falseSprite;
                icon.color = falseColor;
                icon.transform.rotation = Quaternion.Euler(new Vector3(0,0,falseZRotation));
                if (invokeEvents)
                {
                    OnSetToFalseClick.Invoke();
                }
            }
        }
    }
}
