using System;
using System.Collections.Generic;
using AdrianMiasik.Components.Core;
using AdrianMiasik.Components.Wrappers;
using AdrianMiasik.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AdrianMiasik.Components
{
    public class ConfirmationDialog : ThemeElement
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
        
        /// <summary>
        /// Setup our confirmation dialog with custom actions and even custom text
        /// </summary>
        /// <param name="pomodoroTimer">Main class reference</param>
        /// <param name="submit">The action you want to take when the user presses yes</param>
        /// <param name="cancel">The action you want to take when the user presses no</param>
        /// <param name="topText">Optional: The top text label you want to override</param>
        /// <param name="bottomText">Optional: The bottom text label you want to override</param>
        public void Initialize(PomodoroTimer pomodoroTimer, Action submit, Action cancel, 
            string topText = null, string bottomText = null)
        {
            base.Initialize(pomodoroTimer);
            
            onCancel = cancel;
            onSubmit = submit;

            if (!String.IsNullOrEmpty(topText))
            {
                m_topLabel.text = topText;
            }

            if (!String.IsNullOrEmpty(bottomText))
            {
                m_botLabel.text = bottomText;
            }

            m_spawnAnimation.Stop();
            m_spawnAnimation.Play();
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

        public void Close(bool checkInterruptibility = false)
        {
            if (checkInterruptibility)
            {
                if (Timer.IsConfirmationDialogInterruptible())
                {
                    DestroyDialog();
                }
                else
                {
                    Debug.LogWarning("This confirmation dialog is not interruptible.");
                }
            }
            else
            {
                DestroyDialog();
            }
        }

        private void DestroyDialog()
        {
            Timer.ClearDialogPopup(this);
            Timer.GetTheme().Deregister(this); // Remove self from themed components
            Destroy(gameObject);
        }

        public override void ColorUpdate(Theme theme)
        {
            // Background
            Color backgroundColor = theme.GetCurrentColorScheme().m_background;
            backgroundColor.a = theme.m_darkMode ? 0.975f : 0.8f;
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
            overlayColor.a = theme.m_darkMode ? 0.025f : 0.5f;
            m_overlay.color = overlayColor;
        }
    }
}