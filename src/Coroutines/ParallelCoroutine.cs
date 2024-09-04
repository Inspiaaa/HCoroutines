namespace HCoroutines;

/// <summary>
/// Runs multiple coroutines in parallel and exits once all have completed.
/// </summary>
public class ParallelCoroutine : CoroutineBase
{
    private readonly CoroutineBase[] coroutines;

    public ParallelCoroutine(params CoroutineBase[] coroutines)
    {
        this.coroutines = coroutines;
    }

    public override void OnEnter()
    {
        if (coroutines.Length == 0)
        {
            Kill();
            return;
        }

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