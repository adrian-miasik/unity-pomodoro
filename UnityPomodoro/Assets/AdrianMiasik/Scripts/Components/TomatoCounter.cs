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
        [SerializeField] private GameObject m_trashcan;
            
        [SerializeField] private List<Tomato> m_tomatoes = new List<Tomato>();
        private int nextFilledTomatoIndex;
        
        public override void Initialize(PomodoroTimer pomodoroTimer, bool updateColors = true)
        {
            // TODO: (4) Setting property for what defines a long break
            foreach (Tomato tomato in m_tomatoes)
            {
                tomato.Initialize(pomodoroTimer);
            }
            
            base.Initialize(pomodoroTimer, updateColors);
            
            nextFilledTomatoIndex = 0;
            DetermineTrashcanVisibility();
        }
        
        private void DetermineTrashcanVisibility()
        {
            // Only show if user has more than one tomato or has unlocked long break
            m_trashcan.gameObject.SetActive(nextFilledTomatoIndex > 0 || Timer.IsOnLongBreak());
        }
        
        /// <summary>
        /// Fills in the latest tomato
        /// </summary>
        /// <returns>Are we ready for a long break?</returns>
        public void FillTomato()
        {
            // If the user has already unlocked the long break...
            if (Timer.IsOnLongBreak())
            {
                return;
            }
            
            m_tomatoes[nextFilledTomatoIndex].Complete();

            // Increment / wrap new tomato index
            nextFilledTomatoIndex++;
            nextFilledTomatoIndex = ListHelper.Wrap(nextFilledTomatoIndex, m_tomatoes.Count);
            
            // Check for completion
            if (nextFilledTomatoIndex == 0)
            {
                Timer.ActivateLongBreak();
            }
            
            DetermineTrashcanVisibility();
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

            nextFilledTomatoIndex = 0;
            DetermineTrashcanVisibility();
        }

        // Unity Event - Trashcan
        public void TrashTomatoes()
        {
            Timer.SpawnConfirmationDialog(() =>
            {
                if (Timer.IsOnLongBreak())
                {
                    // Reset view to regular break
                    Timer.DeactivateLongBreak();
                    Timer.TrySwitchToBreakTimer();
                }
                else
                {
                    Timer.DeactivateLongBreak();
                }

                ConsumeTomatoes();
            }, null, "This action will delete your pomodoro/tomato progress.");
        }
    }
}
