using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace LeTai.Asset.TranslucentImage.UniversalRP
{
public static class Extensions
{
    public static void BlitCustom(this CommandBuffer     cmd,
                                  RenderTargetIdentifier source,
                                  RenderTargetIdentifier destination,
                                  Material               material,
                                  int                    passIndex,
                                  BlitMode               blitMode)
    {
        switch (blitMode)
        {
        case BlitMode.Procedural:
            cmd.BlitProcedural(source, destination, material, passIndex);
            break;
        case BlitMode.Triangle:
            cmd.BlitFullscreenTriangle(source, destination, material, passIndex);
            break;
        default:
            throw new ArgumentOutOfRangeException(nameof(blitMode), blitMode, null);
        }
    }

    public static void BlitProcedural(this CommandBuffer     cmd,
                                      RenderTargetIdentifier source,
                                      RenderTargetIdentifier destination,
                                      Material               material,
                                      int                    passIndex)
    {
        cmd.SetGlobalTexture(ShaderId.MAIN_TEX, source);
        cmd.SetRenderTarget(new RenderTargetIdentifier(destination, 0, CubemapFace.Unknown, -1),
                            RenderBufferLoadAction.DontCare,
                            RenderBufferStoreAction.Store,
                            RenderBufferLoadAction.DontCare,
                            RenderBufferStoreAction.DontCare);
        cmd.DrawProcedural(Matrix4x4.identity, material, passIndex, MeshTopology.Quads, 4, 1, null);
    }
}

public enum BlitMode
{
    Procedural,
    Triangle
}
}
