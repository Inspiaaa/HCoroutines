using Godot;
using System;
using System.Collections;

namespace HCoroutines;

/// <summary>
/// Runs an IEnumerator as a coroutine (like Unity).
/// If it returns null, it waits until the next frame before continuing
/// the IEnumerator. If it returns another CoroutineBase, it will start it
/// and wait until this new coroutine has finished before continuing the
/// IEnumerator.
/// </summary>
public class Coroutine : CoroutineBase
{
    private readonly IEnumerator routine;

    public Coroutine(
        IEnumerator routine,
        CoProcessMode processMode = CoProcessMode.Inherit,
        CoRunMode runMode = CoRunMode.Inherit
    )
        : base(processMode, runMode)
    {
        this.routine = routine;
    }

    public Coroutine(
        Func<Coroutine, IEnumerator> creator,
        CoProcessMode processMode = CoProcessMode.Inherit,
        CoRunMode runMode = CoRunMode.Inherit
    )
        : base(processMode, runMode)
    {
        this.routine = creator(this);
    }

    protected override void OnEnter()
    {
        if (routine == null)
        {
            Kill();
        }
    }

    protected override void OnStart()
    {
        EnableUpdates();
    }

    public override void Update()
    {
        if (!routine.MoveNext())
        {
            Kill();
            return;
        }

        object obj = routine.Current;

        // yield return null; => do nothing.
        if (obj is null)
        {
            return;
        }

        // yield return some coroutine; => Pause until the returned
        // coroutine is finished.
        if (obj is CoroutineBase childCoroutine)
        {
            // It's important to disable updates before starting the child coroutine.
            // Otherwise, if the child coroutine instantly terminates, which would
            // lead to this coroutine resuming updates, it would disable updates to this coroutine.
            // That would not be correct.
            DisableUpdates();
            StartCoroutine(childCoroutine);
            return;
        }

        // yield return some other enumerator; => Create new Coroutine
        // and pause until it is finished
        if (obj is IEnumerator childEnumerator)
        {
            DisableUpdates();
            StartCoroutine(new Coroutine(childEnumerator));
            return;
        }

        GD.PushWarning($"Invalid yield return value: '{obj}'. See the docs for allowed return values.");
    }

    protected override void OnChildStopped(CoroutineBase child)
    {
        base.OnChildStopped(child);
        EnableUpdates();
    }
}