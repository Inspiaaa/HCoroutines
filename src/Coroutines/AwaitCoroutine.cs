using System;
using System.Collections;
using System.Threading.Tasks;

namespace HCoroutines
{
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

        // TODO: Implement for Task that does not return a value.

        public override void OnEnter()
        {
            base.OnEnter();
            if (Task.IsCompleted)
            {
                Kill();
            }
        }

        public override void Update()
        {
            if (Task.IsCompleted)
            {
                Kill();
            }
        }
    }
}