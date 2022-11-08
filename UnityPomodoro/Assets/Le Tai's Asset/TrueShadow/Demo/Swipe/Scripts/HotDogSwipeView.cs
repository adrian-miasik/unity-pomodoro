using System;
using System.Collections.Generic;
using LeTai.SwipeView;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LeTai.TrueShadow.Demo
{
public class HotDogSwipeView : LeftRightSwipeView<HotDogSprite>
{
    public Sprite[]       hotdogs;
    public Sprite[]       notHotdogs;
    public GradientSlider goodnessSlider;

    float goodness = 100f;

    protected override void Start()
    {
        Init(RandomSprites());
        onSwipeToDirection.AddListener(OnSwipeToDirection);

        AddGoodness(0);
    }

    void Update()
    {
        if (Application.isPlaying)
        {
            var reduction = Mathf.Exp(goodness * .025f + .6f);
            reduction = Mathf.Max(reduction, 0);
            AddGoodness(-reduction * Time.deltaTime);
        }
    }

    void AddGoodness(float amount)
    {
        goodness += amount;
        goodness =  Mathf.Clamp(goodness, 0f, 100f);
        goodnessSlider.Set(goodness / 100f);
    }

    void OnSwipeToDirection(SwipeDirection direction)
    {
        var isHotDog = TopCard.Data.isHotDog;
        switch (direction)
        {
            case SwipeDirection.Left:
                if (!isHotDog) AddGoodness(10);
                else AddGoodness(-10);
                break;
            case SwipeDirection.Right:
                if (isHotDog) AddGoodness(10);
                else AddGoodness(-10);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
    }

    IEnumerable<HotDogSprite> RandomSprites()
    {
        int last     = -1;
        int nAttempt = 0;
        while (true)
        {
            var hotDogRatio = hotdogs.Length / (float) (hotdogs.Length + notHotdogs.Length);
            var array       = Random.value < hotDogRatio ? hotdogs : notHotdogs;
            int next;
            do
            {
                next = Random.Range(0, array.Length);
            } while (next == last && nAttempt++ < 5);

            nAttempt = 0;
            last     = next;

            yield return new HotDogSprite {
                sprite   = array[next],
                isHotDog = array == hotdogs
            };
        }
    }
}
}
