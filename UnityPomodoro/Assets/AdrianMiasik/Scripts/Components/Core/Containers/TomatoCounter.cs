using System.Collections.Generic;
using AdrianMiasik.Components.Base;
using AdrianMiasik.Components.Core.Helpers;
using AdrianMiasik.Components.Core.Items;
using AdrianMiasik.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace AdrianMiasik.Components.Core.Containers
{
    /// <summary>
    /// A <see cref="ThemeElement"/> horizontal layout container used to manage our <see cref="Tomato"/>es/Pomodoros and
    /// manipulates their state as a group. Such as completing/filling each sequential <see cref="Tomato"/> in.
    /// Includes a trashcan that is used to wipe tomato/pomodoro progression. Intended to be used to determine when
    /// to take long breaks.
    /// </summary>
    public class TomatoCounter : ThemeElement
    {
        [SerializeField] private HorizontalLayoutGroup m_horizontal;
        [SerializeField] private GameObject m_trashcan;
            
        [SerializeField] private List<Tomato> m_tomatoes = new List<Tomato>();
        private int nextFilledTomatoIndex;
        
        /// <summary>
        /// Setups up our tomatoes and determines trashcan visibility (based on progression).
        /// </summary>
        /// <param name="pomodoroTimer"></param>
        /// <param name="updateColors"></param>
        public override void Initialize(PomodoroTimer pomodoroTimer, bool updateColors = true)
        {
            // TODO: (4) Setting property for what defines a long break
            foreach (Tomato tomato in m_tomatoes)
            {
                tomato.Initialize(pomodoroTimer, updateColors);
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
        /// Completes / Fills in the latest <see cref="Tomato"/>. (from left to right)
        /// </summary>
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
            nextFilledTomatoIndex = CollectionHelper.Wrap(nextFilledTomatoIndex, m_tomatoes.Count);
            
            // Check for completion
            if (nextFilledTomatoIndex == 0)
            {
                Timer.ActivateLongBreak();
            }
            
            DetermineTrashcanVisibility();
        }

        /// <summary>
        /// Sets the scale of this horizontal layout group.
        /// Intended for animations.
        /// </summary>
        /// <param name="newScale"></param>
        public void SetHorizontalScale(Vector3 newScale)
        {
            m_horizontal.transform.localScale = newScale;
        }
        
        /// <summary>
        /// Wipe / Clears your completed <see cref="Tomato"/>/pomodoro progression back to zero.
        /// </summary>
        public void ConsumeTomatoes()
        {
            foreach (Tomato tomato in m_tomatoes)
            {
                tomato.Reset();
            }

            nextFilledTomatoIndex = 0;
            DetermineTrashcanVisibility();
        }

        /// <summary>
        /// Attempts to destroy our <see cref="Tomato"/>/pomodoro progression. Accounts for long break mode.
        /// Will prompt user with a <see cref="ConfirmationDialog"/> to confirm their action to prevent accidental
        /// wipes / clears.
        /// <remarks>UnityEvent - Attached to trashcan gameobject.</remarks>
        /// </summary>
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
