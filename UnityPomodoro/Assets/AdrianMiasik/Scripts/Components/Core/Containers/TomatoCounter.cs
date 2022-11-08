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
    /// Includes a trashcan that is used to wipe tomato/pomodoro progression. This class in intended to be used to
    /// determine when to take long breaks.
    /// </summary>
    public class TomatoCounter : ThemeElement
    {
        [SerializeField] private HorizontalLayoutGroup m_horizontal;
        [SerializeField] private ClickButton m_trashcan;
        [SerializeField] private List<Tomato> m_uncompletedTomatoes = new List<Tomato>();
        [SerializeField] private Tomato m_tomatoPrefab;

        private List<Tomato> completedTomatoes = new List<Tomato>();

        public void Initialize(PomodoroTimer timer, int pomodoroCount = 4, bool updateColors = true)
        {
            // Register ThemeElement
            base.Initialize(timer, updateColors);
            
            // Hook up trashcan button functionality
            m_trashcan.m_onClick.AddListener(TrashTomatoes);
            
            PreserveTrash();

            // Wipe / Destroy scene pre-cache
            m_uncompletedTomatoes.AddRange(completedTomatoes);
            foreach (Tomato tomato in m_uncompletedTomatoes)
            {
                Destroy(tomato.gameObject);
            }
            completedTomatoes.Clear();
            m_uncompletedTomatoes.Clear();
                
            // Create new tomatoes
            CreateTomatoes(pomodoroCount, out m_uncompletedTomatoes);

            // Init new tomatoes
            foreach (Tomato tomato in m_uncompletedTomatoes)
            {
                tomato.Initialize(Timer);
            }
            
            ReimplementTrash();
            
            // Only show if user has more than one tomato
            m_trashcan.gameObject.SetActive(completedTomatoes.Count > 0);
        }
        
        /// <summary>
        /// Attempts to destroy our <see cref="Tomato"/>/pomodoro progression. Accounts for long break mode.
        /// Will prompt user with a <see cref="ConfirmationDialog"/> to confirm their action to prevent accidental
        /// wipes / clears.
        /// </summary>
        private void TrashTomatoes()
        {
            Timer.GetConfirmDialogManager().SpawnConfirmationDialog(() =>
            {
                Timer.DeactivateLongBreak();
                Timer.IfSetupTriggerRebuild();
                ConsumeTomatoes();
            }, null, "This action will delete your pomodoro/tomato progress.");
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

            Debug.Log("TODO: Steam Achievement Logic - First Tomato");
            // if (SteamManager.Initialized)
            // {
            //     // Fetch achievement status
            //     if (SteamUserStats.GetAchievement("ACH_ACQUIRE_FIRST_TOMATO", 
            //             out bool unlockedFirstTomatoAchievement))
            //     {
            //         // If tomato acquired achievement is NOT unlocked...
            //         if (!unlockedFirstTomatoAchievement)
            //         {
            //             // Unlock achievement
            //             SteamUserStats.SetAchievement("ACH_ACQUIRE_FIRST_TOMATO");
            //             SteamUserStats.StoreStats();
            //
            //             Debug.Log("Steam Achievement Unlocked! 'Yummy: Unlock your first pomodoro/tomato.'");
            //         }
            //     }
            // }
            
            // Check for completion
            if (m_uncompletedTomatoes.Count == 0)
            {
                Timer.ActivateLongBreak();
            }
            
            // Only show if user has more than one tomato
            m_trashcan.gameObject.SetActive(completedTomatoes.Count > 0);
        }

        /// <summary>
        /// Sets the scale of this horizontal layout group.
        /// <remarks>Intended for pulse animation.</remarks>
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
            
            // Only show if user has more than one tomato
            m_trashcan.gameObject.SetActive(completedTomatoes.Count > 0);
        }

        /// <summary>
        /// Preserves the trashcan object for later use.
        /// </summary>
        private void PreserveTrash()
        {
            // Preserve trashcan, it lives on the last tomato.
            m_trashcan.transform.SetParent(m_horizontal.transform);
        }

        /// <summary>
        /// Re-applies the trashcan to the last active created tomato.
        /// </summary>
        private void ReimplementTrash()
        {
            // Re-attach trashcan
            List<Tomato> allTomatoes = new List<Tomato>();
            allTomatoes.AddRange(completedTomatoes);
            allTomatoes.AddRange(m_uncompletedTomatoes);
            m_trashcan.transform.SetParent(allTomatoes[allTomatoes.Count - 1].transform);
            
            RectTransform trashRect = m_trashcan.rectTransform;
            trashRect.anchoredPosition = Vector2.zero;
            trashRect.offsetMax = new Vector2(trashRect.offsetMax.x,0);
            trashRect.offsetMin = new Vector2(trashRect.offsetMin.x,0);
            trashRect.anchoredPosition = new Vector2(8f, trashRect.anchoredPosition.y);
        }

        public bool HasProgression()
        {
            return completedTomatoes.Count > 0;
        }

        public void SetPomodoroCount(int desiredPomodoroCount, int pomodoroProgress)
        {
            PreserveTrash();
            
            // Reset tomatoes
            List<Tomato> allTomatoes = new List<Tomato>();
            allTomatoes.AddRange(completedTomatoes);
            allTomatoes.AddRange(m_uncompletedTomatoes);
            foreach (Tomato tomato in allTomatoes)
            {
                tomato.Reset();
            }
            completedTomatoes.Clear();
            m_uncompletedTomatoes.Clear();

            if (desiredPomodoroCount > allTomatoes.Count)
            {
                int tomatoesToAdd = desiredPomodoroCount - allTomatoes.Count;
                for (int i = 0; i < tomatoesToAdd; i++)
                {
                    Tomato tomato = Instantiate(m_tomatoPrefab, m_horizontal.transform);
                    tomato.Initialize(Timer);
                    allTomatoes.Add(tomato);
                }
            }
            else if (desiredPomodoroCount < allTomatoes.Count)
            {
                int tomatoesToRemove = allTomatoes.Count - desiredPomodoroCount;
                for (int i = allTomatoes.Count; tomatoesToRemove > 0; tomatoesToRemove--)
                {
                    Tomato tomato = allTomatoes[i - 1];
                    allTomatoes.Remove(tomato);
                    Destroy(tomato.gameObject);
                    i--;
                }
            }

            m_uncompletedTomatoes = allTomatoes;
            
            for (int i = 0; i < pomodoroProgress; i++)
            {
                FillTomato();
            }

            ReimplementTrash();
        }

        private void CreateTomatoes(int count, out List<Tomato> newTomatoes)
        {
            newTomatoes = new List<Tomato>();
            
            // Create new tomatoes
            for (int i = 0; i < count; i++)
            {
                newTomatoes.Add(Instantiate(m_tomatoPrefab, m_horizontal.transform));
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