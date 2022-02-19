using UnityEngine;

namespace LeTai.Asset.TranslucentImage
{
    public class ScalableBlur : IBlurAlgorithm
    {
        Shader             shader;
        Material           material;
        ScalableBlurConfig config;

        const int BLUR_PASS      = 0;
        const int CROP_BLUR_PASS = 1;

        Material Material
        {
            get
            {
                if (material == null)
                    Material = new Material(Shader.Find("Hidden/EfficientBlur"));

                return material;
            }
            set { material = value; }
        }

        public void Init(BlurConfig config)
        {
            this.config = (ScalableBlurConfig) config;
        }

        public void Blur(RenderTexture source, Rect sourceCropRegion, ref RenderTexture blurredTexture)
        {
            if (blurredTexture.IsCreated()) blurredTexture.DiscardContents();

            float radius = ScaleWithResolution(
                config.Radius,
                blurredTexture.width * sourceCropRegion.width,
                blurredTexture.height * sourceCropRegion.height
            );

            int firstDownsampleFactor = config.Iteration > 0 ? 1 : 0;

            var tmpRt = CreateTempRenderTextureFrom(blurredTexture, firstDownsampleFactor);

            //Initial downsample
            var sourceFilterMode = source.filterMode;
            source.filterMode = FilterMode.Bilinear;

            ConfigMaterial(radius, sourceCropRegion.ToMinMaxVector());

            Graphics.Blit(source, tmpRt, Material, CROP_BLUR_PASS);

            //Downsample. (iteration - 1) pass
            for (int i = 2; i <= config.Iteration; i++)
            {
                BlurAtDepth(i, ref blurredTexture, ref tmpRt);
            }

            //Upsample. (iteration - 1) pass
            for (int i = config.Iteration - 1; i >= 1; i--)
            {
                BlurAtDepth(i, ref blurredTexture, ref tmpRt);
            }

            //Final upsample
            Graphics.Blit(tmpRt, blurredTexture, Material, BLUR_PASS);
            RenderTexture.ReleaseTemporary(tmpRt);

            source.filterMode = sourceFilterMode;
        }

        ///<summary>
        /// Relative blur size to maintain same look across multiple resolution
        /// </summary>
        float ScaleWithResolution(float baseRadius, float width, float height)
        {
            float scaleFactor = Mathf.Min(width, height) / 1080f;
            scaleFactor = Mathf.Clamp(scaleFactor, .5f, 2f); //too much variation cause artifact
            return baseRadius * scaleFactor;
        }

        protected void ConfigMaterial(float radius, Vector4 cropRegion)
        {
            Material.SetFloat(ShaderId.RADIUS, radius);
            Material.SetVector(ShaderId.CROP_REGION, cropRegion);
        }

        RenderTexture CreateTempRenderTextureFrom(RenderTexture source, int downsampleFactor)
        {
        var desc = source.descriptor;
        desc.width  = source.width >> downsampleFactor; //= width / 2^downsample
        desc.height = source.height >> downsampleFactor;

        var rt = RenderTexture.GetTemporary(desc);
            rt.filterMode = FilterMode.Bilinear;
            return rt;
        }

        protected virtual void BlurAtDepth(int depth, ref RenderTexture baseTexture, ref RenderTexture target)
        {
            var tmpRt = CreateTempRenderTextureFrom(baseTexture, Mathf.Min(depth, config.MaxDepth));

            Graphics.Blit(target, tmpRt, Material, BLUR_PASS);

            RenderTexture.ReleaseTemporary(target);
            target = tmpRt;
        }
    }
}
