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
    public class TwoChoiceDialog : MonoBehaviour, IColorHook
    {
        [SerializeField] private Image backgroundBox;
        [SerializeField] private TMP_Text topLabel;
        [SerializeField] private TMP_Text botLabel;
        [SerializeField] private ClickButtonText submit;
        [SerializeField] private ClickButtonText cancel;
        [SerializeField] private List<Image> lineSeparations;
        [SerializeField] private Image overlay;

        // Used to trigger Cancel and Submit methods via UnityEvent
        public UnityEvent OnCancel;
        public UnityEvent OnSubmit;

        // Used to combine actions
        private Action onCancel;
        private Action onSubmit;
        
        private PomodoroTimer timer;
        
        public void Initialize(PomodoroTimer _timer, Action _submit)
        {
            Initialize(_timer, Close, _submit);
        }

        public void Initialize(PomodoroTimer _timer, Action _submit, Action _cancel)
        {
            timer = _timer;
            timer.GetTheme().RegisterColorHook(this);

            onCancel = _cancel;
            onSubmit = _submit;
            
            ColorUpdate(_timer.GetTheme());
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

        public void ColorUpdate(Theme _theme)
        {
            // Background
            Color _backgroundColor = _theme.GetCurrentColorScheme().background;
            _backgroundColor.a = _theme.isLightModeOn ? 0.8f : 0.975f;
            backgroundBox.color = _backgroundColor;
            
            // Text
            topLabel.color = _theme.GetCurrentColorScheme().foreground;
            botLabel.color = _theme.GetCurrentColorScheme().foreground;
            submit.text.color = _theme.GetCurrentColorScheme().foreground;
            cancel.text.color = _theme.GetCurrentColorScheme().foreground;

            // Lines
            foreach (Image line in lineSeparations)
            {
                line.color = _theme.GetCurrentColorScheme().backgroundHighlight;
            }
            
            // Overlay
            Color _overlayColor = _theme.GetCurrentColorScheme().foreground;
            _overlayColor.a = _theme.isLightModeOn ? 0.5f : 0.025f;
            overlay.color = _overlayColor;
        }
    }
}