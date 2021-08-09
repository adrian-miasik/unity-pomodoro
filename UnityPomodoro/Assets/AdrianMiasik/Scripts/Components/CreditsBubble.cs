using Unity.VectorGraphics;
using UnityEngine;

namespace AdrianMiasik.Components
{
    public class CreditsBubble : MonoBehaviour
    {
        [SerializeField] private SVGImage icon;
        [SerializeField] private CanvasGroup canvasGroup;
        
        // TODO: Wait 3 seconds, fade out with alpha group
        // TODO: Provide helper functions so the info page can toggle this credits bubble

        private void Update()
        {
            
        }

        [ContextMenu("Show")]
        public void Show()
        {
            canvasGroup.alpha = 1;
        }

        [ContextMenu("Hide")]
        public void Hide()
        {
            canvasGroup.alpha = 0;
        }
    }
}
