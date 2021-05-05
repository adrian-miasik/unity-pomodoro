using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AdrianMiasik.Components
{
    public class InformationToggle : Toggle
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
        
        // Cache
        private PomodoroTimer timer;
        
        // Unity Events
        public UnityEvent OnSetToTrueClick;
        public UnityEvent OnSetToFalseClick;
        
        public void Initialize(PomodoroTimer timer, bool isToggled = false)
        {
            this.timer = timer;
            isOn = isToggled;

            UpdateToggle();
        }

        // Unity Event
        public void UpdateToggle()
        {
            if (isOn)
            {
                timer.ShowInfo();
                icon.sprite = trueSprite;
                icon.color = trueColor;
                icon.transform.rotation = Quaternion.Euler(new Vector3(0,0,trueZRotation));
                OnSetToTrueClick.Invoke();
            }
            else
            {
                timer.HideInfo();
                icon.sprite = falseSprite;
                icon.color = falseColor;
                icon.transform.rotation = Quaternion.Euler(new Vector3(0,0,falseZRotation));
                OnSetToFalseClick.Invoke();
            }
        }
    }
}
