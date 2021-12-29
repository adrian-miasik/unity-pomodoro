using AdrianMiasik.ScriptableObjects;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.Events;

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

        // Cache
        private bool isInitialized;

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
            
            m_toggle.isOn = state;
            isInitialized = true;
            
            UpdateToggle(invokeEvents);
        }

        // Unity Event
        public void UpdateToggle(bool invokeEvents)
        {
            if (m_toggle.isOn)
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

            if (isInitialized)
            {
                ColorUpdate(Timer.GetTheme());
            }
        }

        public void SetToTrue()
        {
            m_toggle.isOn = true;
            UpdateToggle(false);
        }

        public void SetToFalse()
        {
            m_toggle.isOn = false;
            UpdateToggle(false);
        }
        
        public override void ColorUpdate(Theme theme)
        {
            switch (m_toggle.isOn)
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
