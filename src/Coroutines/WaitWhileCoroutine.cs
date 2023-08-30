using System;

namespace HCoroutines
{
    /// <summary>
    /// Waits while a certain condition is true.
    /// </summary>
    public class WaitWhileCoroutine : CoroutineBase
    {
        private Func<Boolean> condition;

        public WaitWhileCoroutine(Func<Boolean> condition)
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
            if (!this.condition())
            {
                Kill();
            }
        }
    }
}