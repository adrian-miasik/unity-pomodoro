using UnityEngine;

namespace AdrianMiasik.Components
{
    public class Arrow : MonoBehaviour
    {
        [SerializeField] private ClickButton button;
        [SerializeField] private Transform icon; // Toggles visibility
        [SerializeField] private new Animation animation;
        
        public void Hide()
        {
            Release();

            button.raycastTarget = false;
            icon.gameObject.SetActive(false);
        }

        public void Show()
        {
            button.raycastTarget = true;
            icon.gameObject.SetActive(true);
        }

        public void Hold()
        {
            button.Hold();
        }

        public void Release()
        {
            if (button.IsHolding())
            {
                button.CancelHold();
            }
        }

        public void Click()
        {
            button.OnPointerClick(null);
        }
    }
}