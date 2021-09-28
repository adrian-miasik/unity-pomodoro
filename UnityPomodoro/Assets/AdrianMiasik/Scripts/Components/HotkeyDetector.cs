using System;
using UnityEngine;

namespace AdrianMiasik
{
    public class HotkeyDetector : MonoBehaviour
    {
        private PomodoroTimer timer;
        private bool isInitialized = false;

        public void Initialize(PomodoroTimer pomodoroTimer)
        {
            timer = pomodoroTimer;
            isInitialized = true;
        }

        private void Update()
        {
            if (!isInitialized)
            {
                return;
            }
            
            // Select All
            if (IsSelectingAll() && timer.state == PomodoroTimer.States.SETUP)
            {
                timer.SelectAll();
            }
            
            // Copy
            if (IsCopying())
            {
                GUIUtility.systemCopyBuffer = timer.GetTimerString();
            }
            
            // Paste
            if (IsPasting() && timer.state == PomodoroTimer.States.SETUP)
            {
                timer.SetTimerValue(GUIUtility.systemCopyBuffer);
            }
        }

        private bool IsSelectingAll()
        {
            if (Input.GetKeyUp(KeyCode.A) && Input.GetKey(KeyCode.LeftControl) ||
                Input.GetKeyUp(KeyCode.A) && Input.GetKey(KeyCode.RightControl)) 
            {
                return true;
            }

            return false;
        }
        
        private bool IsCopying()
        {
            if (Input.GetKeyUp(KeyCode.C) && Input.GetKey(KeyCode.LeftControl) ||
                Input.GetKeyUp(KeyCode.C) && Input.GetKey(KeyCode.RightControl)) 
            {
                return true;
            }

            return false;
        }
        
        private bool IsPasting()
        {
            if (Input.GetKeyUp(KeyCode.V) && Input.GetKey(KeyCode.LeftControl) ||
                Input.GetKeyUp(KeyCode.V) && Input.GetKey(KeyCode.RightControl)) 
            {
                return true;
            }

            return false;
        }
    }
}