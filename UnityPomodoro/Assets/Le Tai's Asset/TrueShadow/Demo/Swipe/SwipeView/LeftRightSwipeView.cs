using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LeTai.SwipeView
{
public enum SwipeDirection
{
    Left,
    Right
}

[Serializable]
public class CardSwipedToDirectionEvent : UnityEvent<SwipeDirection> { }

public class LeftRightSwipeView<T> : SwipeView<T>
{
    public CardSwipedToDirectionEvent onSwipeToDirection;

    protected override void Init(IEnumerable<T> data)
    {
        base.Init(data);
        onSwiped.AddListener(OnSwiped);
    }

    void OnSwiped(Vector2 offset)
    {
        if (Vector2.Dot(offset, Vector2.right) >= 0)
            onSwipeToDirection.Invoke(SwipeDirection.Right);
        else
            onSwipeToDirection.Invoke(SwipeDirection.Left);
    }
}
}
