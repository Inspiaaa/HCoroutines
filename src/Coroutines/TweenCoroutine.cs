using Godot;
using System;
using HCoroutines.Util;

namespace HCoroutines;

/// <summary>
/// A coroutine that manages a tween instance.
/// When the tween is finished, the coroutine also finishes.
/// If the coroutine is killed before that, it also kills the tween instance.
/// </summary>
public class TweenCoroutine : CoroutineBase
{
    private readonly Func<Tween> createTween;
    private Tween tween;
    private int schedulerId;

    public TweenCoroutine(Func<Tween> createTween)
    {
        this.createTween = createTween;
    }

    public override void OnEnter()
    {
        tween = createTween();
        
        if (!tween.IsValid() || !tween.IsRunning()) {
            Kill();
            return;
        }

        tween.Finished += Kill;
    }

    public override void OnExit()
    {
        tween.Kill();
    }
}