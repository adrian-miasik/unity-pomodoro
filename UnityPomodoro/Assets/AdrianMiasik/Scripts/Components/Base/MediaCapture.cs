using System;
using System.Collections;
using AdrianMiasik.Components.Specific;
using UnityEngine;

namespace AdrianMiasik.Components.Base
{
    public class MediaCapture : MonoBehaviour
    {
        public static MediaCapture Instance;
        
        private void Awake()
        {
            Instance = this;
        }
        
        IEnumerator TakeScreenshot(string filename, Action nextAction = null)
        {
            yield return new WaitForEndOfFrame();
            ScreenCapture.CaptureScreenshot(filename);
            Debug.Log("Screenshot '" + filename + "' Taken!");

            nextAction?.Invoke();
        }

        public void CaptureScreenshot(string filename, Action nextAction)
        {
            Instance.StartCoroutine(TakeScreenshot(filename, nextAction));
        }
    }
}