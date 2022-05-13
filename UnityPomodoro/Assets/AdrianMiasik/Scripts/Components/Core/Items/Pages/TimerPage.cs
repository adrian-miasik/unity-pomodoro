using AdrianMiasik.Components.Base;
using AdrianMiasik.Components.Core.Containers;
using AdrianMiasik.ScriptableObjects;
using UnityEngine;

namespace AdrianMiasik.Components.Core.Items.Pages
{
    /// <summary>
    /// Our main timer <see cref="Page"/> that is used to show/hide the digit and timer content.
    /// </summary>
    public class TimerPage: Page
    {
        [SerializeField] private DigitFormat m_format;

        public override void Refresh()
        {
            base.Refresh();
            m_format.RefreshDigitVisuals();
        }

        public override void ColorUpdate(Theme theme)
        {
            // No title
        }
    }
}