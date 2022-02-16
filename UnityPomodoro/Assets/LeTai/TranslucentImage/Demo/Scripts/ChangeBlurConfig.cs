using UnityEngine;

namespace LeTai.Asset.TranslucentImage.Demo
{
    [RequireComponent(typeof(TranslucentImageSource))]
    public class ChangeBlurConfig : MonoBehaviour
    {
        TranslucentImageSource    source;
        public TranslucentImage[] translucentImages;

        // Use this for initialization
        void Awake()
        {
            source = GetComponent<TranslucentImageSource>();
        }

        public void ChangeBlurStrength(float value)
        {
            //source.BlurRadius = value;
            ((ScalableBlurConfig) source.BlurConfig).Strength = value;
        }

        public void SetUpdateRate(float value)
        {
            source.maxUpdateRate = value;
        }

        public float GetUpdateRate()
        {
            return source.maxUpdateRate;
        }


        public void ChangeBlurSize(float value)
        {
            //source.BlurRadius = value;
            ((ScalableBlurConfig) source.BlurConfig).Radius = value;
        }

        public void ChangeIteration(float value)
        {
//            source.Iteration = Mathf.RoundToInt(value);
            ((ScalableBlurConfig) source.BlurConfig).Iteration = Mathf.RoundToInt(value);
        }

        public void ChangeDownsample(float value)
        {
            source.Downsample = Mathf.RoundToInt(value);
        }

        public void ChangeVibrancy(float value)
        {
            for (int i = 0; i < translucentImages.Length; i++)
            {
                translucentImages[i].vibrancy = value;
            }
        }
    }
}
