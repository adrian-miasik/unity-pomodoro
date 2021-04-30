using UnityEngine;

namespace AdrianMiasik.Components
{
    public class Arrow : MonoBehaviour
    {
        [SerializeField] private ClickButton button;
        [SerializeField] private new Animation animation;
        
        public void Hide()
        {
            button.raycastTarget = false;
            gameObject.SetActive(false);
        }

        public void Show()
        {
            button.raycastTarget = true;
            gameObject.SetActive(true);
        }
    }
}