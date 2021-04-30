using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace AdrianMiasik.Components
{
    public class InformationToggle : Toggle
    {
        public SVGImage icon;
        
        // Off
        public Sprite offSprite;
        public Color offColor;
        public float offZRotation;
        
        // On
        public Sprite onSprite;
        public Color onColor;
        public float onZRotation;
        
        // Cache
        private PomodoroTimer timer;
        
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
                icon.sprite = onSprite;
                icon.color = onColor;
                icon.transform.rotation = Quaternion.Euler(new Vector3(0,0,onZRotation));
            }
            else
            {
                timer.HideInfo();
                icon.sprite = offSprite;
                icon.color = offColor;
                icon.transform.rotation = Quaternion.Euler(new Vector3(0,0,offZRotation));
            }
        }
    }
}
