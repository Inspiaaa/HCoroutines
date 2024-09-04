using System;

namespace HCoroutines;

/// <summary>
/// Runs a coroutine multiple times. Each time the coroutine is finished,
/// it is restarted.
/// </summary>
public class RepeatCoroutine : CoroutineBase
{
    private readonly int repeatTimes;
    private int currentRepeatCount;
    private readonly Func<RepeatCoroutine, CoroutineBase> coroutineCreator;

    private bool IsInfinite => repeatTimes == -1;

    public RepeatCoroutine(int repeatTimes, Func<RepeatCoroutine, CoroutineBase> coroutineCreator)
    {
        this.repeatTimes = repeatTimes;
        this.coroutineCreator = coroutineCreator;
    }

    public override void OnEnter()
    {
        if (repeatTimes == 0)
        {
            Kill();
            return;
        }

        Repeat();
    }

    private void Repeat()
    {
        currentRepeatCount += 1;
        CoroutineBase coroutine = coroutineCreator.Invoke(this);
        StartCoroutine(coroutine);
    }

    protected override void OnChildStopped(CoroutineBase child)
    {
        base.OnChildStopped(child);

        if (!IsInfinite && currentRepeatCount > repeatTimes)
        {
            Kill();
            return;
        }

        Repeat();
    }
}