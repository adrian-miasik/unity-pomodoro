using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AdrianMiasik.Components.Core
{
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