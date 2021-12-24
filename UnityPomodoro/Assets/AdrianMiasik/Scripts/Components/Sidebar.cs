using System.Collections.Generic;
using AdrianMiasik.Components.Core;
using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace AdrianMiasik.Components
{ 
    [ExecuteInEditMode]
    public class Sidebar : MonoBehaviour, IColorHook
    {
        [Header("Components")]
        [SerializeField] private BooleanToggle m_menuToggle;
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
        private PomodoroTimer timer;
        private bool isOpen;
        private Color overlay;
        
        private int screenWidth;
        private int screenHeight;
        
        private float rowStaggerDelay = 0.15f;
        private float rowStaggerTime;

        public void Initialize(PomodoroTimer pomodoroTimer)
        {
            timer = pomodoroTimer;
            pomodoroTimer.GetTheme().RegisterColorHook(this);
            ColorUpdate(pomodoroTimer.GetTheme());
            
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
            CalculateSidebarWidth();
        }

        private void Update()
        {
            if (Screen.height != screenHeight || Screen.width != screenWidth)
            {
                screenWidth = Screen.width;
                screenHeight = Screen.height;
                CalculateSidebarWidth();
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
        
        private void CalculateSidebarWidth()
        {
            float scalar = (float)Screen.height / Screen.width;
            m_background.anchorMax = new Vector2(Mathf.Clamp(0.5f * scalar,0,0.75f), m_background.anchorMax.y);
        }
        
        public void Open()
        {
            m_rowsToSpawn = new List<SidebarRow>(m_contentRows);
            foreach (SidebarRow row in m_contentRows)
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
            
            ColorUpdate(timer.GetTheme());
            timer.ColorUpdateCreditsBubble();
        }

        public void Close()
        {
            isOpen = false;

            // Cancel holds in-case user holds button down and closes our menu prematurely
            foreach (SidebarRow row in m_contentRows)
            {
                row.CancelHold();
            }

            m_menuToggle.SetToFalse();
            
            m_container.gameObject.SetActive(false);
            m_entryAnimation.Stop();
            gameObject.SetActive(false);
            m_overlayImage.enabled = false;
            m_overlayGroup.alpha = 0;
            
            timer.ColorUpdateCreditsBubble();
        }

        public bool IsOpen()
        {
            return isOpen;
        }
        
        public void SelectRow(SidebarRow rowToSelect, AudioClip clickSoundClip)
        {
            // Deselect other rows
            foreach (SidebarRow row in m_contentRows)
            {
                if (row == rowToSelect)
                    continue;
                
                row.Deselect();
            }

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

        public void ColorUpdate(Theme theme)
        {
            // Overlay
            overlay = theme.GetCurrentColorScheme().m_foreground;
            overlay.a = theme.m_isLightModeOn ? 0.5f : 0.025f;
            m_overlayImage.color = overlay;

            // Background
            m_fill.color = theme.GetCurrentColorScheme().m_background;
            m_edge.color = theme.GetCurrentColorScheme().m_background;
            
            // Text
            m_versionNumber.color = theme.GetCurrentColorScheme().m_foreground;
        }
    }
}
