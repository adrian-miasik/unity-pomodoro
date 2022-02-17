using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AdrianMiasik.Components.Core
{
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
