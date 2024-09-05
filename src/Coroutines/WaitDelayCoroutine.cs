namespace HCoroutines;

/// <summary>
/// Waits until a certain delay has passed.
/// </summary>
public class WaitDelayCoroutine : CoroutineBase
{
    private readonly float delay;

    public WaitDelayCoroutine(float delay)
    {
        this.delay = delay;
    }

    protected override void OnEnter()
    {
        // TODO: Implement pause logic.
        Manager.GetTree().CreateTimer(delay).Timeout += Kill;
    }
}