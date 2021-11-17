using System;
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
        [SerializeField] private BooleanToggle menuToggle;
        [SerializeField] private RectTransform container;
        [SerializeField] private RectTransform background;
        [SerializeField] private Image overlayImage;
        [SerializeField] private CanvasGroup overlayGroup;
        [SerializeField] private Animation entryAnimation;
        [SerializeField] private SVGImage fill;
        [SerializeField] private SVGImage edge;
        [SerializeField] private TMP_Text versionNumber;
        
        // Components
        [Header("Sidebar Rows (Content)")]
        [SerializeField] private List<SidebarRow> contentRows = new List<SidebarRow>();
        [SerializeField] private List<SidebarRow> rowsToSpawn;

        // Cache
        private PomodoroTimer timer;
        private bool isOpen;
        private Color overlay;
        
        private int screenWidth;
        private int screenHeight;
        
        private float rowStaggerDelay = 0.15f;
        private float rowStaggerTime;

        public void Initialize(PomodoroTimer _timer)
        {
            timer = _timer;
            _timer.GetTheme().RegisterColorHook(this);
            ColorUpdate(_timer.GetTheme());
            
            // Initialize row components
            for (int _i = 0; _i < contentRows.Count; _i++)
            {
                SidebarRow _row = contentRows[_i];

                // Only select first item
                if (_i == 0)
                {
                    _row.Initialize(_timer, this, true);
                }
                else
                {
                    _row.Initialize(_timer, this);
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
                if (rowsToSpawn.Count > 0)
                {
                    rowStaggerTime += Time.deltaTime;
                    
                    if (rowStaggerTime >= rowStaggerDelay)
                    {
                        rowStaggerTime = 0;
                        rowsToSpawn[0].PlaySpawnAnimation();
                        rowsToSpawn.RemoveAt(0);
                    }
                }
            }
        }
        
        private void CalculateSidebarWidth()
        {
            float _scalar = (float)Screen.height / Screen.width;
            background.anchorMax = new Vector2(Mathf.Clamp(0.5f * _scalar,0,0.75f), background.anchorMax.y);
        }
        
        public void Open()
        {
            rowsToSpawn = new List<SidebarRow>(contentRows);
            rowStaggerTime = rowStaggerDelay; // First stagger has no delay
            foreach (SidebarRow _row in contentRows)
            {
                _row.Hide();
            }

            isOpen = true;
            
            container.gameObject.SetActive(true);
            gameObject.SetActive(true);
            entryAnimation.Play();
            overlayImage.enabled = true;
            overlayGroup.alpha = 1;
            
            ColorUpdate(timer.GetTheme());
            timer.ColorUpdateCreditsBubble();
        }

        public void Close()
        {
            isOpen = false;

            // Cancel holds in-case user holds button down and closes our menu prematurely
            foreach (SidebarRow _row in contentRows)
            {
                _row.CancelHold();
            }

            menuToggle.SetToFalse();
            
            container.gameObject.SetActive(false);
            entryAnimation.Stop();
            gameObject.SetActive(false);
            overlayImage.enabled = false;
            overlayGroup.alpha = 0;
            
            timer.ColorUpdateCreditsBubble();
        }

        public bool IsOpen()
        {
            return isOpen;
        }
        
        public void SelectRow(SidebarRow _rowToSelect)
        {
            // Deselect other rows
            foreach (SidebarRow _row in contentRows)
            {
                if (_row == _rowToSelect)
                    continue;
                
                _row.Deselect();
            }

            if (!_rowToSelect.IsSelected())
            {
                _rowToSelect.Select();
                
                // Close sidebar after selection
                Close();
            }
            else
            {
                Debug.LogWarning("This sidebar row is already selected!", _rowToSelect.gameObject);
            }
        }

        public void ColorUpdate(Theme _theme)
        {
            // Overlay
            overlay = _theme.GetCurrentColorScheme().foreground;
            overlay.a = _theme.isLightModeOn ? 0.5f : 0.025f;
            overlayImage.color = overlay;

            // Background
            fill.color = _theme.GetCurrentColorScheme().background;
            edge.color = _theme.GetCurrentColorScheme().background;
            
            // Text
            versionNumber.color = _theme.GetCurrentColorScheme().foreground;
        }
    }
}
