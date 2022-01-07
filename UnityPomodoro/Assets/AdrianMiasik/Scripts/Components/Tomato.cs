using AdrianMiasik.Components.Core;
using AdrianMiasik.ScriptableObjects;
using Unity.VectorGraphics;
using UnityEngine;

namespace AdrianMiasik.Components
{
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
        
        public override void Initialize(PomodoroTimer pomodoroTimer, bool updateColors = true)
        {
            base.Initialize(pomodoroTimer, updateColors);
        }

        public override void ColorUpdate(Theme theme)
        {
            base.ColorUpdate(theme);
        }

        [ContextMenu("Complete")]
        public void Complete()
        {
            m_body.sprite = m_croppedBody;
        }

        [ContextMenu("Reset")]
        public void Reset()
        {
            m_body.sprite = m_hollowCroppedBody;
        }
    }
}
