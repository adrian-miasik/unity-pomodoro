using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace LeTai.TrueShadow
{
public partial class ShadowRenderer
{
    // TODO: cleanup unused mask materials
    static readonly Dictionary<int, Material> MASK_MATERIALS_CACHE = new Dictionary<int, Material>();

    internal static void ClearMaskMaterialCache()
    {
        foreach (var keyValuePair in MASK_MATERIALS_CACHE)
        {
            if (Application.isPlaying)
                Destroy(keyValuePair.Value);
            else
                DestroyImmediate(keyValuePair.Value);
        }

        MASK_MATERIALS_CACHE.Clear();
    }

    public Material GetModifiedMaterial(Material baseMaterial)
    {
        if (!shadow)
            return baseMaterial;

        shadow.ModifyShadowRendererMaterial(baseMaterial);

        if (!baseMaterial.HasProperty(ShaderId.STENCIL_ID))
            return baseMaterial; // Shadow is not masked

        var  casterMask   = shadow.GetComponent<Mask>();
        bool casterIsMask = casterMask != null && casterMask.isActiveAndEnabled;

        int hash = HashUtils.CombineHashCodes(
            casterIsMask.GetHashCode(),
            baseMaterial.GetHashCode()
        );

        MASK_MATERIALS_CACHE.TryGetValue(hash, out var mat);

        if (!mat)
        {
            mat = new Material(baseMaterial);

            if (shadow.ShadowAsSibling)
            {
                // Prevent shadow from writing to stencil mask
                mat.SetInt(ShaderId.COLOR_MASK, (int)ColorWriteMask.All);
                mat.SetInt(ShaderId.STENCIL_OP, (int)StencilOp.Keep);
            }
            else if (casterIsMask)
            {
                // Escape own mask
                var baseStencilId = mat.GetInt(ShaderId.STENCIL_ID) + 1;
                int stencilDepth  = 0;
                for (; stencilDepth < 8; stencilDepth++)
                {
                    if (((baseStencilId >> stencilDepth) & 1) == 1)
                        break;
                }

                stencilDepth = Mathf.Max(0, stencilDepth - 1);
                var stencilId = (1 << stencilDepth) - 1;

                mat.SetInt(ShaderId.STENCIL_ID,        stencilId);
                mat.SetInt(ShaderId.STENCIL_READ_MASK, stencilId);
            }

            MASK_MATERIALS_CACHE[hash] = mat;
        }
        else
        {
            // Copy over new materials props, but keep masking data
            var id        = mat.GetInt(ShaderId.STENCIL_ID);
            var op        = mat.GetInt(ShaderId.STENCIL_OP);
            var colorMask = mat.GetInt(ShaderId.COLOR_MASK);
            var readMask  = mat.GetInt(ShaderId.STENCIL_READ_MASK);

            mat.CopyPropertiesFromMaterial(baseMaterial);

            mat.SetInt(ShaderId.STENCIL_ID,        id);
            mat.SetInt(ShaderId.STENCIL_OP,        op);
            mat.SetInt(ShaderId.COLOR_MASK,        colorMask);
            mat.SetInt(ShaderId.STENCIL_READ_MASK, readMask);
        }

        return mat;
    }
}
}
