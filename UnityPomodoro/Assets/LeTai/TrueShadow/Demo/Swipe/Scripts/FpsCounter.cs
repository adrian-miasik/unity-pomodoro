using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace LeTai.Utils
{
[RequireComponent(typeof(Text))]
public class FpsCounter : MonoBehaviour
{
    const long FPS_SAMPLE_PERIOD = 500;
    string      displayFormat    = "{0} FPS\n{1} ms";

    Text text;

    int   framesSinceLast;

    Stopwatch stopwatch;

    void Start()
    {
        stopwatch = Stopwatch.StartNew();
        text          = GetComponent<Text>();
        displayFormat = text.text;
    }

    void Update()
    {
        framesSinceLast++;

        var elapsedMs = stopwatch.ElapsedMilliseconds;
        if (elapsedMs < FPS_SAMPLE_PERIOD)
            return;

        float elapsedSec = elapsedMs / 1000f;

        var fps         = framesSinceLast / elapsedSec;
        var frameTimeMs = elapsedMs / (float)framesSinceLast;

        text.text     =  string.Format(displayFormat, fps, frameTimeMs);

        framesSinceLast = 0;
        stopwatch.Restart();
    }
}
}
