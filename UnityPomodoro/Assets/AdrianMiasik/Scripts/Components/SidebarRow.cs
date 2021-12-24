using AdrianMiasik.Components.Core;
using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace AdrianMiasik.Components
{
    public class SidebarRow : MonoBehaviour, IColorHook
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
        private PomodoroTimer timer;
        private Sidebar sidebar;
        private bool isSelected;
        
        public void Initialize(PomodoroTimer pomodoroTimer, Sidebar parentSidebar, bool selected = false)
        {
            timer = pomodoroTimer;
            sidebar = parentSidebar;
            isSelected = selected;
            pomodoroTimer.GetTheme().RegisterColorHook(this);
            ColorUpdate(pomodoroTimer.GetTheme());
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

        private void Show()
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

            m_background.color = timer.GetTheme().GetCurrentColorScheme().m_backgroundHighlight;
            
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
            
            m_background.color = timer.GetTheme().GetCurrentColorScheme().m_background;

            isSelected = false;
        }

        public void CancelHold()
        {
            m_button.CancelHold();
        }

        public void ColorUpdate(Theme theme)
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
