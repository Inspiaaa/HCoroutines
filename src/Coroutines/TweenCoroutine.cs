using Godot;
using System;
using HCoroutines.Util;

namespace HCoroutines
{
    /// <summary>
    /// A coroutine that manages a tween instance.
    /// When the tween is finished, the coroutine also finishes.
    /// If the coroutine is killed before that, it also kills the tween instance.
    /// </summary>
    public class TweenCoroutine : CoroutineBase
    {
        private Action<SceneTreeTween> setupTween;
        private SceneTreeTween tween;
        private int schedulerId;

        public TweenCoroutine(Action<SceneTreeTween> setupTween)
        {
            this.setupTween = setupTween;
        }

        public override void OnEnter()
        {
            tween = manager.CreateTween();
            setupTween(tween);
            schedulerId = TimeScheduler.Instance.ScheduleOnSignal(Kill, tween, "finished");
        }

        public override void OnExit()
        {
            tween.Kill();
            TimeScheduler.Instance.CancelSchedule(schedulerId);
        }
    }
}