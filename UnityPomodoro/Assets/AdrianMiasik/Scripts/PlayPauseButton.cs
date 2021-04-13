using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.Events;

namespace AdrianMiasik
{
    public class PlayPauseButton : SVGImage
    {
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
        private bool _isRunning = false;
        
        public void Initialize(bool isTimerCountingDown)
        {
            _isRunning = isTimerCountingDown;
            UpdateIcon();
        }
        
        public void OnClick()
        {
            if (_isRunning)
            {
                pauseOnClick.Invoke();
            }
            else
            {
                playOnClick.Invoke();
            }
            
            // Flip state
            _isRunning = !_isRunning;
            UpdateIcon();
        }

        /// <summary>
        /// Updates the icon sprite and color
        /// </summary>
        private void UpdateIcon()
        {
            sprite = _isRunning ? pause : play;
            color = _isRunning ? pauseColor : playColor;
        }
    }
}
