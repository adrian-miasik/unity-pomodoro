using UnityEngine;

namespace LeTai.Asset.TranslucentImage
{
[CreateAssetMenu(fileName = "New Scalable Blur Config",
                 menuName = "Translucent Image/ Scalable Blur Config",
                 order = 100)]
public class ScalableBlurConfig : BlurConfig
{
    [SerializeField] float radius    = 4;
    [SerializeField] int   iteration = 4;
    [SerializeField] int   maxDepth  = 6;
    [SerializeField] float strength;

    /// <summary>
    /// Distance between the base texel and the texel to be sampled.
    /// </summary>
    public float Radius
    {
        get { return radius; }
        set { radius = Mathf.Max(0, value); }
    }

    /// <summary>
    /// Half the number of time to process the image. It is half because the real number of iteration must alway be even. Using half also make calculation simpler
    /// </summary>
    /// <value>
    /// Must be non-negative
    /// </value>
    public int Iteration
    {
        get { return iteration; }
        set { iteration = Mathf.Max(0, value); }
    }

    /// <summary>
    /// Clamp the minimum size of the intermediate texture. Reduce flickering and blur
    /// </summary>
    /// <value>
    /// Must larger than 0
    /// </value>
    public int MaxDepth
    {
        get { return maxDepth; }
        set { maxDepth = Mathf.Max(1, value); }
    }

    /// <summary>
    /// User friendly property to control the amount of blur
    /// </summary>
    ///<value>
    /// Must be non-negative
    /// </value>
    public float Strength
    {
        get { return strength = Radius * Mathf.Pow(2, Iteration); }
        set
        {
            strength = Mathf.Max(0, value);
            SetAdvancedFieldFromSimple();
        }
    }

    /// <summary>
    /// Calculate size and iteration from strength
    /// </summary>
    protected virtual void SetAdvancedFieldFromSimple()
    {
        var iterationPower = Mathf.Pow(2, Iteration);
        Radius = strength / iterationPower;

        while (Radius < 1 && Iteration > 0)
        {
            Iteration--;
            Radius *= 2;
        }

        while (Radius > iterationPower)
        {
            Radius /= 2;
            Iteration++;
        }
    }
}
}
