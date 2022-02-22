using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AdrianMiasik.Components.Core
{
    /// <summary>
    /// Colors the referenced image using the provided hover color when the user's pointer enters this component, and
    /// reset's the color as well on user's pointer exit.
    /// </summary>
    public class ImageHoverColorChange : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image m_image;
        [SerializeField] private Color m_hoverColor;
        
        private Color cachedColor;
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            cachedColor = m_image.color;
            m_image.color = m_hoverColor;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            m_image.color = cachedColor;
        }
    }
}
