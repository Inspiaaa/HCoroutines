namespace HCoroutines;

/// <summary>
/// Runs multiple coroutines one after another. Waits until the first
/// one has finished before starting the second one, ...
/// </summary>
public class SequentialCoroutine : CoroutineBase
{
    private readonly CoroutineBase[] coroutines;
    private int idx = 0;

    // If one of the steps completes while the game is paused, the next step is not instantly run.
    // Instead, it is only run once the game is resumed again, which is indicated by this flag.
    private bool isSwitchToNextCoroutinePending = false;

    public SequentialCoroutine(params CoroutineBase[] coroutines)
        : this(CoProcessMode.Inherit, CoRunMode.Inherit, coroutines) { }

    public SequentialCoroutine(
        CoProcessMode processMode,
        CoRunMode runMode,
        params CoroutineBase[] coroutines
    )
        : base(processMode, runMode)
    {
        this.coroutines = coroutines;
    }

    protected override void OnEnter()
    {
        if (coroutines.Length == 0)
        {
            Kill();
        }
    }

    protected override void OnStart()
    {
        StartCoroutine(coroutines[0]);
    }

    protected override void OnChildStopped(CoroutineBase child)
    {
        base.OnChildStopped(child);

        idx += 1;
        if (idx < coroutines.Length)
        {
            TryStartNextCoroutine();
        }
        else
        {
            Kill();
        }
    }

    private void TryStartNextCoroutine()
    {
        if (IsRunning)
        {
            StartCoroutine(coroutines[idx]);
        }
        else
        {
            isSwitchToNextCoroutinePending = true;
        }
    }

    protected override void OnResume()
    {
        if (isSwitchToNextCoroutinePending)
        {
            isSwitchToNextCoroutinePending = false;
            StartCoroutine(coroutines[idx]);
        }
    }
}