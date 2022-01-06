using System.Collections.Generic;
using AdrianMiasik.Components.Core;
using UnityEngine;
using UnityEngine.UI;

namespace AdrianMiasik.Components
{
    public class TomatoCounter : ThemeElement
    {
        [SerializeField] private HorizontalLayoutGroup m_horizontal;
        [SerializeField] private Tomato m_tomatoPrefab;

        [SerializeField] private List<Tomato> m_tomatoes = new List<Tomato>();

        public override void Initialize(PomodoroTimer pomodoroTimer, bool updateColors = true)
        {
            base.Initialize(pomodoroTimer, updateColors);
        }
        
        /// <summary>
        /// Create, init, cache, and return a tomato
        /// </summary>
        /// <param name="timer"></param>
        /// <returns></returns>
        private Tomato CreateTomato(PomodoroTimer timer)
        {
            Tomato tomato = Instantiate(m_tomatoPrefab, m_horizontal.transform);
            tomato.Initialize(timer);
            m_tomatoes.Add(tomato);

            return tomato;
        }

        /// <summary>
        /// Creates a tomato for us and returns the tomato counter completion state
        /// </summary>
        /// <returns>Are we ready for a long break?</returns>
        public bool AddTomato()
        {
            Tomato tomato = CreateTomato(Timer);
            
            if (m_tomatoes.Count > 3)
            {
                Debug.Log("TODO: Modify for long break");
                
                foreach (Tomato t in m_tomatoes)
                {
                    // TODO: Remove from theme elements
                    Destroy(t.gameObject);
                }
                
                m_tomatoes.Clear();

                return true;
            }

            return false;
        }

        public void SetHorizontalScale(Vector3 newScale)
        {
            m_horizontal.transform.localScale = newScale;
        }
    }
}
