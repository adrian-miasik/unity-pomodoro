using UnityEngine;

namespace LeTai.TrueShadow.Demo
{
[RequireComponent(typeof(BarGraph))]
public class AdsrBarGraphSource : MonoBehaviour
{
    const float HOLD_TIME = 4f;

    public float attack  = 1;
    public float decay   = 1;
    public float sustain = .5f;
    public float release = 1;


#region EditorEventSetter

    public void SetAttack(float value)
    {
        attack = value;
        SetAdsrValues();
    }

    public void SetDecay(float value)
    {
        decay = value;
        SetAdsrValues();
    }

    public void SetSustain(float value)
    {
        sustain = value;
        SetAdsrValues();
    }

    public void SetRelease(float value)
    {
        release = value;
        SetAdsrValues();
    }

#endregion


    BarGraph    graph;
    AdsrEnvelop adsr;

    void Start()
    {
        graph = GetComponent<BarGraph>();
        graph.Init();
        adsr = new AdsrEnvelop();
        SetAdsrValues();
    }

    void SetAdsrValues()
    {
        if (!graph) return;

        var sampleRate = graph.barCount / HOLD_TIME;
        adsr.numAttackSamples  = Mathf.RoundToInt(attack * sampleRate);
        adsr.numDecaySamples   = Mathf.RoundToInt(decay * sampleRate);
        adsr.sustainScale      = sustain;
        adsr.numReleaseSamples = Mathf.RoundToInt(release * sampleRate);

        // int releaseIndex = graph.barCount - adsr.numReleaseSamples;
        int releaseIndex = Mathf.RoundToInt(graph.barCount * .75f);

        adsr.Reset();
        for (var i = 0; i < graph.barCount; i++)
        {
            if (i == releaseIndex)
                adsr.Release();

            adsr.MoveNext();
            graph.SetValue(i, (float) adsr.Current);
        }
    }
}
}
