using Godot;
using System.Collections;
using System.Threading.Tasks;

namespace HCoroutines.Demo;

public partial class DynamicIconAnimation : Node2D
{
    public override void _Ready()
    {
        Co.Run(PlayAnimation());
    }

    private IEnumerator PlayAnimation()
    {
        // Go to the center of the screen.
        Vector2 center = GetViewportRect().Size / 2;
        Rotation = GetAngleTo(center);
        yield return GoToPosition(center, speed: 100);
        
        // Wait for a second.
        yield return Co.Wait(1);

        // Rotate towards the mouse.
        yield return Co.Parallel(
            Co.Coroutine(LookAtMouse(0.1f, 0.05f)),
            Co.Tween(tween => ChangeColorToRed(tween, 0.5f))
        );

        // Follow the mouse.
        yield return FollowMouse(speed: 250, minDistance: 30);

        // Play an explosion animation.
        yield return Co.Tween(Explode);

        // Simulate some async action, e.g. making a network request or writing to file.
        yield return Co.Await(UpdateScoreboard());
        
        // End.
        QueueFree();
    }

    private IEnumerator GoToPosition(Vector2 targetPosition, float speed)
    {
        while (Position.DistanceTo(targetPosition) > 0.1f)
        {
            Position = Position.MoveToward(targetPosition, speed * Co.DeltaTime);
            yield return null;
        }
    }

    private IEnumerator LookAtMouse(float speed, float tolerance)
    {
        float targetAngle;
        do
        {
            Vector2 mouse = GetGlobalMousePosition();
            targetAngle = Position.AngleToPoint(mouse);
            
            Rotation = Mathf.LerpAngle(Rotation, targetAngle, 1 - Mathf.Pow(speed, Co.DeltaTime));
            
            yield return null;
        } while (Mathf.Abs(Mathf.AngleDifference(Rotation, targetAngle)) > tolerance);
    }

    private IEnumerator FollowMouse(float speed, float minDistance)
    {
        Vector2 mouse;
        while (Position.DistanceTo(mouse = GetGlobalMousePosition()) > minDistance)
        {
            Position = Position.MoveToward(mouse, speed * Co.DeltaTime);
            Rotation = Position.AngleToPoint(mouse);
            yield return null;
        }
    }

    private void ChangeColorToRed(Tween tween, float duration)
    {
        tween.TweenProperty(this, "modulate", new Color(1, 0, 0), duration);
    }

    private void Explode(Tween tween)
    {
        float duration = 0.125f;
        
        tween.TweenProperty(this, "modulate", Modulate with { A = 0 }, duration)
            .SetEase(Tween.EaseType.In)
            .SetTrans(Tween.TransitionType.Cubic);
        
        tween.Parallel();
        
        tween.TweenProperty(this, "scale", Scale * 5, duration)
            .SetEase(Tween.EaseType.In)
            .SetTrans(Tween.TransitionType.Cubic);
    }

    private async Task<int> UpdateScoreboard()
    {
        await Task.Delay(100);
        return 0;
    }
}