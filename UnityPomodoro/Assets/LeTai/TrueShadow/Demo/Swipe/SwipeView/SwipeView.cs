using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LeTai.SwipeView
{
[Serializable]
public class CardSwipedEvent : UnityEvent<Vector2> { }

public class SwipeView<TData> : Graphic, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int             stackSize = 4;
    public GameObject      cardPrefab;
    public AnimationCurve  shapeXCurve;
    public AnimationCurve  shapeYCurve;
    public AnimationCurve  sizeCurve;
    public Vector2         rotationPivot       = Vector2.down * 400;
    public float           distanceToRemove    = 100f;
    public float           animationSmoothTime = .15f;
    public CardSwipedEvent onSwiped;

    public Swipable<TData> TopCard => cards.Count > 0 ? cards.Peek() : null;

    protected IEnumerable<TData>     data;
    protected Queue<Swipable<TData>> cards;

    internal float throwDistance;

    IEnumerator<TData> dataEnumerator;

    protected virtual void Init(IEnumerable<TData> data)
    {
        if (!Application.isPlaying)
            return;

        cards          = new Queue<Swipable<TData>>(stackSize);
        this.data      = data;
        dataEnumerator = this.data.GetEnumerator();

        for (var i = 0; i < stackSize; i++) InsertNextCard();
    }

    void InsertNextCard()
    {
        if (dataEnumerator.MoveNext())
        {
            var obj  = Instantiate(cardPrefab, transform);
            var card = obj.GetComponent<Swipable<TData>>();
            if (!card) throw new ArgumentException($"Card Prefab need a {nameof(Swipable<TData>)} component");

            card.view = this;
            card.SetData(dataEnumerator.Current);
            card.removed += RemoveTopCard;
            card.RectTransform.SetSiblingIndex(0);

            cards.Enqueue(card);

            obj.name = card.Data.ToString();
        }

        UpdateCardsPosition();
    }

    void UpdateCardsPosition()
    {
        int index = 0;
        foreach (var card in cards)
        {
            var curvePosition = index++ % stackSize / (float) (stackSize - 1);
            card.snapScale = Vector3.one * sizeCurve.Evaluate(curvePosition);
            card.snapPosition = new Vector2(
                shapeXCurve.Evaluate(curvePosition),
                shapeYCurve.Evaluate(curvePosition)
            );

            card.StartSnap();
        }
    }

    void RemoveTopCard(Vector2 offset)
    {
        onSwiped.Invoke(offset);
        cards.Dequeue();
        InsertNextCard();
    }

    Vector2 pointerStartPos;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!TopCard) return;

        EventSystem.current.SetSelectedGameObject(TopCard.gameObject, eventData);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out pointerStartPos
        );

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            Vector2.right * (Screen.width + Screen.height),
            eventData.pressEventCamera,
            out var farPoint
        );
        throwDistance = farPoint.magnitude;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!TopCard) return;

        EventSystem.current.SetSelectedGameObject(TopCard.gameObject, eventData);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out var pos
        );
        TopCard.Swipe(pos - pointerStartPos);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!TopCard) return;

        EventSystem.current.SetSelectedGameObject(null);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out var pos
        );
        TopCard.EndSwipe(pos - pointerStartPos);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        UnityEditor.Handles.color = Color.green;
        var rotPivotWorld = transform.TransformPoint(rotationPivot * canvas.scaleFactor);
        UnityEditor.Handles.Disc(Quaternion.identity,
                                 rotPivotWorld,
                                 transform.forward,
                                 UnityEditor.HandleUtility.GetHandleSize(rotPivotWorld) * .1f,
                                 false,
                                 0);
    }
#endif
}
}
