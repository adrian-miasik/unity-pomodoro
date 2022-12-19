using AdrianMiasik.Components.Base;
using AdrianMiasik.ScriptableObjects;
using LeTai.Asset.TranslucentImage;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AdrianMiasik.Components.Core
{
    /// <summary>
    /// An overlay that will blur UI elements.
    /// <remarks>Note: Specific Canvas' is required for this Asset.</remarks>
    /// Used to currently blur background on sidebar focus and confirmation dialog pop-ups.
    /// </summary>
    public class BlurOverlay : ThemeElement, IPointerClickHandler
    {
        [SerializeField] private TranslucentImage m_image;
        [SerializeField] private GraphicRaycaster m_mainCanvasRaycaster; // Intended to prevent clicking on blurred elements

        public override void Initialize(PomodoroTimer pomodoroTimer, bool updateColors = true)
        {
            base.Initialize(pomodoroTimer, updateColors);

            m_image.source = pomodoroTimer.GetTranslucentImageSource();
        }

        public void Show()
        {
            m_image.enabled = true;
            m_mainCanvasRaycaster.enabled = false;
        }

        public void Hide()
        {
            m_image.enabled = false;
            m_mainCanvasRaycaster.enabled = true;
        }
    
        public override void ColorUpdate(Theme theme)
        {
            base.ColorUpdate(theme);
            
            m_image.color = theme.m_dark ? new Color(0.2f, 0.2f, 0.2f) : Color.grey;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (Timer.IsSidebarOpen())
            {
                Timer.CloseSidebar();
                Hide();
            }
        }
    }
}
