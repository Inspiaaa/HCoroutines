using Godot;
using System;

namespace HCoroutines
{
    /// <summary>
    /// Base class of all coroutines that allows for pausing / resuming / killing / ... the coroutine.
    /// It is also responsible for managing the hierarchical structure and organisation
    /// of coroutine nodes.
    /// The coroutines themselves act like a doubly linked list, so that
    /// the list of children can be efficiently managed and even modified during iteration.
    /// </summary>
    public class CoroutineBase : ICoroutineStopListener
    {
        public CoroutineManager manager;
        public ICoroutineStopListener stopListener;

        protected CoroutineBase firstChild, lastChild;
        protected CoroutineBase previousSibling, nextSibling;

        public bool isAlive = true;
        public bool isPlaying = false;

        public void StartCoroutine(CoroutineBase coroutine)
        {
            coroutine.stopListener = this;
            coroutine.manager = manager;

            AddChild(coroutine);
            coroutine.OnEnter();
        }

        /// <summary>
        /// Called when the coroutine starts.
        /// </summary>
        public virtual void OnEnter()
        {
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
        public void Resume()
        {
            isPlaying = true;
            manager.ActivateCoroutine(this);
        }

        /// <summary>
        /// Stops giving the coroutine Update() calls each frame.
        /// This is independent of the child coroutines.
        /// </summary>
        public void Pause()
        {
            isPlaying = false;
            manager.DeactivateCoroutine(this);
        }

        public virtual void OnChildStop(CoroutineBase child)
        {
            // If the parent coroutine is dead, then there is no reason to
            // manually remove the child coroutines
            if (isAlive)
            {
                RemoveChild(child);
            }
        }

        /// <summary>
        /// Kills this coroutine and all child coroutines that were started using
        /// StartCoroutine(...) on this coroutine.
        /// </summary>
        public void Kill()
        {
            try
            {
                OnExit();
            }
            catch (Exception e)
            {
                GD.PrintErr(e.ToString());
            }

            isAlive = false;
            manager.DeactivateCoroutine(this);

            CoroutineBase child = firstChild;
            while (child != null)
            {
                child.Kill();
                child = child.nextSibling;
            }

            stopListener?.OnChildStop(this);
        }

        /// <summary>
        /// Adds a coroutine as a child.
        /// </summary>
        protected void AddChild(CoroutineBase coroutine)
        {
            if (firstChild == null)
            {
                firstChild = coroutine;
                lastChild = coroutine;
            }
            else
            {
                lastChild.nextSibling = coroutine;
                coroutine.previousSibling = lastChild;
                lastChild = coroutine;
            }
        }

        /// <summary>
        /// Removes a child from the list of child coroutines.
        /// </summary>
        protected void RemoveChild(CoroutineBase coroutine)
        {
            if (coroutine.previousSibling != null)
            {
                coroutine.previousSibling.nextSibling = coroutine.nextSibling;
            }

            if (coroutine.nextSibling != null)
            {
                coroutine.nextSibling.previousSibling = coroutine.previousSibling;
            }

            if (firstChild == coroutine)
            {
                firstChild = coroutine.nextSibling;
            }

            if (lastChild == coroutine)
            {
                lastChild = coroutine.previousSibling;
            }
        }
    }
}
