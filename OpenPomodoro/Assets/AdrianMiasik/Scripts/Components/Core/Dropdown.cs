using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AdrianMiasik.Components.Core
{
    /// <summary>
    /// A dropdown component with support for sounds on interaction.
    /// </summary>
    public class Dropdown: TMP_Dropdown
    {
        [SerializeField] private AudioSource m_audio;

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            
            m_audio.Play();
        }

        protected override void DestroyBlocker(GameObject blocker)
        {
            base.DestroyBlocker(blocker);
            
            m_audio.Play();
        }
    }
}