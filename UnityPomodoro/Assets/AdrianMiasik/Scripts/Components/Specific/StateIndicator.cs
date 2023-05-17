using AdrianMiasik.Components.Base;
using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
using Unity.VectorGraphics;
using UnityEngine;

namespace AdrianMiasik.Components.Specific
{
    public class StateIndicator : ThemeElement, ITimerState
    {
        [SerializeField] private SVGImage m_icon;
        [SerializeField] private SVGImage m_circleBackground;

        [Header("Sprites")]
        [SerializeField] private Sprite m_setup;
        [SerializeField] private Sprite m_running;
        [SerializeField] private Sprite m_paused;
        [SerializeField] private Sprite m_complete;

        public void StateUpdate(PomodoroTimer.States state, Theme theme)
        {
            Color stateColor = theme.GetCurrentColorScheme().m_backgroundHighlight;
            Sprite stateSprite = null;

            switch (state)
            {
                case PomodoroTimer.States.SETUP:
                    // stateColor = Color.blue;
                    stateSprite = m_setup;
                    break;

                case PomodoroTimer.States.RUNNING:
                    // stateColor = Color.green;
                    stateSprite = m_running;
                    break;

                case PomodoroTimer.States.PAUSED:
                    // stateColor = Color.grey;
                    stateSprite = m_paused;
                    break;

                case PomodoroTimer.States.COMPLETE:
                    // stateColor = Color.red;
                    stateSprite = m_complete;
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
        }
    }
}
