using Godot;
using HCoroutines.Util;

namespace HCoroutines;

/// <summary>
/// Waits until a certain delay has passed.
/// </summary>
public class WaitDelayCoroutine : CoroutineBase
{
    private readonly float delay;
    private readonly bool ignoreTimeScale;

    private PausableTimer timer;

    public WaitDelayCoroutine(float delay, bool ignoreTimeScale = false, CoRunMode runMode = CoRunMode.Inherit)
        : base(CoProcessMode.Inherit, runMode)
    {
        this.delay = delay;
        this.ignoreTimeScale = ignoreTimeScale;
    }

    protected override void OnStart()
    {
        timer = new PausableTimer(Manager.GetTree(), delay, ignoreTimeScale, callback: Kill);
    }

    protected override void OnPause()
    {
        timer.Pause();
    }

    protected override void OnResume()
    {
        timer.Resume();
    }
}