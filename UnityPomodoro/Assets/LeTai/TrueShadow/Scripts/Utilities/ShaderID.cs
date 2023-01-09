using UnityEngine;

namespace LeTai
{
public static class ShaderId
{
    public static readonly int MAIN_TEX           = Shader.PropertyToID("_MainTex");
    public static readonly int SHADOW_TEX         = Shader.PropertyToID("_ShadowTex");
    public static readonly int CLIP_RECT          = Shader.PropertyToID("_ClipRect");
    public static readonly int TEXTURE_SAMPLE_ADD = Shader.PropertyToID("_TextureSampleAdd");
    public static readonly int COLOR_MASK         = Shader.PropertyToID("_ColorMask");
    public static readonly int STENCIL_OP         = Shader.PropertyToID("_StencilOp");
    public static readonly int STENCIL_ID         = Shader.PropertyToID("_Stencil");
    public static readonly int STENCIL_READ_MASK  = Shader.PropertyToID("_StencilReadMask");
    public static readonly int OFFSET             = Shader.PropertyToID("_Offset");
    public static readonly int OVERFLOW_ALPHA     = Shader.PropertyToID("_OverflowAlpha");
    public static readonly int ALPHA_MULTIPLIER   = Shader.PropertyToID("_AlphaMultiplier");
    public static readonly int SCREEN_PARAMS   = Shader.PropertyToID("_ScreenParams");
    public static readonly int SCALE_X   = Shader.PropertyToID("_ScaleX");
    public static readonly int SCALE_Y   = Shader.PropertyToID("_ScaleY");
}
}
