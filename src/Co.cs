using System;
using System.Collections;
using System.Threading.Tasks;
using Godot;

// Hierarchical Coroutines v1.0 for Godot
// by @Inspiaaa

namespace HCoroutines;

/// <summary>
/// Class that allows for easy access to the standard coroutine types.
/// </summary>
public static class Co
{
    private static Coroutine[] GetCoroutines(IEnumerator[] enumerators)
    {
        Coroutine[] coroutines = new Coroutine[enumerators.Length];

        for (int i = 0; i < enumerators.Length; i++)
        {
            coroutines[i] = new Coroutine(enumerators[i]);
        }

        return coroutines;
    }

    
    public static float DeltaTime => CoroutineManager.Instance.DeltaTime;
    public static double DeltaTimeDouble => CoroutineManager.Instance.DeltaTimeDouble;
    
    public static float PhysicsDeltaTime => CoroutineManager.Instance.PhysicsDeltaTime;
    public static double PhysicsDeltaTimeDouble => CoroutineManager.Instance.PhysicsDeltaTimeDouble;


    public static void Run(CoroutineBase coroutine)
        => CoroutineManager.Instance.StartCoroutine(coroutine);

    public static Coroutine Run(
        IEnumerator coroutine, 
        CoProcessMode processMode = CoProcessMode.Inherit, 
        CoRunMode runMode = CoRunMode.Inherit)
    {
        Coroutine co = new Coroutine(coroutine, processMode, runMode);
        CoroutineManager.Instance.StartCoroutine(co);
        return co;
    }

    public static Coroutine Run(
        Func<IEnumerator> creator,
        CoProcessMode processMode = CoProcessMode.Inherit,
        CoRunMode runMode = CoRunMode.Inherit)
    {
        Coroutine co = new Coroutine(creator(), processMode, runMode);
        CoroutineManager.Instance.StartCoroutine(co);
        return co;
    }

    public static Coroutine Run(
        Func<Coroutine, IEnumerator> creator,
        CoProcessMode processMode = CoProcessMode.Inherit,
        CoRunMode runMode = CoRunMode.Inherit)
    {
        Coroutine coroutine = new Coroutine(creator, processMode, runMode);
        CoroutineManager.Instance.StartCoroutine(coroutine);
        return coroutine;
    }


    public static Coroutine Coroutine(
            IEnumerator enumerator,
            CoProcessMode processMode = CoProcessMode.Inherit,
            CoRunMode runMode = CoRunMode.Inherit)
        => new Coroutine(enumerator, processMode, runMode);

    public static Coroutine Coroutine(
            Func<Coroutine, IEnumerator> creator,
            CoProcessMode processMode = CoProcessMode.Inherit,
            CoRunMode runMode = CoRunMode.Inherit)
        => new Coroutine(creator, processMode, runMode);


    public static ParallelCoroutine Parallel(params IEnumerator[] enumerators)
        => new ParallelCoroutine(GetCoroutines(enumerators));
    
    public static ParallelCoroutine Parallel(
            CoProcessMode processMode,
            CoRunMode runMode, 
            params IEnumerator[] enumerators)
        => new ParallelCoroutine(processMode, runMode, GetCoroutines(enumerators));

    public static ParallelCoroutine Parallel(params CoroutineBase[] coroutines)
        => new ParallelCoroutine(coroutines);
    
    public static ParallelCoroutine Parallel(
            CoProcessMode processMode,
            CoRunMode runMode, 
            params CoroutineBase[] coroutines)
        => new ParallelCoroutine(processMode, runMode, coroutines);


    public static SequentialCoroutine Sequence(params IEnumerator[] enumerators)
        => new SequentialCoroutine(GetCoroutines(enumerators));
    
    public static SequentialCoroutine Sequence(
            CoProcessMode processMode,
            CoRunMode runMode, 
            params IEnumerator[] enumerators)
        => new SequentialCoroutine(processMode, runMode, GetCoroutines(enumerators));

    public static SequentialCoroutine Sequence(params CoroutineBase[] coroutines)
        => new SequentialCoroutine(coroutines);
    
    public static SequentialCoroutine Sequence(
            CoProcessMode processMode,
            CoRunMode runMode, 
            params CoroutineBase[] coroutines)
        => new SequentialCoroutine(processMode, runMode, coroutines);


