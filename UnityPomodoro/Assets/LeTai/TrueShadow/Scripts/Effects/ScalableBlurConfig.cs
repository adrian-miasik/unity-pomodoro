using UnityEngine;
using static UnityEngine.Mathf;

namespace LeTai.Effects
{
public class ScalableBlurConfig : BlurConfig
{
    [SerializeField]                 float radius    = 4;
    [SerializeField]                 int   iteration = 4;
    [SerializeField]                 int   maxDepth  = 6;
    [SerializeField] [Range(0, 256)] float strength;

    /// <summary>
    /// Distance between the base texel and the texel to be sampled.
    /// </summary>
    public float Radius
    {
        get { return radius; }
        set { radius = Max(0, value); }
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
        set { iteration = Max(0, value); }
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
        set { maxDepth = Max(1, value); }
    }

    /// <summary>
    /// User friendly property to control the amount of blur
    /// </summary>
    ///<value>
    /// Must be non-negative
    /// </value>
    public float Strength
    {
        get { return strength = radius * (3 * (1 << iteration) - 2) / UNIT_VARIANCE; }
        set
        {
            strength = Max(0, value);
            SetAdvancedFieldFromSimple();
        }
    }

    // With the "correct" unit variance, the edge of the shadow at higher stddev go below 8bit fixed point resolution
    // We "wastes" processing power on these.
    // TODO: optimize that:
    // The maximum distance that will show up is:
    // e^(-D^2 / 2R^2) < .5/256
    // => D < 3*sqrt(2*log(2)) * R ~ 3.53223*R
    // Can probably stop sooner than that
    static readonly float UNIT_VARIANCE = 1f + Sqrt(2f) / 2f;

    /// <summary>
    /// Calculate size and iteration from strength
    /// </summary>
    protected virtual void SetAdvancedFieldFromSimple()
    {
        if (strength < 1e-2)
        {
            iteration = 0;
            radius    = 0;
            return;
        }

        var variance = strength * UNIT_VARIANCE;

        // Each level of the pyramid have double the effective radius of the last, so total effective radius would be:
        // S = (2^0 + 2^1 +...+ 2^iteration +...+ 2^1 + 2^0) * R
        // https://en.wikipedia.org/wiki/1_%2B_2_%2B_4_%2B_8_%2B_%E2%8B%AF
        // S = (3 * 2^I - 2) * R
        // so:
        // I = log((s + 2r)/ (3r))/log(2)
        // and:
        // R = S / (3 * 2^I - 2)
        //
        // Experimental result show that best result are obtained with R <= 2^I - 1, so:
        // I >= log(1/6 * (sqrt(12S + 1) + 5))/log(2)
        //
        // There still some artifact at the lower end, not sure how to handle that yet
        // TODO: use a different algorithm for low Strength.

        iteration = CeilToInt(Log(1 / 6f * (Sqrt(12 * variance + 1) + 5)) / Log(2));

        radius = variance / (3 * (1 << iteration) - 2);
    }

    void OnValidate()
    {
        SetAdvancedFieldFromSimple();
    }
}
}
