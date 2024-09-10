namespace HCoroutines;

/// <summary>
/// Runs multiple coroutines in parallel and exits as soon as any one of them finishes. 
/// </summary>
public class WaitForAnyCoroutine : CoroutineBase
{
    private readonly CoroutineBase[] coroutines;

    public WaitForAnyCoroutine(params CoroutineBase[] coroutines) 
        : this(CoProcessMode.Inherit, CoRunMode.Inherit, coroutines) { }
    
    public WaitForAnyCoroutine(
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

            if (!IsAlive)
            {
                return;
            }
        }
    }

    protected override void OnChildStopped(CoroutineBase child)
    {
        Kill();
    }
}