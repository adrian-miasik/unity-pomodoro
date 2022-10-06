using System.Collections.Generic;
using AdrianMiasik.Components.Base;
using AdrianMiasik.Components.Core.Helpers;
using AdrianMiasik.Components.Core.Items;
using AdrianMiasik.Components.Specific;
using AdrianMiasik.ScriptableObjects;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;

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
        [SerializeField] private ClickButtonImageIcon m_logo;
        [SerializeField] private RectTransform m_container;
        [SerializeField] private RectTransform m_background;
        [SerializeField] private RectTransform m_maskBG;
        [SerializeField] private RectTransform m_header;
        [SerializeField] private CanvasGroup m_canvasGroup;
        [SerializeField] private SVGImage m_fill;
        [SerializeField] private SVGImage m_edge;
        [SerializeField] private TMP_Text m_versionNumber;
        [SerializeField] private List<SVGImage> m_externals;

        [Header("Animations")] 
        [SerializeField] private Animation m_animation;
        [SerializeField] private AnimationClip m_entryAnimation;
        [SerializeField] private AnimationClip m_exitAnimation;

        [Header("Sidebar Rows - Content")] 
        [SerializeField] private SidebarRow m_pomodoroTimerRow;
        [SerializeField] private SidebarRow m_settingsRow;
        [SerializeField] private SidebarRow m_documentationRow;
        [SerializeField] private SidebarRow m_aboutRow;
        
        // Components
        [Header("Sidebar Rows (Content)")] 
        private List<SidebarRow> contentRows;
        private List<SidebarRow> rowsToSpawn;

        // Cache
        private bool isOpen;

        [SerializeField] private float m_rowStaggerDelay = 0.0725f;
        private float rowStaggerTime;
        
        private bool isCalculatingText;

        /// <summary>
        /// Sets up our <see cref="SidebarRow"/>'s and selects the first one, also calculates and determines
        /// the sidebar width based on screen values.
        /// </summary>
        /// <param name="pomodoroTimer"></param>
        /// <param name="resolutionDetector"></param>
        public void Initialize(PomodoroTimer pomodoroTimer, ResolutionDetector resolutionDetector)
        {
            base.Initialize(pomodoroTimer);

            // When resolution changes...
            resolutionDetector.onResolutionChange.AddListener(() =>
            {
                // Resize sidebar width and conform credits bubble to it's width.
                Timer.ConformCreditsBubbleToSidebar(CalculateSidebarWidth());
                
                // Re-calculate our content row texts to use the smallest font size.
                CalculateAndSetSidebarRowMaxFontSize();
            });

            // Fill content rows
            contentRows = new List<SidebarRow>
            {
                m_pomodoroTimerRow,
                m_documentationRow,
                m_aboutRow,
                m_settingsRow
            };
            
            // Initialize row components
            for (int i = 0; i < contentRows.Count; i++)
            {
                SidebarRow row = contentRows[i];

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
            
            m_pomodoroTimerRow.GetClickButton().m_onClick.AddListener(Timer.ShowMainContent);
            m_settingsRow.GetClickButton().m_onClick.AddListener(Timer.ShowSettings);
            m_documentationRow.GetClickButton().m_onClick.AddListener(() =>
            {
                m_documentationRow.GetClickButton().OpenURL("https://adrian-miasik.github.io/unity-pomodoro-docs/");
            });
            m_aboutRow.GetClickButton().m_onClick.AddListener(Timer.ShowAbout);
        }

        private void Update()
        {
            if (isOpen)
            {
                if (rowsToSpawn.Count > 0)
                {
                    rowStaggerTime += Time.deltaTime;
                    
                    if (rowStaggerTime >= m_rowStaggerDelay)
                    {
                        rowStaggerTime = 0;
                        rowsToSpawn[0].PlaySpawnAnimation();
                        rowsToSpawn.RemoveAt(0);
                    }
                }
            }

            if (m_animation.clip == m_exitAnimation)
            {
                if (!m_animation.isPlaying)
                {
                    // Exit animation complete
                    m_animation.clip = null;
                    gameObject.SetActive(false);
                }
            }
        }

        public float CalculateSidebarWidth()
        {
            float scalar = (float)Screen.height / Screen.width;
            float desiredWidth = Mathf.Clamp(0.5f * scalar, 0, 0.75f);
            
            m_background.anchorMax = new Vector2(desiredWidth, m_background.anchorMax.y);

            return desiredWidth;
        }
        
        /// <summary>
        /// Animate sidebar open and animate our <see cref="SidebarRow"/>'s in with a stagger,
        /// show overlay, and updates <see cref="CreditsGhost"/> and sidebar colors.
        /// </summary>
        public void Open()
        {
            Timer.ConformCreditsBubbleToSidebar(CalculateSidebarWidth());
            Timer.FadeCreditsBubble(true);
            
            rowsToSpawn = new List<SidebarRow>(contentRows);

            foreach (SidebarRow row in rowsToSpawn)
            {
                row.Hide();
            }
            
            isOpen = true;

            // Open sidebar instantly (by-passing the animation) so we can calculate the optimal font size to use.
            Show();
            // However, hide the opened sidebar visually from user.
            m_canvasGroup.alpha = 0;

            // Run the optimal font size calculation.
            CalculateAndSetSidebarRowMaxFontSize();
            
            // Now that we calculated our font size we close sidebar instantly (So we can animate in normally).
            Hide();
            // Show sidebar visually to user when we do animate it in normally.
            m_canvasGroup.alpha = 1;
            
            // Begin the sidebar open animation that the user can see.
            m_container.gameObject.SetActive(true);
            gameObject.SetActive(true);
            PlayAnimation(m_entryAnimation);
            Timer.ShowOverlay();

            // Theming
            ColorUpdate(Timer.GetTheme());
            Timer.ColorUpdateCreditsBubble();
        }

        /// <summary>
        /// Shows the sidebar instantly.
        /// </summary>
        private void Show()
        {
            m_container.gameObject.SetActive(true);
            gameObject.SetActive(true);
            m_maskBG.gameObject.SetActive(true);
            RectTransformHelper.StretchToFit(m_maskBG);
            m_header.gameObject.SetActive(true);
            m_edge.gameObject.SetActive(true);
            foreach (SidebarRow row in contentRows)
            {
                row.Show();
            }
        }

        /// <summary>
        /// Hides the sidebar instantly.
        /// </summary>
        private void Hide()
        {
            m_container.gameObject.SetActive(true);
            gameObject.SetActive(true);
            m_maskBG.gameObject.SetActive(false);
            m_header.gameObject.SetActive(false);
            m_edge.gameObject.SetActive(false);
            foreach (SidebarRow row in contentRows)
            {
                row.Hide();
            }
        }

        private void CalculateAndSetSidebarRowMaxFontSize()
        {
            // If sidebar is not open...
            if (!isOpen)
            {
                // Don't bother calculating
                return;
            }
            
            // Set to starting max font size and rebuild.
            foreach (SidebarRow row in contentRows)
            { 
                row.ResetMaxFontSize();
            }
            
            // Force a rebuild
            Canvas.ForceUpdateCanvases();
            
            // Print each font size after rebuild.
            // foreach (SidebarRow row in contentRows)
            // {
            //     Debug.Log(row.GetLabelFontSize(), row.gameObject);
            // }
            
            // Cache and find the lowest font size.
            float smallestFoundFontSize = Mathf.Infinity;
            foreach (SidebarRow row in contentRows)
            {
                float rowFontSize = row.GetLabelFontSize();
                if (rowFontSize < smallestFoundFontSize)
                {
                    smallestFoundFontSize = rowFontSize;
                }
            }

            // Debug.Log("Smallest found font size: " + smallestFoundFontSize);

            // Set/apply max font size.
            foreach (SidebarRow row in contentRows)
            {
                row.SetMaxFontSize(smallestFoundFontSize);
            }
        }

        /// <summary>
        /// Instantly closes our sidebar so it can no longer be seen by the user, hides overlay,
        /// updates <see cref="CreditsGhost"/>'s colors.
        /// </summary>
        public void Close()
        {
            Timer.ResetCreditsBubbleSidebarConformity();
            Timer.FadeCreditsBubble(false);

            isOpen = false;

            // Cancel holds in-case user holds button down and closes our menu prematurely
            foreach (SidebarRow row in contentRows)
            {
                row.CancelHold();
            }
            m_logo.CancelHold();

            m_menuToggleSprite.SetToFalse();
            
            PlayAnimation(m_exitAnimation);
            
            Timer.HideOverlay();

            // Theming
            Timer.ColorUpdateCreditsBubble();
            
            AudioMimic.Instance.PlaySound(m_logo.m_clickSound.clip);
        }
        
        private void PlayAnimation(AnimationClip animationToPlay)
        {
            m_animation.Stop();
            m_animation.clip = animationToPlay;
            m_animation.Play();
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
            if (!rowToSelect.IsSelectable())
            {
                return;
            }
            
            // Deselect other rows
            foreach (SidebarRow row in contentRows)
            {
                if (row == rowToSelect)
                    continue;
                
                row.Deselect();
            }

            // Select self, if not selected.
            if (!rowToSelect.IsSelected())
            {
                rowToSelect.Select();
            }
            else
            {
                Debug.LogWarning("This sidebar row is already selected!", rowToSelect.gameObject);
            }
            
            Close();
            
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
            // Background
            m_fill.color = theme.GetCurrentColorScheme().m_background;
            m_edge.color = theme.GetCurrentColorScheme().m_background;
            
            // Text
            m_versionNumber.color = theme.GetCurrentColorScheme().m_foreground;
            
            // External icons
            foreach (SVGImage external in m_externals)
            {
                external.color = theme.GetCurrentColorScheme().m_foreground;
            }
        }
    }
}