using LeTai.TrueShadow;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SymbolsManager : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] float          cellSize = 100;
    [SerializeField] GameObject     shapePrefab;
    [SerializeField] Sprite[]       sprites;
    [SerializeField] Color[]        colors;
    [SerializeField] float          maxReactionDistance;
    [SerializeField] AnimationCurve reactionCurve;
    [SerializeField] float          heightOffset;
    [SerializeField] Vector2        shadowSizeMinMax;
    [SerializeField] Vector2        shadowDistanceMinMax;
    [SerializeField] Gradient       shadowGradient;

    [SerializeField] TrueShadow[] samples;
#pragma warning restore 0649

    float colorScale = 1;

    RectTransform selfRt;
    Camera        interectionCam;

    Vector2[]       initialPositions;
    RectTransform[] rectTransforms;
    TrueShadow[]    shadows;

    void Start()
    {
        interectionCam = Camera.main;
        Spawn();
    }

    void Spawn()
    {
        selfRt = GetComponent<RectTransform>();

        var res    = GetComponentInParent<CanvasScaler>().referenceResolution;
        var xCount = Mathf.CeilToInt(res.x / cellSize);
        var yCount = Mathf.CeilToInt(res.y / cellSize);
        var count  = xCount * yCount;
        initialPositions = new Vector2[count];
        rectTransforms   = new RectTransform[count];
        shadows          = new TrueShadow[count];

        var randomFrom = .25f * cellSize;
        var randomTo   = .75f * cellSize;

        for (int idY = 0; idY < yCount; idY++)
        for (int idX = 0; idX < xCount; idX++)
        {
            var obj = Instantiate(shapePrefab, transform);

            var rt = obj.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.zero;
            rt.anchoredPosition = new Vector3(
                idX * cellSize + Random.Range(randomFrom, randomTo),
                idY * cellSize + Random.Range(randomFrom, randomTo),
                transform.position.z
            );
            rt.rotation  =  Quaternion.Euler(0, 0, Mathf.Floor((Random.value - .5f) * 4) * 90 / 4);
            rt.sizeDelta *= cellSize / 220;

            var img      = obj.GetComponent<Image>();
            var spriteId = Random.Range(0, sprites.Length);
            img.sprite = sprites[spriteId];
            img.color  = colors[spriteId];

            var index = idY * xCount + idX;
            rectTransforms[index]   = rt;
            initialPositions[index] = rt.anchoredPosition;
            shadows[index]          = img.GetComponent<TrueShadow>();
        }
    }

    void Update()
    {
        React();
    }

    void React()
    {
        var count = initialPositions.Length;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(selfRt,
                                                                Input.mousePosition,
                                                                interectionCam,
                                                                out var mouse);

        for (int i = 0; i < count; i++)
        {
            var position = initialPositions[i];
            var dist     = (position - mouse).magnitude;

            var reaction = reactionCurve.Evaluate(Mathf.InverseLerp(maxReactionDistance, 0, dist));

            position.y += heightOffset * reaction;

            rectTransforms[i].anchoredPosition = position;

            var shadow = shadows[i];
            shadow.Size           = Mathf.Lerp(shadowSizeMinMax.x,     shadowSizeMinMax.y,     reaction);
            shadow.OffsetDistance = Mathf.Lerp(shadowDistanceMinMax.x, shadowDistanceMinMax.y, reaction);

            var color = shadowGradient.Evaluate(reaction);
            color.r      *= colorScale;
            color.g      *= colorScale;
            color.b      *= colorScale;
            shadow.Color =  color;
        }
    }

    public void SetMaxSize(float maxSize)
    {
        shadowSizeMinMax.y = maxSize;

        var sampleSize = maxSize / 2f;
        for (var i = 0; i < samples.Length; i++)
        {
            samples[i].Size = sampleSize;
        }
    }

    public void SetColorScale(float scale)
    {
        colorScale = scale;

        var sampleColor = Color.white * (scale / 2f + .25f);
        sampleColor.a = samples[0].Color.a;
        for (var i = 0; i < samples.Length; i++)
        {
            samples[i].Color = sampleColor;
        }
    }
}
