using System;
using System.Collections;
using UnityEngine;

namespace AdrianMiasik.Components.Specific.Automation
{
    /// <summary>
    /// A MonoBehaviour script that gets created by our <see cref="MediaCreator"/> that allows us to use coroutines
    /// from a static context, specifically used for taking screenshots at the end of our frame.
    /// </summary>
    public class MediaCapture : MonoBehaviour
    {
        private static MediaCapture _instance;
        
        private void Awake()
        {
            _instance = this;
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
            _instance.StartCoroutine(TakeScreenshot(filename, nextAction));
        }
    }
}