    public static WaitDelayCoroutine Wait(float delay, bool ignoreTimeScale = false, CoRunMode runMode = CoRunMode.Inherit)
        => new WaitDelayCoroutine(delay, ignoreTimeScale, runMode);
    
    public static WaitDelayCoroutine Sleep(float delay, bool ignoreTimeScale = false, CoRunMode runMode = CoRunMode.Inherit)
        => new WaitDelayCoroutine(delay, ignoreTimeScale, runMode);
    
    public static WaitDelayCoroutine Delay(float delay, bool ignoreTimeScale = false, CoRunMode runMode = CoRunMode.Inherit)
        => new WaitDelayCoroutine(delay, ignoreTimeScale, runMode);


    public static WaitWhileCoroutine WaitWhile(
            Func<Boolean> condition,
            CoProcessMode processMode = CoProcessMode.Inherit,
            CoRunMode runMode = CoRunMode.Inherit)
        => new WaitWhileCoroutine(condition, processMode, runMode);


    public static WaitUntilCoroutine WaitUntil(
            Func<Boolean> condition,
            CoProcessMode processMode = CoProcessMode.Inherit,
            CoRunMode runMode = CoRunMode.Inherit)
        => new WaitUntilCoroutine(condition, processMode, runMode);


    public static WaitForSignalCoroutine WaitForSignal(GodotObject obj, string signal)
        => new WaitForSignalCoroutine(obj, signal);



    public static RepeatCoroutine Repeat(
            int times, 
            Func<RepeatCoroutine, CoroutineBase> creator,
            CoProcessMode processMode = CoProcessMode.Inherit,
            CoRunMode runMode = CoRunMode.Inherit)
        => new RepeatCoroutine(times, creator, processMode, runMode);

    public static RepeatCoroutine Repeat(
            int times, 
            Func<CoroutineBase> creator,
            CoProcessMode processMode = CoProcessMode.Inherit,
            CoRunMode runMode = CoRunMode.Inherit)
        => new RepeatCoroutine(times, creator, processMode, runMode);

    public static RepeatCoroutine Repeat(
            int times, 
            Func<RepeatCoroutine, IEnumerator> creator,
            CoProcessMode processMode = CoProcessMode.Inherit,
            CoRunMode runMode = CoRunMode.Inherit)
        => new RepeatCoroutine(times, coroutine => new Coroutine(creator(coroutine)), processMode, runMode);

    public static RepeatCoroutine Repeat(
            int times, 
            Func<IEnumerator> creator,
            CoProcessMode processMode = CoProcessMode.Inherit,
            CoRunMode runMode = CoRunMode.Inherit)
        => new RepeatCoroutine(times, () => new Coroutine(creator()), processMode, runMode);


    public static RepeatCoroutine RepeatInfinitely(
            Func<RepeatCoroutine, CoroutineBase> creator,
            CoProcessMode processMode = CoProcessMode.Inherit,
            CoRunMode runMode = CoRunMode.Inherit)
        => new RepeatCoroutine(-1, creator, processMode, runMode);

    public static RepeatCoroutine RepeatInfinitely(
            Func<CoroutineBase> creator,
            CoProcessMode processMode = CoProcessMode.Inherit,
            CoRunMode runMode = CoRunMode.Inherit)
        => new RepeatCoroutine(-1, creator, processMode, runMode);

    public static RepeatCoroutine RepeatInfinitely(
            Func<RepeatCoroutine, IEnumerator> creator,
            CoProcessMode processMode = CoProcessMode.Inherit,
            CoRunMode runMode = CoRunMode.Inherit)
        => new RepeatCoroutine(-1, coroutine => new Coroutine(creator(coroutine)), processMode, runMode);

    public static RepeatCoroutine RepeatInfinitely(
            Func<IEnumerator> creator,
            CoProcessMode processMode = CoProcessMode.Inherit,
            CoRunMode runMode = CoRunMode.Inherit)
        => new RepeatCoroutine(-1, () => new Coroutine(creator()), processMode, runMode);


    public static TweenCoroutine Tween(Action<Tween> setupTween)
        => new TweenCoroutine(setupTween);


    public static AwaitCoroutine<T> Await<T>(Task<T> task, CoRunMode runMode = CoRunMode.Inherit)
        => new AwaitCoroutine<T>(task, runMode);

    public static AwaitCoroutine Await(Task task, CoRunMode runMode = CoRunMode.Inherit)
        => new AwaitCoroutine(task, runMode);
}