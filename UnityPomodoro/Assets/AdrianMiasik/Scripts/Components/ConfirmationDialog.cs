using System;
using System.Collections.Generic;
using AdrianMiasik.Components.Core;
using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AdrianMiasik.Components
{
    public class ConfirmationDialog : MonoBehaviour, IColorHook
    {
        [SerializeField] private Image m_backgroundBox;
        [SerializeField] private TMP_Text m_topLabel;
        [SerializeField] private TMP_Text m_botLabel;
        [SerializeField] private ClickButtonText m_submit;
        [SerializeField] private ClickButtonText m_cancel;
        [SerializeField] private List<Image> m_lineSeparations;
        [SerializeField] private Image m_overlay;
        [SerializeField] private Animation m_spawnAnimation;
        
        // Used to combine actions
        private Action onCancel;
        private Action onSubmit;
        
        private PomodoroTimer timer;
        
        public void Initialize(PomodoroTimer pomodoroTimer, Action submit)
        {
            Initialize(pomodoroTimer, Close, submit);
        }

        public void Initialize(PomodoroTimer pomodoroTimer, Action submit, Action cancel)
        {
            timer = pomodoroTimer;
            timer.GetTheme().RegisterColorHook(this);

            onCancel = cancel;
            onSubmit = submit;
            
            m_spawnAnimation.Stop();
            m_spawnAnimation.Play();
            
            ColorUpdate(pomodoroTimer.GetTheme());
        }

        // UnityEvent - Invoked by no button
        public void Cancel()
        {
            onCancel?.Invoke();
            Close();
        }

        // UnityEvent - Invoked by yes button
        public void Submit()
        {
            onSubmit?.Invoke();
            Close();
        }

        public void Close()
        {
            timer.ClearDialogPopup(this);
            timer.GetTheme().Deregister(this); // Remove self from themed components
            Destroy(gameObject);
        }

        public void ColorUpdate(Theme theme)
        {
            // Background
            Color backgroundColor = theme.GetCurrentColorScheme().m_background;
            backgroundColor.a = theme.m_isLightModeOn ? 0.8f : 0.975f;
            m_backgroundBox.color = backgroundColor;
            
            // Text
            m_topLabel.color = theme.GetCurrentColorScheme().m_foreground;
            m_botLabel.color = theme.GetCurrentColorScheme().m_foreground;
            m_submit.m_text.color = theme.GetCurrentColorScheme().m_foreground;
            m_cancel.m_text.color = theme.GetCurrentColorScheme().m_foreground;

            // Lines
            foreach (Image line in m_lineSeparations)
            {
                line.color = theme.GetCurrentColorScheme().m_backgroundHighlight;
            }
            
            // Overlay
            Color overlayColor = theme.GetCurrentColorScheme().m_foreground;
            overlayColor.a = theme.m_isLightModeOn ? 0.5f : 0.025f;
            m_overlay.color = overlayColor;
        }
    }
}