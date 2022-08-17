using HCoroutines.Util;

namespace HCoroutines
{
    /// <summary>
    /// Waits until a certain delay has passed.
    /// </summary>
    public class WaitDelayCoroutine : CoroutineBase
    {
        private float delay;
        private int schedulerId;

        public WaitDelayCoroutine(float delay)
        {
            this.delay = delay;
        }

        public override void OnEnter()
        {
            schedulerId = TimeScheduler.Instance.Schedule(Kill, delay);
        }

        public override void OnExit()
        {
            TimeScheduler.Instance.CancelSchedule(schedulerId);
        }
    }
}