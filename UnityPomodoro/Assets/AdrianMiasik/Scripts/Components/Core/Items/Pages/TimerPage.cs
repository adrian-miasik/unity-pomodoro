using System;
using System.Collections.Generic;
using AdrianMiasik.Components.Base;
using AdrianMiasik.Components.Core.Containers;
using AdrianMiasik.ScriptableObjects;
using LeTai.TrueShadow;
using UnityEngine;

namespace AdrianMiasik.Components.Core.Items.Pages
{
    /// <summary>
    /// Our main timer <see cref="Page"/> that is used to show/hide the digit and timer content.
    /// </summary>
    public class TimerPage: Page
    {
        [SerializeField] private DigitFormat m_format;

        [SerializeField] private List<TrueShadow> m_trueShadows = new();

        public override void Show(Action onAnimationCompletion)
        {
            base.Show(onAnimationCompletion);

            foreach (TrueShadow shadow in m_trueShadows)
            {
                shadow.IgnoreCasterColor = true;
            }
        }

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