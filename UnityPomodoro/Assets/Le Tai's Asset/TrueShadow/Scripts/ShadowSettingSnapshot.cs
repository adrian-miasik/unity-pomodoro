using System;
using UnityEngine;
using UnityEngine.UI;

namespace LeTai.TrueShadow
{
class ShadowSettingSnapshot
{
    public readonly TrueShadow    shadow;
    public readonly Canvas        canvas;
    public readonly RectTransform canvasRt;
    public readonly float         canvasScale;
    public readonly float         size;
    public readonly Vector2       canvasRelativeOffset;
    public readonly Vector2       dimensions;

    internal ShadowSettingSnapshot(TrueShadow shadow)
    {
        this.shadow = shadow;
        canvas      = shadow.Graphic.canvas;
        canvasRt    = (RectTransform)canvas.transform;

        Bounds meshBound;
        if (shadow.SpriteMesh)
            meshBound = shadow.SpriteMesh.bounds;
        else
            meshBound = new Bounds(Vector3.zero, Vector3.zero);

        canvasScale = canvas.scaleFactor;

        var canvasRelativeRotation = Quaternion.Inverse(canvasRt.rotation) * shadow.RectTransform.rotation;
        canvasRelativeOffset = shadow.Offset.Rotate(-canvasRelativeRotation.eulerAngles.z) * canvasScale;

        dimensions = (Vector2)meshBound.size * canvasScale;
        size       = shadow.Size * canvasScale;

        CalcHash();
    }

    const int DIMENSIONS_HASH_STEP = 1;

    void CalcHash()
    {
        var graphic = shadow.Graphic;

        int canvasScaleHash = (int)(canvasScale * 1e4);
        int insetHash       = shadow.Inset ? 1 : 0;

        var clearColor = shadow.ClearColor;
        var imageColor = graphic.color;
        if (shadow.IgnoreCasterColor)
            imageColor = Color.clear;

        int colorHash = HashUtils.CombineHashCodes(
            shadow.IgnoreCasterColor ? 1 : 0,
            (int)shadow.ColorBleedMode,
            (int)(imageColor.r * 255),
            (int)(imageColor.g * 255),
            (int)(imageColor.b * 255),
            (int)(imageColor.a * 255),
            (int)(clearColor.r * 255),
            (int)(clearColor.g * 255),
            (int)(clearColor.b * 255),
            (int)(clearColor.a * 255)
        );

        // Hack until we have separated cutout cache, or proper sibling mode
        int offsetHash = HashUtils.CombineHashCodes(
            shadow.Cutout ? 1 : 0,
            (int)(canvasRelativeOffset.x * 100),
            (int)(canvasRelativeOffset.y * 100)
        );

        // Tiled type cannot be batched by similar size
        int dimensionHash = graphic is Image im && im.type == Image.Type.Tiled
                                ? dimensions.GetHashCode()
                                : HashUtils.CombineHashCodes(
                                    Mathf.CeilToInt(dimensions.x / DIMENSIONS_HASH_STEP) * DIMENSIONS_HASH_STEP,
                                    Mathf.CeilToInt(dimensions.y / DIMENSIONS_HASH_STEP) * DIMENSIONS_HASH_STEP
                                );

        var sizeHash   = Mathf.CeilToInt(size * 100);
        var spreadHash = Mathf.CeilToInt(shadow.Spread * 100);

        var commonHash = HashUtils.CombineHashCodes(
            shadow.TextureRevision,
            graphic.materialForRendering.ComputeCRC(),
            canvasScaleHash,
            insetHash,
            colorHash,
            offsetHash,
            dimensionHash,
            sizeHash,
            spreadHash,
            shadow.CustomHash
        );

        switch (graphic)
        {
        case Image image:
            int spriteHash = 0;
            if (image.sprite)
                spriteHash = image.sprite.GetHashCode();

            int imageHash = HashUtils.CombineHashCodes(
                (int)image.type,
                (int)(image.fillAmount * 360 * 20),
                (int)image.fillMethod,
                image.fillOrigin,
                image.fillClockwise ? 1 : 0
            );

            hash = HashUtils.CombineHashCodes(
                commonHash,
                spriteHash,
                imageHash
            );
            break;
        case RawImage rawImage:
            var textureHash = 0;
            if (rawImage.texture)
                textureHash = rawImage.texture.GetInstanceID();

            hash = HashUtils.CombineHashCodes(
                commonHash,
                textureHash
            );
            break;
        case Text text:
            // Other properties should all cause dimensions changes, so they do not need to be explicitly hashed
            hash = HashUtils.CombineHashCodes(
                commonHash,
                text.text.GetHashCode(),
                text.font.GetHashCode(),
                (int)text.alignment
            );
            break;

#if TMP_PRESENT
        case TMPro.TextMeshProUGUI tmp:
            // Other properties should all cause dimensions changes, so they do not need to be explicitly hashed
            int tmpColorHash = 0;
            if (!shadow.IgnoreCasterColor)
            {
                tmpColorHash = HashUtils.CombineHashCodes(
                    tmp.enableVertexGradient.GetHashCode(),
                    tmp.colorGradient.GetHashCode(),
                    tmp.overrideColorTags.GetHashCode()
                );
            }

            hash = HashUtils.CombineHashCodes(
                commonHash,
                tmp.text.GetHashCode(),
                tmp.font.GetHashCode(),
                tmp.fontSize.GetHashCode(),
                tmpColorHash,
                tmp.characterSpacing.GetHashCode(),
                tmp.wordSpacing.GetHashCode(),
                tmp.lineSpacing.GetHashCode(),
                tmp.paragraphSpacing.GetHashCode(),
                (int)tmp.alignment
            );
            break;
#endif
        default:
            hash = commonHash;
            break;
        }
    }

    int hash;

    // ReSharper disable once NonReadonlyMemberInGetHashCode
    public override int GetHashCode() => hash;

    public override bool Equals(object obj)
    {
        if (obj == null) return false;

        return GetHashCode() == obj.GetHashCode();
    }
}
}
