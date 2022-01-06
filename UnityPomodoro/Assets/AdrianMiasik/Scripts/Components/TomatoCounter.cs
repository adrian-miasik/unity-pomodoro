using AdrianMiasik.Components.Core;
using UnityEngine;
using UnityEngine.UI;

namespace AdrianMiasik.Components
{
    public class TomatoCounter : ThemeElement
    { 
        [SerializeField] private HorizontalLayoutGroup m_horizontal;
        [SerializeField] private Tomato m_tomatoPrefab;
        
        public override void Initialize(PomodoroTimer pomodoroTimer, bool updateColors = true)
        {
            base.Initialize(pomodoroTimer, updateColors);
            
            // TODO: Create tomatoes
            // TODO: Initialize tomatoes

            Debug.Log("Initialized tomato counter");
        }
    }
}
