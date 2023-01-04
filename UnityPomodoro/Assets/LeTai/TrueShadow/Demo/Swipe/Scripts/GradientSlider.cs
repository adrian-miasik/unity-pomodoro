using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class GradientSlider : MonoBehaviour
{
    public Gradient gradient;

    Image image;

    void Start()
    {
        image = GetComponent<Image>();
    }

    public void Set(float value)
    {
        if (!image) return;

        image.fillAmount = value;
        image.color      = gradient.Evaluate(value);
    }
}
