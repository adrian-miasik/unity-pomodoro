using UnityEngine;
using UnityEngine.UI;

namespace AdrianMiasik.Components
{
    public class Menu : MonoBehaviour
    {
        [SerializeField] private Image background;
        
        public void Show()
        {
            background.enabled = true;
        }

        public void Hide()
        {
            background.enabled = false;
        }
    }
}
