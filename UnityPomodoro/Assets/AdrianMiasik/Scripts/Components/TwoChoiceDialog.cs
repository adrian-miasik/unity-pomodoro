using System;
using AdrianMiasik.Components.Core;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AdrianMiasik.Components
{
    public class TwoChoiceDialog : MonoBehaviour
    {
        [SerializeField] private Image backgroundBox;
        [SerializeField] private TMP_Text topLabel;
        [SerializeField] private TMP_Text botLabel;
        [SerializeField] private ClickButton submit;
        [SerializeField] private ClickButton cancel;

        // Used to trigger Cancel and Submit methods via UnityEvent
        public UnityEvent OnCancel;
        public UnityEvent OnSubmit;

        // Used to combine actions
        private Action onCancel;
        private Action onSubmit;
        
        public void Initialize(Action _submit)
        {
            Initialize(Close, _submit);
        }

        public void Initialize(Action _submit, Action _cancel)
        {
            onCancel = _cancel;
            onSubmit = _submit;
        }

        // UnityEvent - Invoked by no button
        public void Cancel()
        {
            onCancel.Invoke();
            Close();
        }

        // UnityEvent - Invoked by yes button
        public void Submit()
        {
            onSubmit.Invoke();
            Close();
        }

        public void Close()
        {
            Destroy(gameObject);
        }
    }
}