using AdrianMiasik.Components.Core;
using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
using TMPro;
using UnityEngine;

namespace AdrianMiasik.Components
{
    public class SettingsPanel : MonoBehaviour, IColorHook
    {
        [SerializeField] private TMP_Text title;
        [SerializeField] private LabelledDropdown digitFormatDropdown;

        private bool isOpen = false;
        private PomodoroTimer timer;

        public void Initialize(PomodoroTimer _timer)
        {
            timer = _timer;
            timer.GetTheme().RegisterColorHook(this);
            
            digitFormatDropdown.Initialize(timer);
        }

        public void UpdateDropdown()
        {
            digitFormatDropdown.UpdateDropdownValue();
        }
        
        public void ColorUpdate(Theme _theme)
        {
            // TODO: Check if settings page is open
            title.color = _theme.GetCurrentColorScheme().foreground;
        }

        public void Show()
        {
            gameObject.SetActive(true);
            digitFormatDropdown.UpdateDropdownValue();
            isOpen = true;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            isOpen = false;
        }

        public bool IsPageOpen()
        {
            return isOpen;
        }
    }
}
