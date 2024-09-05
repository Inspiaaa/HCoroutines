namespace HCoroutines;

/// <summary>
/// Waits until a certain delay has passed.
/// </summary>
public class WaitDelayCoroutine : CoroutineBase
{
    private readonly float delay;

    public WaitDelayCoroutine(float delay, CoRunMode runMode = CoRunMode.Inherit)
        : base(CoProcessMode.Inherit, runMode)
    {
        this.delay = delay;
    }

    protected override void OnEnter()
    {
        // TODO: Implement pause logic.
        // TODO: Expose option to use timescale
        Manager.GetTree().CreateTimer(delay).Timeout += Kill;
    }
}