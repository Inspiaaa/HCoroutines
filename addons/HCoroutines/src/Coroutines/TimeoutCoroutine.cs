using HCoroutines.Util;

namespace HCoroutines;

/// <summary>
/// Runs a coroutine until completion or until a predefined duration has elapsed.
/// </summary>
public class TimeoutCoroutine : CoroutineBase
{
    private readonly float timeout;
    private readonly CoroutineBase coroutine;
    private readonly bool ignoreTimeScale;

    private PausableTimer timer;

    public TimeoutCoroutine(
        float timeout,
        CoroutineBase coroutine,
        bool ignoreTimeScale = false,
        CoProcessMode processMode = CoProcessMode.Inherit,
        CoRunMode runMode = CoRunMode.Inherit
    )
        : base(processMode, runMode)
    {
        this.timeout = timeout;
        this.coroutine = coroutine;
        this.ignoreTimeScale = ignoreTimeScale;
    }

    protected override void OnStart()
    {
        StartCoroutine(coroutine);

        if (IsAlive)
        {
            timer = new PausableTimer(Manager.GetTree(), timeout, ignoreTimeScale, callback: Kill);
        }
    }

    protected override void OnPause()
    {
        timer.Pause();
    }

    protected override void OnResume()
    {
        timer.Resume();
    }

    protected override void OnChildStopped(CoroutineBase child)
    {
        Kill();
    }
}