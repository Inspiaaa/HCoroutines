using System;

namespace HCoroutines;

/// <summary>
/// Waits while a certain condition is true.
/// </summary>
public class WaitWhileCoroutine : CoroutineBase
{
    private readonly Func<Boolean> condition;

    public WaitWhileCoroutine(
        Func<Boolean> condition, 
        CoProcessMode processMode = CoProcessMode.Inherit, 
        CoRunMode runMode = CoRunMode.Inherit
    )
        : base(processMode, runMode)
    {
        this.condition = condition;
    }

    protected override void OnStart()
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