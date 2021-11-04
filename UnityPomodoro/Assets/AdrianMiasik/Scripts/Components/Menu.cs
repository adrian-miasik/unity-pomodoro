using UnityEngine;
using UnityEngine.UI;

namespace AdrianMiasik.Components
{
    public class Menu : MonoBehaviour
    {
        [SerializeField] private Image background;
        [SerializeField] private RectTransform container;
        
        public void Show()
        {
            background.enabled = true;
            container.gameObject.SetActive(true);
        }

        public void Hide()
        {
            background.enabled = false;
            container.gameObject.SetActive(false);
        }
    }
}
