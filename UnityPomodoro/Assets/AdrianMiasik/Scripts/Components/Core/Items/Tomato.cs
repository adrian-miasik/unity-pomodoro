using AdrianMiasik.Components.Base;
using AdrianMiasik.Components.Core.Containers;
using Unity.VectorGraphics;
using UnityEngine;

namespace AdrianMiasik.Components.Core.Items
{
    /// <summary>
    /// A <see cref="ThemeElement"/> image used to represent a tomato in two states: **Incomplete** (Hollow) or
    /// **Complete** (Filled-in).
    /// (Also see <see cref="TomatoCounter"/>)
    /// </summary>
    public class Tomato : ThemeElement
    {
        [Header("References")]
        [SerializeField] private SVGImage m_body;
        [SerializeField] private SVGImage m_stem;

        [Header("Assets")] 
        [Header("Hollow")] 
        [SerializeField] private Sprite m_hollowCroppedBody;
        [SerializeField] private Sprite m_hollowFullBody;

        [Header("Complete")]
        [SerializeField] private Sprite m_croppedBody;
        [SerializeField] private Sprite m_fullBody;

        [SerializeField] private Animation m_spawnTomato;

        public override void Initialize(PomodoroTimer pomodoroTimer, bool updateColors = true)
        {
            base.Initialize(pomodoroTimer, updateColors);
            Reset();
        }

        /// <summary>
        /// Completes this tomato by filling it in.
        /// </summary>
        [ContextMenu("Complete")]
        public void Complete()
        {
            m_body.sprite = m_croppedBody;
            m_spawnTomato.Stop();
            m_spawnTomato.Play();
        }

        /// <summary>
        /// In-completes this tomato by hollowing it out.
        /// </summary>
        [ContextMenu("Reset")]
        public void Reset()
        {
            m_body.sprite = m_hollowCroppedBody;
        }
    }
}