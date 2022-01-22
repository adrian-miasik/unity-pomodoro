using System.Collections.Generic;
using AdrianMiasik.Components.Base;
using AdrianMiasik.Components.Core.Items;
using AdrianMiasik.Components.Specific;
using AdrianMiasik.ScriptableObjects;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace AdrianMiasik.Components.Core.Containers
{ 
    /// <summary>
    /// A <see cref="ThemeElement"/> container page that holds <see cref="SidebarRow"/>'s and deals with group
    /// selections and animations.
    /// </summary>
    [ExecuteInEditMode]
    public class Sidebar : ThemeElement
    {
        [Header("Components")]
        [SerializeField] private ToggleSprite m_menuToggleSprite;
        [SerializeField] private RectTransform m_container;
        [SerializeField] private RectTransform m_background;
        [SerializeField] private Image m_overlayImage;
        [SerializeField] private CanvasGroup m_overlayGroup;
        [SerializeField] private Animation m_entryAnimation;
        [SerializeField] private SVGImage m_fill;
        [SerializeField] private SVGImage m_edge;
        [SerializeField] private TMP_Text m_versionNumber;
        
        // Components
        [Header("Sidebar Rows (Content)")]
        [SerializeField] private List<SidebarRow> m_contentRows = new List<SidebarRow>(); 
        [SerializeField] private List<SidebarRow> m_rowsToSpawn;

        // Cache
        private bool isOpen;
        private Color overlay;
        
        private int screenWidth;
        private int screenHeight;
        
        private float rowStaggerDelay = 0.15f;
        private float rowStaggerTime;

        /// <summary>
        /// Sets up our <see cref="SidebarRow"/>'s and selects the first one, also calculates and determines
        /// the sidebar width based on screen values.
        /// </summary>
        /// <param name="pomodoroTimer"></param>
        public void Initialize(PomodoroTimer pomodoroTimer)
        {
            base.Initialize(pomodoroTimer);
            
            // Initialize row components
            for (int i = 0; i < m_contentRows.Count; i++)
            {
                SidebarRow row = m_contentRows[i];

                // Only select first item
                if (i == 0)
                {
                    row.Initialize(pomodoroTimer, this, true);
                }
                else
                {
                    row.Initialize(pomodoroTimer, this);
                }
            }

            // Calculate screen dimensions
            screenWidth = Screen.width;
            screenHeight = Screen.height;
        }

        private void Update()
        {
            if (Screen.height != screenHeight || Screen.width != screenWidth)
            {
                screenWidth = Screen.width;
                screenHeight = Screen.height;
                
                // Set widths (credits bubble and sidebar)
                Timer.ConformCreditsBubbleToSidebar(CalculateSidebarWidth());
            }

            if (isOpen)
            {
                if (m_rowsToSpawn.Count > 0)
                {
                    rowStaggerTime += Time.deltaTime;
                    
                    if (rowStaggerTime >= rowStaggerDelay)
                    {
                        rowStaggerTime = 0;
                        m_rowsToSpawn[0].PlaySpawnAnimation();
                        m_rowsToSpawn.RemoveAt(0);
                    }
                }
            }
        }
        
        private float CalculateSidebarWidth()
        {
            float scalar = (float)Screen.height / Screen.width;
            float desiredWidth = Mathf.Clamp(0.5f * scalar, 0, 0.75f);
            
            m_background.anchorMax = new Vector2(desiredWidth, m_background.anchorMax.y);

            return desiredWidth;
        }
        
        /// <summary>
        /// Animate sidebar open and animate our <see cref="SidebarRow"/>'s in with a stagger,
        /// create overlay, and updates <see cref="CreditsBubble"/> colors.
        /// </summary>
        public void Open()
        {
            Timer.ConformCreditsBubbleToSidebar(CalculateSidebarWidth());
            Timer.FadeCreditsBubble(true);
            
            m_rowsToSpawn = new List<SidebarRow>(m_contentRows);

            foreach (SidebarRow row in m_rowsToSpawn)
            {
                row.Hide();
            }
            
            rowStaggerTime = rowStaggerDelay; // First stagger has no delay
            isOpen = true;
            
            m_container.gameObject.SetActive(true);
            gameObject.SetActive(true);
            m_entryAnimation.Play();
            m_overlayImage.enabled = true;
            m_overlayGroup.alpha = 1;

            // Theming
            ColorUpdate(Timer.GetTheme());
            Timer.ColorUpdateCreditsBubble();
        }

        /// <summary>
        /// Instantly closes our sidebar so it can no longer be seen by the user, hides overlay,
        /// updates <see cref="CreditsBubble"/>'s colors.
        /// </summary>
        public void Close()
        {
            if (!Timer.IsAboutPageOpen())
            {
                Timer.CloseOutCreditsBubble();
            }

            Timer.ResetCreditsBubbleSidebarConformity();
            Timer.FadeCreditsBubble(false);
            
            isOpen = false;

            // Cancel holds in-case user holds button down and closes our menu prematurely
            foreach (SidebarRow row in m_contentRows)
            {
                row.CancelHold();
            }

            m_menuToggleSprite.SetToFalse();

            m_container.gameObject.SetActive(false);
            m_entryAnimation.Stop();
            gameObject.SetActive(false);
            m_overlayImage.enabled = false;
            m_overlayGroup.alpha = 0;

            // Theming
            Timer.ColorUpdateCreditsBubble();
        }

        /// <summary>
        /// Is this <see cref="Sidebar"/> currently open and visible to the user?
        /// </summary>
        /// <returns></returns>
        public bool IsOpen()
        {
            return isOpen;
        }
        
        /// <summary>
        /// Deselects all other <see cref="SidebarRow"/>'s and highlights the provided sidebar row.
        /// </summary>
        /// <param name="rowToSelect"></param>
        /// <param name="clickSoundClip"></param>
        public void SelectRow(SidebarRow rowToSelect, AudioClip clickSoundClip)
        {
            // Deselect other rows
            foreach (SidebarRow row in m_contentRows)
            {
                if (row == rowToSelect)
                    continue;
                
                row.Deselect();
            }

            // Select self, if not selected.
            if (!rowToSelect.IsSelected())
            {
                rowToSelect.Select();
                
                // Close sidebar after selection
                Close();
            }
            else
            {
                Debug.LogWarning("This sidebar row is already selected!", rowToSelect.gameObject);
            }
            
            // Pass the buck of playing our close sound to the audio mimic,
            // since closing the sidebar will disable the audio sources we want to play from.
            // And audio sources can not be played when in a disabled state.
            // So instead we propagate the sound up to our singleton audio mimic class. 
            AudioMimic.Instance.PlaySound(clickSoundClip);
        }

        /// <summary>
        /// Applies our <see cref="Theme"/> changes to our referenced components when necessary.
        /// </summary>
        /// <param name="theme">The theme to apply on our referenced components.</param>
        public override void ColorUpdate(Theme theme)
        {
            // Overlay
            overlay = theme.GetCurrentColorScheme().m_foreground;
            overlay.a = theme.m_darkMode ? 0.025f : 0.5f;
            m_overlayImage.color = overlay;

            // Background
            m_fill.color = theme.GetCurrentColorScheme().m_background;
            m_edge.color = theme.GetCurrentColorScheme().m_background;
            
            // Text
            m_versionNumber.color = theme.GetCurrentColorScheme().m_foreground;
        }
    }
}