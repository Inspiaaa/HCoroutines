using Godot;
using System;
using System.Collections;
using HCoroutines;

public class IconDemo : Node2D
{
    public override void _Ready()
    {
        Co.Run(DoAnimation());
    }

    private IEnumerator DoAnimation() {
        yield return MoveToPosition(new Vector2(0, 0), 2);

        yield return FullRotation(2);

        yield return Co.Wait(1);

        yield return Co.Parallel(
            MoveToPosition(new Vector2(150, 150), 2),
            ChangeColor(new Color(1, 0, 0), 2)
        );

        yield return Co.Wait(1);

        yield return Co.Repeat(5, () => FullRotation(0.25f));

        yield return ChangeColor(new Color(0, 0, 0, 0), 1);
        QueueFree();
    }

    private IEnumerator MoveToPosition(Vector2 targetPos, float duration) {
        float speed = Position.DistanceTo(targetPos) / duration;

        while (Position.DistanceTo(targetPos) > 0.1f) {
            float movement = speed * Co.DeltaTime;
            Position = Position.MoveToward(targetPos, movement);
            yield return null;
        }
    }

    private IEnumerator FullRotation(float duration) {
        float angle = 0;
        float speed = 2 * Mathf.Pi / duration;

        while (angle < 2 * Mathf.Pi) {
            angle += speed * Co.DeltaTime;
            Rotation = angle;
            yield return null;
        }
        Rotation = 0;
    }

    private IEnumerator ChangeColor(Color targetColor, float duration) {
        SceneTreeTween tween = CreateTween();
        tween
            .TweenProperty(this, "modulate", targetColor, duration)
            .SetTrans(Tween.TransitionType.Expo);

        yield return Co.WaitForSignal(tween, "finished");
    }
}
