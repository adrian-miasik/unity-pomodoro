using AdrianMiasik.Components.Base;
using AdrianMiasik.Components.Core.Containers;
using AdrianMiasik.ScriptableObjects;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace AdrianMiasik.Components.Core.Items
{
    /// <summary>
    /// A <see cref="ThemeElement"/> used to display an icon, and a text label. Includes <see cref="ClickButton"/>,
    /// and a spawn animation. Intended to be used by <see cref="sidebar"/>.
    /// </summary>
    public class SidebarRow : ThemeElement
    {
        [SerializeField] private Animation m_spawn;
        [SerializeField] private RectTransform m_container;
        [SerializeField] private ClickButton m_button;
        [SerializeField] private Image m_accent;
        [SerializeField] private RectTransform m_contentContainer;
        [SerializeField] private Image m_background;
        [SerializeField] private SVGImage m_icon;
        [SerializeField] private SVGImage m_iconBackground;
        [SerializeField] private TMP_Text m_label;

        // Cache
        private Sidebar sidebar;
        private bool isSelected;
        
        public void Initialize(PomodoroTimer pomodoroTimer, Sidebar parentSidebar, bool selected = false)
        {
            base.Initialize(pomodoroTimer);
            sidebar = parentSidebar;
            isSelected = selected;
        }

        public void Hide()
        {
            m_container.gameObject.SetActive(false);
        }
        
        public void PlaySpawnAnimation()
        {
            Show();
            m_spawn.Play();
        }

        public void Show()
        {
            m_container.gameObject.SetActive(true);
        }

        // UnityEvent
        public void OnClick()
        {
            sidebar.SelectRow(this, m_button.m_clickSound.clip);
        }
        
        [ContextMenu("Select")]
        public void Select()
        {
            // Set width of accent
            m_accent.rectTransform.sizeDelta = new Vector2(6f, m_accent.rectTransform.sizeDelta.y);
            
            // Move content aside
            m_contentContainer.offsetMin = new Vector2(6, m_contentContainer.offsetMin.y); // Left
            m_contentContainer.offsetMax = new Vector2(-6, m_contentContainer.offsetMax.y); // Right

            m_background.color = Timer.GetTheme().GetCurrentColorScheme().m_backgroundHighlight;
            
            isSelected = true;
        }

        [ContextMenu("Deselect")]
        public void Deselect()
        {
            // Remove accent
            m_accent.rectTransform.sizeDelta = new Vector2(0, m_accent.rectTransform.sizeDelta.y);
            
            // Move content back to original location
            m_contentContainer.offsetMin = Vector2.zero; // Left
            m_contentContainer.offsetMax = Vector2.zero; // Right
            
            m_background.color = Timer.GetTheme().GetCurrentColorScheme().m_background;

            isSelected = false;
        }

        public void CancelHold()
        {
            m_button.CancelHold();
        }

        public override void ColorUpdate(Theme theme)
        {
            // Backgrounds
            m_background.color = isSelected ? theme.GetCurrentColorScheme().m_backgroundHighlight : theme.GetCurrentColorScheme().m_background;
            m_iconBackground.color = theme.GetCurrentColorScheme().m_background;
            
            // Foreground
            m_icon.color = theme.GetCurrentColorScheme().m_foreground;
            m_label.color = theme.GetCurrentColorScheme().m_foreground;
        }

        public bool IsSelected()
        {
            return isSelected;
        }
    }
}
