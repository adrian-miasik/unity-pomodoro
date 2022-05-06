using AdrianMiasik.Components.Base;
using AdrianMiasik.Components.Core.Containers;
using AdrianMiasik.ScriptableObjects;
using UnityEngine;

namespace AdrianMiasik.Components.Specific.Pages
{
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