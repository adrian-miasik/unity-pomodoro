using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace LeTai.Asset.TranslucentImage
{
public static class Extensions
{
    static Mesh fullscreenTriangle;

    /// <summary>
    /// A fullscreen triangle mesh.
    /// </summary>
    static Mesh FullscreenTriangle
    {
        get
        {
            if (fullscreenTriangle != null)
                return fullscreenTriangle;

            fullscreenTriangle = new Mesh { name = "Fullscreen Triangle" };
            fullscreenTriangle.SetVertices(
                new List<Vector3> {
                    new Vector3(-1f, -1f, 0f),
                    new Vector3(-1f, 3f,  0f),
                    new Vector3(3f,  -1f, 0f)
                }
            );
            fullscreenTriangle.SetIndices(new[] { 0, 1, 2 }, MeshTopology.Triangles, 0, false);
            fullscreenTriangle.UploadMeshData(false);

            return fullscreenTriangle;
        }
    }

    public static void BlitFullscreenTriangle(this CommandBuffer     cmd,
                                              RenderTargetIdentifier source,
                                              RenderTargetIdentifier destination,
                                              Material               material,
                                              int                    pass)
    {
        cmd.SetGlobalTexture("_MainTex", source);

#if UNITY_2018_2_OR_NEWER
        cmd.SetRenderTarget(destination, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
#else
            cmd.SetRenderTarget(destination);
#endif

        cmd.DrawMesh(FullscreenTriangle, Matrix4x4.identity, material, 0, pass);
    }

    /// For normalized screen size
    internal static bool Approximately(this Rect self, Rect other)
    {
        return QuickApproximate(self.x,      other.x)
            && QuickApproximate(self.y,      other.y)
            && QuickApproximate(self.width,  other.width)
            && QuickApproximate(self.height, other.height);
    }

    const float EPSILON01 = 5.9604644e-8f; // different between 1 and largest float < 1

    private static bool QuickApproximate(float a, float b)
    {
        return Mathf.Abs(b - a) < EPSILON01;
    }

    public static Vector4 ToMinMaxVector(this Rect self)
    {
        return new Vector4(
            self.xMin,
            self.yMin,
            self.xMax,
            self.yMax
        );
    }

    public static Vector4 ToVector4(this Rect self)
    {
        return new Vector4(
            self.xMin,
            self.yMin,
            self.width,
            self.height
        );
    }
}
}
