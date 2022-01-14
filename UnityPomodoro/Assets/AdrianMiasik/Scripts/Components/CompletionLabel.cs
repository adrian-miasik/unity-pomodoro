using System.Collections.Generic;
using AdrianMiasik.Components.Core;
using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
using TMPro;
using UnityEngine;

namespace AdrianMiasik.Components
{
    /// <summary>
    /// A <see cref="ThemeElement"/> used primarily to update the color of the referenced
    /// "TIMER COMPLETE" labels via <see cref="IColorHook"/> ColorUpdate.
    /// </summary>
    public class CompletionLabel : ThemeElement
    {
        [SerializeField] private List<TMP_Text> m_labels = new List<TMP_Text>();
        
        /// <summary>
        /// Applies our <see cref="Theme"/> changes to our referenced components when necessary.
        /// </summary>
        /// <param name="theme">The theme to apply on our referenced components.</param>
        public override void ColorUpdate(Theme theme)
        {
            foreach (TMP_Text text in m_labels)
            {
                text.color = theme.GetCurrentColorScheme().m_complete;
            }
        }
    }
}
