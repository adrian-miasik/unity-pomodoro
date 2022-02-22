using System.Collections.Generic;
using AdrianMiasik.Components.Base;
using AdrianMiasik.ScriptableObjects;
using UnityEngine;

namespace AdrianMiasik.Components.Core.Containers
{
    /// <summary>
    /// A container that manages a list of <see cref="ThemeIcon"/>'s and updates their color's based on the
    /// current <see cref="Theme"/>.
    /// </summary>
    public class ThemeIconContainer : ThemeElement
    {
        [SerializeField] private List<ThemeIcon> m_icons = new List<ThemeIcon>();

        public override void Initialize(PomodoroTimer pomodoroTimer, bool updateColors = true)
        {
            base.Initialize(pomodoroTimer, updateColors);

            foreach (ThemeIcon icon in m_icons)
            {
                icon.Initialize(pomodoroTimer, updateColors);
            }
        }

        /// <summary>
        /// Applies our <see cref="Theme"/> changes to our referenced components when necessary.
        /// </summary>
        /// <param name="theme">The theme to apply on our referenced components.</param>
        public void ColorUpdate(Theme theme)
        {
            foreach (ThemeIcon icon in m_icons)
            {
                icon.ColorUpdate(theme);
            }
        }
    }
}