using UnityEngine;

namespace LeTai.TrueShadow.Demo
{
public class BarGraph : MonoBehaviour
{
    public GameObject barPrefab;
    public int        barCount = 15;
    public int        maxValue = 7;

    BarGraphBar[] bars;

    public void Init()
    {
        bars = new BarGraphBar[barCount];

        for (var i = 0; i < barCount; i++)
        {
            var go  = Instantiate(barPrefab, transform);
            var bar = go.GetComponent<BarGraphBar>();
            bar.Init(maxValue);
            bars[i] = bar;
        }
    }

    public void SetValue(int index, float value)
    {
        bars[index].SetValue(value);
    }
}
}
