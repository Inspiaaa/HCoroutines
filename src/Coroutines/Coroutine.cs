using Godot;
using System;
using System.Collections;

namespace HCoroutines {
    /// <summary>
    /// Runs an IEnumerator as a coroutine (like Unity).
    /// If it returns null, it waits until the next frame before continuing
    /// the IEnumerator. If it returns another CoroutineBase, it will start it
    /// and wait until this new coroutine has finished before continuing the
    /// IEnumerator.
    /// </summary>
    public class Coroutine : CoroutineBase {
        private IEnumerator routine;

        public Coroutine(IEnumerator routine) {
            this.routine = routine;
        }

        public Coroutine(Func<Coroutine, IEnumerator> creator) {
            this.routine = creator(this);
        }

        public override void OnEnter()
        {
            base.OnEnter();
            if (routine == null) {
                Kill();
            }
        }

        public override void Update() {
            if (!routine.MoveNext()) {
                Kill();
                return;
            }

            object obj = routine.Current;

            // yield return null; => do nothing.
            if (obj == null) {
                return;
            }

            // yield return some coroutine; => Pause until the returned
            // coroutine is finished.
            if (obj is CoroutineBase childCoroutine) {
                StartCoroutine(childCoroutine);
                Pause();
                return;
            }

            // yield return some other enumerator; => Create new Coroutine
            // and pause until it is finished
            if (obj is IEnumerator childEnumerator) {
                StartCoroutine(new Coroutine(childEnumerator));
                Pause();
                return;
            }
        }

        public override void OnChildStop(CoroutineBase child) {
            base.OnChildStop(child);
            Resume();
        }
    }
}