using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.Events;

namespace AdrianMiasik
{
    public class PlayPauseButton : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SVGImage icon;
        
        [Header("SVGs")]
        [SerializeField] private Sprite play;
        [SerializeField] private Sprite pause;

        [Header("Colors")]
        [SerializeField] private Color playColor = new Color(0.35f, 0.89f, 0.4f);
        [SerializeField] private Color pauseColor = new Color(0.05f, 0.47f, 0.95f);

        // Unity Events
        public UnityEvent playOnClick;
        public UnityEvent pauseOnClick;

        // Cache
        private PomodoroTimer timer;
        private float xPivot; 
        
        public void Initialize(PomodoroTimer timer)
        {
            this.timer = timer;
            UpdateIcon();
        }

        /// <summary>
        /// Updates the icon sprite and color
        /// </summary>
        public void UpdateIcon()
        {
            // Correct icon offset
            Vector2 pivot = icon.rectTransform.pivot;
            icon.rectTransform.pivot = timer.IsRunning() ? new Vector2(0.5f, pivot.y) : new Vector2(0.6f, pivot.y); // magic numbers
            icon.rectTransform.offsetMin = new Vector2(0, 0);
            
            icon.sprite = timer.IsRunning() ? pause : play;
            icon.color = timer.IsRunning() ? pauseColor : playColor;
        }

        public void OnClick()
        {
            if (timer.IsRunning())
            {
                pauseOnClick.Invoke();
            }
            else
            {
                playOnClick.Invoke();
            }
            
            UpdateIcon();
        }
    }
}
