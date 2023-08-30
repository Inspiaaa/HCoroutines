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

        private void TryEnd()
        {
            if (Task.IsCompleted)
            {
                Kill();
            }
        }

        public override void OnEnter()
        {
            TryEnd();
            ResumeUpdates();
        }

        public override void Update()
        {
            TryEnd();
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

        private void TryEnd()
        {
            if (Task.IsCompleted)
            {
                Kill();
            }
        }

        public override void OnEnter()
        {
            base.OnEnter();
            TryEnd();
        }

        public override void Update()
        {
            TryEnd();
        }
    }
}