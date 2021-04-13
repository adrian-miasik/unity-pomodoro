using UnityEngine;
using UnityEngine.UI;

namespace AdrianMiasik
{
    // TODO: Clean up
    public class PomodoroTimer : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private PlayPauseButton playPauseButton;

        [SerializeField] private Image progress;

        [Header("Colors")] 
        [SerializeField] private Color setupColor = new Color(0.05f, 0.47f, 0.95f);
        [SerializeField] private Color runningColor = new Color(0.35f, 0.89f, 0.4f);
        [SerializeField] private Color completedColor = new Color(0.97f, 0.15f, 0.15f);

        public float startingTime = 15f; // in seconds

        // Cache
        private bool _isRunning;
        private float _currentTime;
        private bool _isComplete;
        
        // Specific to our ring shader
        private static readonly int RingColor = Shader.PropertyToID("Color_297012532bf444df807f8743bdb7e4fd");
        
        private void Start()
        {
            Setup();
        }

        private void Setup()
        {
            _isComplete = false;
            
            Pause();
            _currentTime = startingTime;
            progress.fillAmount = 1f;
            
            playPauseButton.Initialize(_isRunning);
        }

        private void Update()
        {
            if (_isRunning)
            {
                if (_currentTime <= 0)
                {
                    _isRunning = false;
                    progress.fillAmount = 1f;
                    progress.material.SetColor(RingColor, completedColor);
                    playPauseButton.Initialize(_isRunning);

                    _isComplete = true;
                    
                    // Early exit
                    return;
                }

                progress.fillAmount = _currentTime / startingTime;
                _currentTime -= Time.deltaTime;
            }
        }

        public void Play()
        {
            if (_isComplete)
            {
                Setup();
            }
            
            _isRunning = true;
            progress.material.SetColor(RingColor, runningColor);
        }

        public void Pause()
        {
            _isRunning = false;
            progress.material.SetColor(RingColor, setupColor);
        }

        public void Restart()
        {
            Setup();
        }
    }
}
