using Godot;
using HCoroutines.Util;

namespace HCoroutines;

/// <summary>
/// Waits until a certain signal has been fired.
/// </summary>
public class WaitForSignalCoroutine : CoroutineBase
{
    private readonly GodotObject targetObject;
    private readonly string targetSignal;
    private int schedulerId;

    public WaitForSignalCoroutine(GodotObject obj, string signal)
    {
        this.targetObject = obj;
        this.targetSignal = signal;
    }

    public override void OnEnter() {
        Manager.ToSignal(targetObject, targetSignal).OnCompleted(Kill);
    }
}