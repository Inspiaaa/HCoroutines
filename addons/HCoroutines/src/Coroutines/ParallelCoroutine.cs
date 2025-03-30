namespace HCoroutines;

/// <summary>
/// Runs multiple coroutines in parallel and exits once all have completed.
/// </summary>
public class ParallelCoroutine : CoroutineBase
{
    private readonly CoroutineBase[] coroutines;

    public ParallelCoroutine(params CoroutineBase[] coroutines)
        : this(CoProcessMode.Inherit, CoRunMode.Inherit, coroutines) { }

    public ParallelCoroutine(
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
        foreach (CoroutineBase coroutine in coroutines)
        {
            StartCoroutine(coroutine);
        }
    }

    protected override void OnChildStopped(CoroutineBase child)
    {
        base.OnChildStopped(child);

        // If there are no more actively running coroutines, stop.
        if (firstChild == null)
        {
            Kill();
        }
    }
}