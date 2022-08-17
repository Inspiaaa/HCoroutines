using Godot;
using System;
using System.Collections;
using HCoroutines.Util;

namespace HCoroutines
{
    /// <summary>
    /// Waits until a certain signal has been fired.
    /// </summary>
    public class WaitForSignalCoroutine : CoroutineBase
    {
        private Godot.Object targetObject;
        private string targetSignal;
        private int schedulerId;

        public WaitForSignalCoroutine(Godot.Object obj, string signal)
        {
            this.targetObject = obj;
            this.targetSignal = signal;
        }

        public override void OnEnter()
        {
            schedulerId = TimeScheduler.Instance.ScheduleOnSignal(Kill, targetObject, targetSignal);
        }

        public override void OnExit()
        {
            TimeScheduler.Instance.CancelSchedule(schedulerId);
        }
    }
}