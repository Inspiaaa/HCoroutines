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
    private readonly Func<CoroutineBase> coroutineCreator;

    private bool IsInfinite => repeatTimes == -1;

    // If the coroutine that is being repeated completes while the game is paused, it is not instantly restarted.
    // Instead, it is restarted once the game is resumed, which is indicated by this flag.
    private bool isRepeatPending;
    
    public RepeatCoroutine(
        int repeatTimes, 
        Func<RepeatCoroutine, CoroutineBase> coroutineCreator,
        CoProcessMode processMode = CoProcessMode.Inherit, 
        CoRunMode runMode = CoRunMode.Inherit
    )
        : base(processMode, runMode)
    {
        this.repeatTimes = repeatTimes;
        this.coroutineCreator = () => coroutineCreator(this);
    }
    
    public RepeatCoroutine(
        int repeatTimes, 
        Func<CoroutineBase> coroutineCreator,
        CoProcessMode processMode = CoProcessMode.Inherit, 
        CoRunMode runMode = CoRunMode.Inherit
    )
        : base(processMode, runMode)
    {
        this.repeatTimes = repeatTimes;
        this.coroutineCreator = coroutineCreator;
    }

    protected override void OnEnter()
    {
        if (repeatTimes == 0)
        {
            Kill();
        }
    }

    protected override void OnStart()
    {
        Repeat();
    }

    private void Repeat()
    {
        currentRepeatCount += 1;
        CoroutineBase coroutine = coroutineCreator();
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

        TryRepeat();
    }

    private void TryRepeat()
    {
        if (IsRunning)
        {
            Repeat();
        }
        else
        {
            isRepeatPending = true;
        }
    }
    
    protected override void OnResume()
    {
        if (isRepeatPending)
        {
            isRepeatPending = false;
            Repeat();
        }
    }
}