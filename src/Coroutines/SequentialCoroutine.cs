namespace HCoroutines;

/// <summary>
/// Runs multiple coroutines one after another. Waits until the first
/// one has finished before starting the second one, ...
/// </summary>
public class SequentialCoroutine : CoroutineBase
{
    private readonly CoroutineBase[] coroutines;
    private int idx = 0;

    public SequentialCoroutine(params CoroutineBase[] coroutines)
    {
        this.coroutines = coroutines;
    }

    protected override void OnEnter()
    {
        if (coroutines.Length == 0)
        {
            Kill();
            return;
        }

        StartCoroutine(coroutines[0]);
    }

    protected override void OnChildStopped(CoroutineBase child)
    {
        base.OnChildStopped(child);

        idx += 1;
        if (idx < coroutines.Length)
        {
            StartCoroutine(coroutines[idx]);
        }
        else
        {
            Kill();
        }
    }
}