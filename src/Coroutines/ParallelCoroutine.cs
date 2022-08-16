using Godot;
using System;
using System.Collections;

namespace HCoroutines {
    /// <summary>
    /// Runs multiple coroutines in parallel and exits once all have completed.
    /// </summary>
    public class ParallelCoroutine : CoroutineBase {
        private CoroutineBase[] coroutines;

        public ParallelCoroutine(params CoroutineBase[] coroutines) {
            this.coroutines = coroutines;
        }

        public override void OnEnter() {
            foreach (CoroutineBase coroutine in coroutines) {
                StartCoroutine(coroutine);
            }
        }

        public override void OnChildStop(CoroutineBase child) {
            base.OnChildStop(child);

            // If there are no more actively running coroutines, stop.
            if (firstChild == null) {
                Kill();
            }
        }
    }
}