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
        [SerializeField] private Animation spawn;
        [SerializeField] private RectTransform container;
        [SerializeField] private ClickButton button;
        [SerializeField] private Image accent;
        [SerializeField] private RectTransform contentContainer;
        [SerializeField] private Image background;
        [SerializeField] private SVGImage icon;
        [SerializeField] private SVGImage iconBackground;
        [SerializeField] private TMP_Text label;

        // Cache
        private PomodoroTimer timer;
        private Sidebar sidebar;
        private bool isSelected;
        
        public void Initialize(PomodoroTimer _timer, Sidebar _sidebar, bool _isSelected = false)
        {
            timer = _timer;
            sidebar = _sidebar;
            isSelected = _isSelected;
            _timer.GetTheme().RegisterColorHook(this);
            ColorUpdate(_timer.GetTheme());
            Hide();
        }

        public void Hide()
        {
            container.gameObject.SetActive(false);
        }
        
        public void PlaySpawnAnimation()
        {
            Show();
            spawn.Stop();
            spawn.Play();
        }

        private void Show()
        {
            container.gameObject.SetActive(true);
        }

        // UnityEvent
        public void OnClick()
        {
            sidebar.SelectRow(this, button.clickSound.clip);
        }
        
        [ContextMenu("Select")]
        public void Select()
        {
            // Set width of accent
            accent.rectTransform.sizeDelta = new Vector2(6f, accent.rectTransform.sizeDelta.y);
            
            // Move content aside
            contentContainer.offsetMin = new Vector2(6, contentContainer.offsetMin.y); // Left
            contentContainer.offsetMax = new Vector2(-6, contentContainer.offsetMax.y); // Right

            background.color = timer.GetTheme().GetCurrentColorScheme().backgroundHighlight;
            
            isSelected = true;
        }

        [ContextMenu("Deselect")]
        public void Deselect()
        {
            // Remove accent
            accent.rectTransform.sizeDelta = new Vector2(0, accent.rectTransform.sizeDelta.y);
            
            // Move content back to original location
            contentContainer.offsetMin = Vector2.zero; // Left
            contentContainer.offsetMax = Vector2.zero; // Right
            
            background.color = timer.GetTheme().GetCurrentColorScheme().background;

            isSelected = false;
        }

        public void CancelHold()
        {
            button.CancelHold();
        }

        public void ColorUpdate(Theme _theme)
        {
            // Backgrounds
            background.color = isSelected ? _theme.GetCurrentColorScheme().backgroundHighlight : _theme.GetCurrentColorScheme().background;
            iconBackground.color = _theme.GetCurrentColorScheme().background;
            
            // Foreground
            icon.color = _theme.GetCurrentColorScheme().foreground;
            label.color = _theme.GetCurrentColorScheme().foreground;
        }

        public bool IsSelected()
        {
            return isSelected;
        }
    }
}
