using Godot;
using System;

namespace HCoroutines {
    /// <summary>
    /// Base class of all coroutines that allows for pausing / resuming / killing / ... the coroutine.
    /// </summary>
    public class CoroutineBase : HCoroutineNode, ICoroutineStopListener {
        public CoroutineManager manager;
        public bool isAlive = true;
        public bool isPlaying = false;
        public ICoroutineStopListener stopListener;

        public void StartCoroutine(CoroutineBase coroutine) {
            coroutine.stopListener = this;
            coroutine.manager = manager;

            AddChild(coroutine);
            coroutine.OnEnter();
        }

        /// <summary>
        /// Called when the coroutine starts.
        /// </summary>
        public virtual void OnEnter() {
            Resume();
        }

        /// <summary>
        /// Called every frame if the coroutine is playing.
        /// </summary>
        public virtual void Update() { }

        /// <summary>
        /// Called when the coroutine is killed.
        /// </summary>
        public virtual void OnExit() { }

        /// <summary>
        /// Starts playing this coroutine, meaning that it will receive Update() calls
        /// each frame.
        /// This is independent of the child coroutines.
        /// </summary>
        public void Resume() {
            isPlaying = true;
            manager.ActivateCoroutine(this);
        }

        /// <summary>
        /// Stops giving the coroutine Update() calls each frame.
        /// This is independent of the child coroutines.
        /// </summary>
        public void Pause() {
            isPlaying = false;
            manager.DeactivateCoroutine(this);
        }

        public virtual void OnChildStop(CoroutineBase child) {
            // If the parent coroutine is dead, then there is no reason to
            // manually remove the child coroutines
            if (isAlive) {
                RemoveChild(child);
            }
        }

        /// <summary>
        /// Kills this coroutine and all child coroutines that were started using
        /// StartCoroutine(...) on this coroutine.
        /// </summary>
        public void Kill() {
            try {
                OnExit();
            }
            catch (Exception e) {
                GD.PrintErr(e.ToString());
            }

            isAlive = false;
            manager.DeactivateCoroutine(this);

            CoroutineBase child = firstChild;
            while (child != null) {
                child.Kill();
                child = child.nextSibling;
            }

            stopListener?.OnChildStop(this);
        }
    }
}
