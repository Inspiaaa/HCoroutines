using System;

namespace HCoroutines;

/// <summary>
/// Waits until a certain condition is true.
/// </summary>
public class WaitUntilCoroutine : CoroutineBase
{
    private readonly Func<Boolean> condition;

    public WaitUntilCoroutine(Func<Boolean> condition)
    {
        this.condition = condition;
    }

    protected override void OnEnter()
    {
        CheckCondition();
        if (IsAlive) ResumeUpdates();
    }

    public override void Update()
    {
        CheckCondition();
    }

    private void CheckCondition()
    {
        if (condition())
        {
            Kill();
        }
    }
}