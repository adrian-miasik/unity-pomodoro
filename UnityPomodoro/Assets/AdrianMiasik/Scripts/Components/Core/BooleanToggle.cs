using AdrianMiasik.Components.Wrappers;
using AdrianMiasik.ScriptableObjects;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AdrianMiasik.Components.Core
{
    public class BooleanToggle : ThemeElementToggle
    {
        public SVGImage m_icon;
        
        // False / Off
        public Sprite m_falseSprite;
        public float m_falseZRotation;
        
        // True / On
        public Sprite m_trueSprite;
        public float m_trueZRotation;
        
        // Unity Events
        public UnityEvent m_onSetToTrueClick;
        public UnityEvent m_onSetToFalseClick;
        
        // Override
        private bool overrideTrueColor;
        private Color overridenTrueColor;
        private bool overrideFalseColor;
        private Color overridenFalseColor;
        
        /// <summary>
        /// Note: Needs to be invoked before initialize.<see cref="Initialize"/>
        /// </summary>
        /// <param name="color"></param>
        public void OverrideTrueColor(Color color)
        {
            overrideTrueColor = true;
            overridenTrueColor = color;
        }

        /// <summary>
        /// Note: Needs to be invoked before initialize.<see cref="Initialize"/>
        /// </summary>
        /// <param name="color"></param>
        public void OverrideFalseColor(Color color)
        {
            overrideFalseColor = true;
            overridenFalseColor = color;
        }
        
        public void Initialize(PomodoroTimer pomodoroTimer, bool state, bool invokeEvents = false)
        {
            base.Initialize(pomodoroTimer);
            
            isOn = state;

            UpdateToggle(invokeEvents);
        }

        // Unity Event
        public void UpdateToggle(bool invokeEvents)
        {
            if (isOn)
            {
                m_icon.sprite = m_trueSprite;
                m_icon.transform.rotation = Quaternion.Euler(new Vector3(0,0,m_trueZRotation));
                if (invokeEvents)
                {
                    m_onSetToTrueClick.Invoke();
                }
            }
            else
            {
                m_icon.sprite = m_falseSprite;
                m_icon.transform.rotation = Quaternion.Euler(new Vector3(0,0,m_falseZRotation));
                if (invokeEvents)
                {
                    m_onSetToFalseClick.Invoke();
                }
            }

            if (IsInitialized)
            {
                ColorUpdate(Timer.GetTheme());
            }
        }

        public void SetToTrue()
        {
            isOn = true;
            UpdateToggle(false);
        }

        public void SetToFalse()
        {
            isOn = false;
            UpdateToggle(false);
        }
        
        public override void ColorUpdate(Theme theme)
        {
            switch (isOn)
            {
                case true:
                    m_icon.color = overrideTrueColor ? overridenTrueColor : theme.GetCurrentColorScheme().m_close;
                    break;
                case false:
                    m_icon.color = overrideFalseColor ? overridenFalseColor : theme.GetCurrentColorScheme().m_backgroundHighlight;
                    break;
            }
        }
    }
}
