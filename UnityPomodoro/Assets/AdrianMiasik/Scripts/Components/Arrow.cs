using UnityEngine;

namespace AdrianMiasik.Components
{
    public class Arrow : MonoBehaviour
    {
        [SerializeField] private ClickButton button;
        [SerializeField] private new Animation animation;
        
        public void Hide()
        {
            Release();

            button.raycastTarget = false;
            gameObject.SetActive(false);
        }

        public void Show()
        {
            button.raycastTarget = true;
            gameObject.SetActive(true);
        }

        public void Hold()
        {
            button.OnPointerDown(null);
        }

        public void Release()
        {
            if (button.IsHolding())
            {
                button.CancelHold();
            }
        }
    }
}