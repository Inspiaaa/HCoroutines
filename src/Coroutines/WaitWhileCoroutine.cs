using System;

namespace HCoroutines;

/// <summary>
/// Waits while a certain condition is true.
/// </summary>
public class WaitWhileCoroutine : CoroutineBase
{
    private readonly Func<Boolean> condition;

    public WaitWhileCoroutine(Func<Boolean> condition)
    {
        this.condition = condition;
    }

    protected override void OnEnter()
    {
        CheckCondition();
        if (IsAlive) EnableUpdates();
    }

    public override void Update()
    {
        CheckCondition();
    }

    private void CheckCondition()
    {
        if (!condition())
        {
            Kill();
        }
    }
}