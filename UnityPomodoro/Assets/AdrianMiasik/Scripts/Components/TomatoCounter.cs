using AdrianMiasik.Components.Core;
using UnityEngine;
using UnityEngine.UI;

namespace AdrianMiasik.Components
{
    public class TomatoCounter : ThemeElement
    { 
        [SerializeField] private HorizontalLayoutGroup m_horizontal;

        public override void Initialize(PomodoroTimer pomodoroTimer, bool updateColors = true)
        {
            base.Initialize(pomodoroTimer, updateColors);

            Debug.Log("Initialized tomato counter");
        }
    }
}
