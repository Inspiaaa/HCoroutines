using System;

namespace HCoroutines
{
    /// <summary>
    /// Waits until a certain condition is true.
    /// </summary>
    public class WaitUntilCoroutine : CoroutineBase
    {
        private Func<Boolean> condition;

        public WaitUntilCoroutine(Func<Boolean> condition)
        {
            this.condition = condition;
        }

        public override void OnEnter()
        {
            CheckCondition();
            if (isAlive) ResumeUpdates();
        }

        public override void Update()
        {
            CheckCondition();
        }

        private void CheckCondition()
        {
            if (this.condition())
            {
                Kill();
            }
        }
    }
}