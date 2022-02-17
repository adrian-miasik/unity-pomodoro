using AdrianMiasik.Components.Base;
using AdrianMiasik.ScriptableObjects;
using LeTai.Asset.TranslucentImage;
using UnityEngine;

namespace AdrianMiasik.Components.Core
{
    /// <summary>
    /// An overlay that will blur the UI elements behind.
    /// <remarks>Two Canvas' are required for this Asset.</remarks>
    /// </summary>
    public class BlurOverlay : ThemeElement
    {
        [SerializeField] private TranslucentImage m_image;

        public override void Initialize(PomodoroTimer pomodoroTimer, bool updateColors = true)
        {
            base.Initialize(pomodoroTimer, updateColors);

            m_image.source = pomodoroTimer.GetTranslucentImageSource();
        }

        public void Show()
        {
            m_image.enabled = true;
        }

        public void Hide()
        {
            m_image.enabled = false;
        }
    
        public override void ColorUpdate(Theme theme)
        {
            base.ColorUpdate(theme);
            
            m_image.color = theme.m_dark ? new Color(0.2f, 0.2f, 0.2f) : Color.grey;
        }
    }
}
