using System;
using AdrianMiasik.Components.Base;
using UnityEngine;

namespace AdrianMiasik.Components.Core
{
    /// <summary>
    /// Responsible for spawning and keeping track of the currently used confirmation dialog.
    /// Use this class for interacting with our pop-up's via code.
    /// </summary>
    public class ConfirmationDialogManager : ThemeElement
    {
        [SerializeField] private ConfirmationDialog m_confirmationDialogPrefab; // Prefab reference
        [SerializeField] private Transform m_overlayCanvas; // Translucent Image Overlay Canvas
        
        // Cache
        private ConfirmationDialog currentDialogPopup;
        private bool isCurrentDialogInterruptible = true;
        
        /// <summary>
        /// Creates a custom <see cref="ConfirmationDialog"/> if one is currently not present/visible.
        /// <remarks>Either the submit/close buttons will trigger the dialog to close.</remarks>
        /// </summary>
        /// <param name="onSubmit">What do you want to do when the user presses yes?</param>
        /// <param name="onCancel">What do you want to do when the user presses no?</param>
        /// <param name="topText">What primary string do you want to display to the user?</param>
        /// <param name="bottomText">What secondary string do you want to display to the user?</param>
        /// <param name="interruptible">Can this popup be closed by our timer?</param>
        public void SpawnConfirmationDialog(Action onSubmit, Action onCancel = null, 
            string topText = null, string bottomText = null, bool interruptible = true)
        {
            if (currentDialogPopup != null)
                return;
            
            currentDialogPopup = Instantiate(m_confirmationDialogPrefab, m_overlayCanvas.transform);
            isCurrentDialogInterruptible = interruptible;
            currentDialogPopup.Initialize(Timer, this, onSubmit, onCancel, topText, bottomText);
        }

        public ConfirmationDialog GetCurrentConfirmationDialog()
        {
            return currentDialogPopup;
        }
        
        /// <summary>
        /// Is our current <see cref="ConfirmationDialog"/> interruptible by our timer?
        /// </summary>
        /// <returns></returns>
        public bool IsConfirmationDialogInterruptible()
        {
            return isCurrentDialogInterruptible;
        }

        /// <summary>
        /// Clear our current timer popup reference.
        /// <remarks>Should be done when destroying our popup dialog.</remarks>
        /// </summary>
        /// <param name="dialog"></param>
        public void ClearDialogPopup(ConfirmationDialog dialog)
        {
            if (dialog == currentDialogPopup)
            {
                currentDialogPopup = null;
            }
        }

        /// <summary>
        /// Clears and destroys the current timer popup so it's no longer visible to the user.
        /// </summary>
        public void ClearCurrentDialogPopup()
        {
            if (currentDialogPopup != null)
            {
                currentDialogPopup.Close();
                ClearDialogPopup(currentDialogPopup);
            }
        }

        public void TryClearCurrentDialogPopup()
        {
            if (currentDialogPopup != null && isCurrentDialogInterruptible)
            {
                currentDialogPopup.Close(true);
                ClearDialogPopup(currentDialogPopup);
            }
        }
    }
}