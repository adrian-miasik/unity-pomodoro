using System.Collections.Generic;
using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
using TMPro;
using UnityEngine;

namespace AdrianMiasik.Components
{
    public class CompletionLabel : MonoBehaviour, IColorHook
    {
        [SerializeField] private List<TMP_Text> m_labels = new List<TMP_Text>();

        public void Initialize(PomodoroTimer pomodoroTimer)
        {
            pomodoroTimer.GetTheme().RegisterColorHook(this);
            ColorUpdate(pomodoroTimer.GetTheme());
        }

        public void ColorUpdate(Theme theme)
        {
            foreach (TMP_Text text in m_labels)
            {
                text.color = theme.GetCurrentColorScheme().m_complete;
            }
        }
    }
}
