using Godot;

namespace HCoroutines;

/// <summary>
/// Waits until a certain signal has been fired.
/// </summary>
public class WaitForSignalCoroutine : CoroutineBase
{
    private readonly GodotObject targetObject;
    private readonly string targetSignal;

    public WaitForSignalCoroutine(GodotObject obj, string signal)
        : base(CoProcessMode.Inherit, CoRunMode.Inherit)
    {
        this.targetObject = obj;
        this.targetSignal = signal;
    }

    protected override void OnEnter() {
        Manager.ToSignal(targetObject, targetSignal).OnCompleted(Kill);
    }
}