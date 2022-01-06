using System.Collections.Generic;
using AdrianMiasik.Components.Core;
using AdrianMiasik.Components.Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace AdrianMiasik.Components
{
    public class TomatoCounter : ThemeElement
    {
        [SerializeField] private HorizontalLayoutGroup m_horizontal;
        [SerializeField] private Tomato m_tomatoPrefab;

        [SerializeField] private List<Tomato> m_tomatoes = new List<Tomato>();
        private int lastFilledTomatoIndex;
        
        public override void Initialize(PomodoroTimer pomodoroTimer, bool updateColors = true)
        {
            // TODO: (4) Setting property for what defines a long break
            //CreateTomato(pomodoroTimer, 4);
            
            lastFilledTomatoIndex = 0;

            base.Initialize(pomodoroTimer, updateColors);
        }

        /// <summary>
        /// Create, init, cache our tomatoes.
        /// </summary>
        /// <param name="timer">Main class reference</param>
        /// <param name="count">How many tomatoes do you want to create?</param>
        /// <returns></returns>
        private void CreateTomato(PomodoroTimer timer, int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                Tomato tomato = Instantiate(m_tomatoPrefab, m_horizontal.transform);
                tomato.Initialize(timer);
                m_tomatoes.Add(tomato);
            }
        }

        /// <summary>
        /// Fills in the latest tomato
        /// </summary>
        /// <returns>Are we ready for a long break?</returns>
        public void FillTomato()
        {
            m_tomatoes[lastFilledTomatoIndex].Complete();

            // Increment / wrap new tomato index
            lastFilledTomatoIndex++;
            lastFilledTomatoIndex = ListHelper.Wrap(lastFilledTomatoIndex, m_tomatoes.Count);
            
            // Check for completion
            if (lastFilledTomatoIndex == 0)
            {
                Timer.ReadyForLongBreak();
            }
        }

        public void SetHorizontalScale(Vector3 newScale)
        {
            m_horizontal.transform.localScale = newScale;
        }
        
        public void ConsumeTomatoes()
        {
            foreach (Tomato tomato in m_tomatoes)
            {
                tomato.Reset();
            }
        }
    }
}
