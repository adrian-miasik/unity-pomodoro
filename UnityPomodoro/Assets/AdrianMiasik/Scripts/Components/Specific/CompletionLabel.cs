using System.Collections.Generic;
using AdrianMiasik.Components.Base;
using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
using TMPro;
using UnityEngine;

namespace AdrianMiasik.Components.Specific
{
    /// <summary>
    /// A <see cref="ThemeElement"/> used primarily to update the color of the referenced
    /// "TIMER COMPLETE" labels via <see cref="IColorHook"/> ColorUpdate. Also used for offsetting
    /// the labels depending on long break setting. <see cref="Settings"/>
    /// </summary>
    public class CompletionLabel : ThemeElement
    {
        [SerializeField] private RectTransform m_self;
        [SerializeField] private Vector4 m_anchorsWhenLongBreakOn; // anchor min, anchor max
        [SerializeField] private Vector4 m_anchorsWhenLongBreakOff; // anchor min, anchor max
        
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

        public void MoveAnchors(bool isItLongBreak)
        {
            if (isItLongBreak)
            {
                m_self.anchorMin = new Vector2(m_anchorsWhenLongBreakOn.x, m_anchorsWhenLongBreakOn.y);
                m_self.anchorMax = new Vector2(m_anchorsWhenLongBreakOn.z, m_anchorsWhenLongBreakOn.w);
            }
            else
            {
                m_self.anchorMin = new Vector2(m_anchorsWhenLongBreakOff.x, m_anchorsWhenLongBreakOff.y);
                m_self.anchorMax = new Vector2(m_anchorsWhenLongBreakOff.z, m_anchorsWhenLongBreakOff.w);
            }
        }
    }
}