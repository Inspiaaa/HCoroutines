using System.Threading.Tasks;

namespace HCoroutines;

/// <summary>
/// A coroutine that waits until an asynchronous task has been completed.
/// If the coroutine is killed before completion, the async task
/// will currently *not* be canceled.
/// </summary>
public class AwaitCoroutine<T> : CoroutineBase
{
    public readonly Task<T> Task;

    public AwaitCoroutine(Task<T> task)
    {
        this.Task = task;
    }

    public override void OnEnter()
    {
        // As the CoroutineManager class is not thread safe, ensure that Kill()
        // is executed on the main Godot thread.
        var godotTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        Task.ContinueWith(result => Kill(), godotTaskScheduler);
    }
}

/// <summary>
/// A coroutine that waits until an asynchronous task has been completed.
/// If the coroutine is killed before completion, the async task
/// will currently *not* be canceled.
/// </summary>
public class AwaitCoroutine : CoroutineBase
{
    public readonly Task Task;

    public AwaitCoroutine(Task task)
    {
        this.Task = task;
    }

    public override void OnEnter()
    {
        // As the CoroutineManager class is not thread safe, ensure that Kill()
        // is executed on the main Godot thread.
        var godotTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        Task.ContinueWith(result => Kill(), godotTaskScheduler);
    }
}