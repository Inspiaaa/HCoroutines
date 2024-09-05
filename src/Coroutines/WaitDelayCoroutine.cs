using Godot;

namespace HCoroutines;

/// <summary>
/// Waits until a certain delay has passed.
/// </summary>
public class WaitDelayCoroutine : CoroutineBase
{
    private readonly float delay;
    
    private SceneTreeTimer timer;
    private float remainingTime;

    public WaitDelayCoroutine(float delay, CoRunMode runMode = CoRunMode.Inherit)
        : base(CoProcessMode.Inherit, runMode)
    {
        this.delay = delay;
    }

    protected override void OnStart()
    {
        timer = Manager.GetTree().CreateTimer(delay);
        timer.Timeout += Kill;
    }

    protected override void OnPause()
    {
        remainingTime = (float)timer.TimeLeft;
        timer.TimeLeft = double.MaxValue;
    }

    protected override void OnResume()
    {
        timer.TimeLeft = remainingTime;
    }
}