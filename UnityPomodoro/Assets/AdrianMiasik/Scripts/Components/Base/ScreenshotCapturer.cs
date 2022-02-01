using System;
using System.Collections;
using UnityEngine;

namespace AdrianMiasik.Components.Base
{
    public class ScreenshotCapturer : MonoBehaviour
    {
        public static ScreenshotCapturer Instance;
        
        private void Awake()
        {
            Instance = this;
        }

        IEnumerator TakeScreenshot(string filename, float delayInSeconds)
        {
            Debug.Log("Screenshot Capture Delay...");
            yield return new WaitForSeconds(delayInSeconds);
            ScreenCapture.CaptureScreenshot("screenshot_0.png");
            Debug.Log("Screenshot '" + filename + "' Taken!");
        }

        public void CaptureScreenshot(string filename, float delayInSeconds)
        {
            Instance.StartCoroutine(TakeScreenshot(filename, delayInSeconds));
        }
    }
}