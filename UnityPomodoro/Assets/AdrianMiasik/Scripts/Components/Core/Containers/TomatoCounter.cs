using System.Collections.Generic;
using AdrianMiasik.Components.Base;
using AdrianMiasik.Components.Core.Items;
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
        [SerializeField] private ClickButton m_trashcan;
        [SerializeField] private List<Tomato> m_uncompletedTomatoes = new List<Tomato>();
        [SerializeField] private Tomato m_tomatoPrefab;

        private List<Tomato> completedTomatoes = new List<Tomato>();
 
        /// <summary>
        /// Setups up our tomatoes and determines trashcan visibility (based on progression).
        /// </summary>
        /// <param name="pomodoroTimer"></param>
        /// <param name="updateColors"></param>
        public override void Initialize(PomodoroTimer pomodoroTimer, bool updateColors = true)
        {
            foreach (Tomato tomato in m_uncompletedTomatoes)
            {
                tomato.Initialize(pomodoroTimer, updateColors);
            }
            
            base.Initialize(pomodoroTimer, updateColors);
            
            m_trashcan.m_onClick.AddListener(TrashTomatoes);
            
            completedTomatoes.Clear();
            DetermineTrashcanVisibility();
        }
        
        private void DetermineTrashcanVisibility()
        {
            // Only show if user has more than one tomato
            m_trashcan.gameObject.SetActive(completedTomatoes.Count > 0);
        }
        
        /// <summary>
        /// Completes / Fills in the latest <see cref="Tomato"/>. (from left to right)
        /// </summary>
        public void FillTomato()
        {
            Tomato tomatoToFill = m_uncompletedTomatoes[0];
            m_uncompletedTomatoes.RemoveAt(0);
            completedTomatoes.Add(tomatoToFill);
            tomatoToFill.Complete();
            
            // Check for completion
            if (m_uncompletedTomatoes.Count == 0)
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
            // Move completed tomatoes back into the uncompleted tomatoes list in the correct order
            for (int i = completedTomatoes.Count; i > 0; i--)
            {
                m_uncompletedTomatoes.Insert(0, completedTomatoes[i - 1]);
            }
            
            foreach (Tomato tomato in m_uncompletedTomatoes)
            {
                tomato.Reset();
            }
            
            completedTomatoes.Clear();
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
            Timer.GetConfirmDialogManager().SpawnConfirmationDialog(() =>
            {
                Timer.DeactivateLongBreak();
                Timer.IfSetupTriggerRebuild();
                ConsumeTomatoes();
            }, null, "This action will delete your pomodoro/tomato progress.");
        }

        public bool HasProgression()
        {
            return completedTomatoes.Count > 0;
        }

        public void SetPomodoroCount(int desiredPomodoroCount, int pomodoroProgress)
        {
            // Preserve trashcan, it lives on the last tomato.
            m_trashcan.transform.SetParent(m_horizontal.transform);

            // Dispose of tomatoes
            foreach (Tomato t in m_uncompletedTomatoes)
            {
                Destroy(t.gameObject);
            }
            m_uncompletedTomatoes.Clear();
            foreach (Tomato t in completedTomatoes)
            {
                Destroy(t.gameObject);
            }
            completedTomatoes.Clear();

            // Create new tomatoes
            for (int i = 0; i < desiredPomodoroCount; i++)
            {
                m_uncompletedTomatoes.Add(Instantiate(m_tomatoPrefab, m_horizontal.transform));
            }
            
            // Re-attach trashcan
            m_trashcan.transform.SetParent(m_uncompletedTomatoes[m_uncompletedTomatoes.Count - 1].transform);

            // Re-init
            Initialize(Timer);

            for (int i = 0; i < pomodoroProgress; i++)
            {
                FillTomato();
            }
        }

        public int GetTomatoProgress()
        {
            return completedTomatoes.Count;
        }

        public int GetTomatoCount()
        {
            return completedTomatoes.Count + m_uncompletedTomatoes.Count;
        }
    }
}