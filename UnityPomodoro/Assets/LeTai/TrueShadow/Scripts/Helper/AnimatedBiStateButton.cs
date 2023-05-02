using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LeTai.TrueShadow
{
public class AnimatedBiStateButton : MonoBehaviour,
                                     IPointerDownHandler, IPointerUpHandler,
                                     IPointerEnterHandler, IPointerExitHandler
{
    public enum State
    {
        Up,
        AnimateDown,
        Down,
        AnimateUp,
    }

    public float          animationDuration  = .1f;
    public AnimationCurve animationCurve     = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public bool           useEnterExitEvents = true;

    public event Action willPress;
    public event Action willRelease;

    protected State state = State.Up;

    /// <summary>
    /// 0 = fully up
    /// 1 = fully down
    /// </summary>
    protected float pressAmount = 0;

    protected bool IsAnimating => state == State.AnimateDown || state == State.AnimateUp;

    void Update()
    {
        PollPointerUp();
        DoAnimation();
    }

    void DoAnimation()
    {
        if (!IsAnimating) return;

        if (state == State.AnimateDown)
        {
            pressAmount += Time.deltaTime / animationDuration;
        }
        else if (state == State.AnimateUp)
        {
            pressAmount -= Time.deltaTime / animationDuration;
        }

        pressAmount = Mathf.Clamp01(pressAmount);
        var animationProgress                           = pressAmount;
        if (state == State.AnimateUp) animationProgress = 1 - animationProgress;
        animationProgress = animationCurve.Evaluate(animationProgress);
        if (state == State.AnimateUp) animationProgress = 1 - animationProgress;

        Animate(animationProgress);

        if (state == State.AnimateDown && pressAmount == 1)
        {
            state = State.Down;
        }

        if (state == State.AnimateUp && pressAmount == 0)
        {
            state = State.Up;
        }
    }

    protected void Press()
    {
        if (state != State.Down && state != State.AnimateDown)
        {
            OnWillPress();
            state = State.AnimateDown;
        }
    }

    protected void Release()
    {
        if (state != State.Up && state != State.AnimateUp)
        {
            OnWillRelease();
            state = State.AnimateUp;
        }
    }

    /// <summary>
    /// Pointer Up event does not fire on an object if it was not the one receive the Pointer Down event.
    /// </summary>
    void PollPointerUp()
    {
        if (useEnterExitEvents
         && (state == State.Down || state == State.AnimateDown)
         && !Input.GetMouseButton(0))
        {
            Release();
        }
    }

    /// <summary>
    /// NOP if not overrided
    /// </summary>
    /// <param name="visualPressAmount"><see cref="pressAmount"/> conformed to <see cref="animationCurve"/></param>
    protected virtual void Animate(float visualPressAmount) { }

    public void OnPointerDown(PointerEventData eventData)
    {
        Press();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Release();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (useEnterExitEvents && Input.GetMouseButton(0)) Press();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (useEnterExitEvents) Release();
    }

    protected virtual void OnWillPress()
    {
        willPress?.Invoke();
    }

    protected virtual void OnWillRelease()
    {
        willRelease?.Invoke();
    }
}
}
