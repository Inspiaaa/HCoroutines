using Godot;
using System;

namespace HCoroutines;

/// <summary>
/// A coroutine that manages a tween instance.
/// When the tween is finished, the coroutine also finishes.
/// If the coroutine is killed before that, it also kills the tween instance.
/// </summary>
public class TweenCoroutine : CoroutineBase
{
    private readonly Action<Tween> setupTween;
    private Tween tween;
    
    public TweenCoroutine(Action<Tween> setupTween, CoRunMode runMode = CoRunMode.Inherit)
        : base(CoProcessMode.Inherit, runMode)
    {
        this.setupTween = setupTween;
    }

    protected override void OnStart()
    {
        tween = Manager.CreateTween();
        setupTween(tween);
        
        if (!tween.IsValid() || !tween.IsRunning()) {
            Kill();
            return;
        }

        tween.Finished += Kill;
    }

    protected override void OnPause()
    {
        tween.Pause();
    }

    protected override void OnResume()
    {
        tween.Play();
    }

    protected override void OnExit()
    {
        tween.Kill();
    }
}