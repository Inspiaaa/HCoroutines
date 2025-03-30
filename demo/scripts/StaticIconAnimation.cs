using System.Collections;
using Godot;

namespace HCoroutines.Demo;

public partial class StaticIconAnimation : Node2D
{
    [Export] private Color pauseColor;
    [Export] private Color runColor;

    [Export] private float scaleFrequency = 2;

    [Export] private CoRunMode runMode;

    private Vector2 initialScale;

    private CoroutineBase coroutine;

    public override void _Ready()
    {
        initialScale = Scale;

        Co.Run(UpdateColor(), runMode: CoRunMode.Always);

        coroutine = Co.RepeatInfinitely(
            () => Co.Sequence(
                Co.Coroutine(AnimateSize()),
                Co.Tween(PlaySpinAnimation)
            ),
            runMode: runMode
        );
        Co.Run(coroutine);
    }

    private IEnumerator AnimateSize()
    {
        float time = 0;

        while (time < 4 * Mathf.Pi)
        {
            Scale = initialScale - initialScale * 0.25f * Mathf.Sin(time);
            time += Co.DeltaTime * scaleFrequency;
            yield return null;
        }
    }

    private void PlaySpinAnimation(Tween tween)
    {
        tween
            .TweenProperty(this, "rotation", Mathf.Pi * 4, duration: 4)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.InOut);

        tween.TweenCallback(Callable.From(() => Rotation = 0));
    }

    private IEnumerator UpdateColor()
    {
        while (true)
        {
            Modulate = coroutine.IsRunning ? runColor : pauseColor;
            yield return null;
        }
    }
}