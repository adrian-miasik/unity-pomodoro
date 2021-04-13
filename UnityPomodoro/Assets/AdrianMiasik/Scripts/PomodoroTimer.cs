using System;
using UnityEngine;
using UnityEngine.UI;

namespace AdrianMiasik
{
    // TODO: Clean up
    public class PomodoroTimer : MonoBehaviour
    {
        [Header("Digits")] 
        [SerializeField] private DoubleDigit hourDigits;
        [SerializeField] private DoubleDigit minuteDigits;
        [SerializeField] private DoubleDigit secondDigits;

        [Header("Buttons")]
        [SerializeField] private PlayPauseButton playPauseButton;

        [Header("Ring")]
        [SerializeField] private Image progress;

        [Header("Colors")] 
        [SerializeField] private Color setupColor = new Color(0.05f, 0.47f, 0.95f);
        [SerializeField] private Color runningColor = new Color(0.35f, 0.89f, 0.4f);
        [SerializeField] private Color completedColor = new Color(0.97f, 0.15f, 0.15f);

        [Header("Data")]
        [SerializeField] private int hours = 0;
        [SerializeField] private int minutes = 0;
        [SerializeField] private int seconds = 15;
        
        // Cache
        private bool _isRunning;
        private double _currentTime;
        private bool _isComplete;
        
        // Specific to our ring shader
        private static readonly int RingColor = Shader.PropertyToID("Color_297012532bf444df807f8743bdb7e4fd");

        private float totalTimeInSeconds;

        private void OnValidate()
        {
            hours = Mathf.Clamp(hours, 0, 99);
            minutes = Mathf.Clamp(minutes, 0, 59);
            seconds = Mathf.Clamp(seconds, 0, 59);
        }

        private void Start()
        {
            Setup();
        }

        private void Setup()
        {
            _isComplete = false;
            TimeSpan ts = TimeSpan.FromHours(hours) +  TimeSpan.FromMinutes(minutes) +  TimeSpan.FromSeconds(seconds);
            CalculateDigits(ts);
            _currentTime = ts.TotalSeconds;
            totalTimeInSeconds = (float) ts.TotalSeconds;
            
            Pause();
            progress.fillAmount = 1f;
            
            playPauseButton.Initialize(_isRunning);
        }

        private void CalculateDigits(TimeSpan timeSpan)
        {
            hourDigits.Initialize((int) timeSpan.TotalHours);
            minuteDigits.Initialize(timeSpan.Minutes);
            secondDigits.Initialize(timeSpan.Seconds);
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

                progress.fillAmount = (float) _currentTime / totalTimeInSeconds;
                
                CalculateDigits(TimeSpan.FromSeconds(_currentTime));
                
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
