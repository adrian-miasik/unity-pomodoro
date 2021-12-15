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
        [SerializeField] private ClickButton cancel;
        [SerializeField] private ClickButton submit;

        public UnityEvent OnCancel;
        public UnityEvent OnSubmit;

        private Action onCancel;
        private Action onSumbit;

        public void Initialize(Action _submit)
        {
            Initialize(Close, _submit);
        }

        public void Initialize(Action _cancel, Action _submit)
        {
            onCancel = _cancel;
            onSumbit = _submit;
        }

        // Invoked by button
        public void Cancel()
        {
            onCancel.Invoke();
        }

        // Invoked by button
        public void Submit()
        {
            onSumbit.Invoke();
        }

        public void Close()
        {
            Debug.Log("Closing dialog");
            Destroy(gameObject);
        }
    }
}
