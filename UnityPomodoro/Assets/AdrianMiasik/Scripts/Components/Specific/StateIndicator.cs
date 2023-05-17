using AdrianMiasik.Components.Base;
using AdrianMiasik.Components.Core.Helpers;
using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
using Unity.VectorGraphics;
using UnityEngine;

namespace AdrianMiasik.Components.Specific
{
    public class StateIndicator : ThemeElement, ITimerState
    {
        [Header("State Indicator - References")]
        [SerializeField] private RectTransform m_self;
        [SerializeField] private SVGImage m_icon;
        [SerializeField] private SVGImage m_circleBackground;

        [Header("Sprites (Icons)")]
        [SerializeField] private Sprite m_setup;
        [SerializeField] private Sprite m_running;
        [SerializeField] private Sprite m_paused;
        [SerializeField] private Sprite m_complete;

        [Header("Scale")]
        [SerializeField] private float m_defaultScale = 0.6f;
        [SerializeField] private float m_pausedScale = 0.5f;

        [Header("Rect Transforms (Position and Layout)")]
        [SerializeField] private RectTransform m_setupRT;
        [SerializeField] private RectTransform m_runningAndPausedRT;
        [SerializeField] private RectTransform m_completeRT;

        public override void ColorUpdate(Theme theme)
        {
            base.ColorUpdate(theme);

            // Apply state color changes based on new theme
            StateUpdate(Timer.m_state, theme);
        }

        public void StateUpdate(PomodoroTimer.States state, Theme theme)
        {
            Color stateColor = theme.GetCurrentColorScheme().m_backgroundHighlight;
            Sprite stateSprite = null;
            RectTransform stateRectTransform = null;

            switch (state)
            {
                case PomodoroTimer.States.SETUP:
                    // stateColor = Color.blue;
                    stateSprite = m_setup;
                    stateRectTransform = m_setupRT;
                    m_icon.transform.localScale = Vector3.one * m_defaultScale;
                    break;

                case PomodoroTimer.States.RUNNING:
                    // stateColor = Color.green;
                    stateSprite = m_running;
                    stateRectTransform = m_runningAndPausedRT;
                    m_icon.transform.localScale = Vector3.one * m_defaultScale;
                    break;

                case PomodoroTimer.States.PAUSED:
                    // stateColor = Color.grey;
                    stateSprite = m_paused;
                    stateRectTransform = m_runningAndPausedRT;
                    m_icon.transform.localScale = Vector3.one * m_pausedScale;
                    break;

                case PomodoroTimer.States.COMPLETE:
                    stateColor = theme.GetCurrentColorScheme().m_running;
                    stateSprite = m_complete;
                    stateRectTransform = m_completeRT;
                    m_icon.transform.localScale = Vector3.one * m_defaultScale;
                    break;

                default:
                    Debug.LogWarning("This state is not currently supported by the StateIndicator.", gameObject);
                    break;
            }

            // Set color
            m_icon.color = stateColor;
            m_circleBackground.color = stateColor;

            // Set sprite
            m_icon.sprite = stateSprite;

            // Set this components RectTransform to use the desired state RectTransform (Apply new RT based on state
            // using inspector pre-defined RT's)
            RectTransformHelper.ApplyRectTransform(stateRectTransform, m_self);
        }
    }
}
