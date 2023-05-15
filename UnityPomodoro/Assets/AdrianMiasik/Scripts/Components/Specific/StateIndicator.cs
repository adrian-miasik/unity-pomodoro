using System;
using AdrianMiasik.Components.Base;
using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace AdrianMiasik.Components.Specific
{
    public class StateIndicator : ThemeElement, ITimerState
    {
        [SerializeField] private Image m_icon;

        public void StateUpdate(PomodoroTimer.States state, Theme theme)
        {
            Color stateColor = Color.black;

            switch (state)
            {
                case PomodoroTimer.States.SETUP:
                    stateColor = Color.blue;
                    break;
                case PomodoroTimer.States.RUNNING:
                    stateColor = Color.green;
                    break;
                case PomodoroTimer.States.PAUSED:
                    stateColor = Color.grey;
                    break;
                case PomodoroTimer.States.COMPLETE:
                    stateColor = Color.red;
                    break;
                default:
                    Debug.LogWarning("This state is not currently supported by the StateIndicator.", gameObject);
                    break;
            }

            m_icon.color = stateColor;
        }
    }
}
