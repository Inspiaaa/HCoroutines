using Godot;
using System;

namespace HCoroutines;

/// <summary>
/// Base class of all coroutines that allows for pausing / resuming / killing / ... the coroutine.
/// It is also responsible for managing the hierarchical structure and organisation
/// of coroutine nodes.
/// The coroutines themselves act like a doubly linked list, so that
/// the list of children can be efficiently managed and even modified during iteration.
/// </summary>
public class CoroutineBase
{
    public CoroutineManager Manager;
    public CoroutineBase Parent;
    
    protected CoroutineBase firstChild, lastChild;
    protected CoroutineBase previousSibling, nextSibling;

    public bool IsAlive { get; private set; } = true;
    public bool IsPlaying { get; private set; } = false;

    // TODO: Add way to set this property.
    
    /// <summary>
    /// Determines whether the Update() method is called during process frames or physics frames.
    /// </summary>
    public CoProcessMode ProcessMode { get; private set; }

    public void StartCoroutine(CoroutineBase coroutine)
    {
        coroutine.Manager = Manager;
        coroutine.Parent = this;

        AddChild(coroutine);
        coroutine.OnEnter();
    }

    /// <summary>
    /// Called when the coroutine starts.
    /// </summary>
    public virtual void OnEnter()
    {
        if (this.ProcessMode == CoProcessMode.Inherit) 
        {
            this.ProcessMode = Parent?.ProcessMode ?? CoProcessMode.Normal;
        }    
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
    /// each frame. This is independent of the child coroutines.
    /// This method only works if the coroutine is still alive.
    /// </summary>
    public void ResumeUpdates()
    {
        if (!IsAlive)
        {
            throw new InvalidOperationException("Cannot resume updates on dead coroutine.");
        }

        IsPlaying = true;
        Manager.ActivateCoroutine(this);
    }

    /// <summary>
    /// Stops giving the coroutine Update() calls each frame.
    /// This is independent of the child coroutines.
    /// </summary>
    public void PauseUpdates()
    {
        IsPlaying = false;
        Manager.DeactivateCoroutine(this);
    }

    protected virtual void OnChildStopped(CoroutineBase child)
    {
        // If the parent coroutine is dead, then there is no reason to
        // manually remove the child coroutines.
        if (IsAlive)
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
        if (!IsAlive)
        {
            return;
        }

        try
        {
            OnExit();
        }
        catch (Exception e)
        {
            GD.PrintErr(e.ToString());
        }

        IsAlive = false;
        Manager.DeactivateCoroutine(this);

        CoroutineBase child = firstChild;
        while (child != null)
        {
            child.Kill();
            child = child.nextSibling;
        }

        Parent?.OnChildStopped(this);
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