using System;

namespace HCoroutines;

/// <summary>
/// Waits until a certain condition is true.
/// </summary>
public class WaitUntilCoroutine : CoroutineBase
{
    private readonly Func<Boolean> condition;

    public WaitUntilCoroutine(
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
        if (condition())
        {
            Kill();
        }
    }
}