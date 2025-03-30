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
    public bool IsRunning { get; private set; } = false;
    public bool ShouldReceiveUpdates { get; private set; } = false;

    private bool hasCalledStart = false;
    private bool wasReceivingUpdatesBeforePause = false;

    /// <summary>
    /// Determines whether the Update() method is called during process frames or physics frames.
    /// </summary>
    public CoProcessMode ProcessMode { get; private set; }

    /// <summary>
    /// Determines the pause behaviour of this coroutine.
    /// </summary>
    public CoRunMode RunMode { get; private set; }

    // The ProcessMode and RunMode fields should be treated as readonly, as they are not and should not be changed
    // after construction of the coroutine. They are not declared as readonly, as the `Inherit` mode has to be
    // translated into an actual mode, which is only known when the coroutine is added to the hierarchy, not when
    // the constructor is called.

    /// <summary>
    /// Event that is fired when this coroutine is killed (stops).
    /// </summary>
    public event Action Stopped;

    public CoroutineBase(CoProcessMode processMode, CoRunMode runMode)
    {
        this.ProcessMode = processMode;
        this.RunMode = runMode;
    }

    public void StartCoroutine(CoroutineBase coroutine)
    {
        if (!IsAlive)
        {
            throw new InvalidOperationException("Cannot start child coroutine on dead parent coroutine.");
        }

        coroutine.Manager = Manager;
        coroutine.Parent = this;

        AddChild(coroutine);
        coroutine.Init();
    }

    /// <summary>
    /// Initializes the coroutine once it has been added to the active coroutine hierarchy.
    /// </summary>
    public void Init()
    {
        InheritModesFromParent();

        OnEnter();

        if (IsAlive)
        {
            UpdateRunState(Manager.IsPaused);
        }
    }

    private void InheritModesFromParent()
    {
        if (this.ProcessMode == CoProcessMode.Inherit)
        {
            this.ProcessMode = Parent?.ProcessMode ?? CoProcessMode.Normal;
        }

        if (this.RunMode == CoRunMode.Inherit)
        {
            this.RunMode = Parent?.RunMode ?? CoRunMode.Pausable;
        }
    }


    // Coroutine lifecycle events:

    /// <summary>
    /// Called when the coroutine enters the active coroutine hierarchy.
    /// </summary>
    protected virtual void OnEnter() { }

    /// <summary>
    /// Called when the coroutine is killed.
    /// </summary>
    protected virtual void OnExit() { }


    // Coroutine execution events:

    /// <summary>
    /// Called after OnEnter() if the coroutine is running or as soon as it is unpaused.
    /// </summary>
    protected virtual void OnStart() { }

    /// <summary>
    /// Called when this coroutine is paused, which happens (depending on the RunMode) when the game is paused.
    /// By default, this method disables updates to the coroutine.
    /// </summary>
    protected virtual void OnPause()
    {
        wasReceivingUpdatesBeforePause = ShouldReceiveUpdates;
        DisableUpdates();
    }

    /// <summary>
    /// Called when this coroutine is unpaused, which happens (depending on the RunMode) when the game is unpaused.
    /// By default, this method re-enables updates to the coroutine if they were enabled before pausing.
    /// </summary>
    protected virtual void OnResume()
    {
        if (wasReceivingUpdatesBeforePause)
        {
            EnableUpdates();
        }
    }

    /// <summary>
    /// Called every frame if the coroutine is playing.
    /// The ProcessMode determines whether it is run in _Process() or _PhysicsProcess().
    /// </summary>
    public virtual void Update() { }

    /// <summary>
    /// Starts playing this coroutine, meaning that it will receive Update() calls
    /// each frame. This is independent of the child coroutines.
    /// This method only works if the coroutine is still alive.
    /// </summary>
    protected void EnableUpdates()
    {
        if (!IsAlive)
        {
            throw new InvalidOperationException("Cannot enable updates on a dead coroutine.");
        }

        ShouldReceiveUpdates = true;
        Manager.ActivateCoroutine(this);
    }

    /// <summary>
    /// Stops giving the coroutine Update() calls each frame.
    /// This is independent of the child coroutines.
    /// </summary>
    protected void DisableUpdates()
    {
        ShouldReceiveUpdates = false;
        Manager.DeactivateCoroutine(this);
    }

    /// <summary>
    /// Called when one of the child coroutines stops / finishes.
    /// This method is only called by a child coroutine when this coroutine is not dead.
    /// </summary>
    protected virtual void OnChildStopped(CoroutineBase child)
    {
        RemoveChild(child);
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

        IsAlive = false;
        Manager.DeactivateCoroutine(this);

        try
        {
            OnExit();
        }
        catch (Exception e)
        {
            GD.PrintErr(e.ToString());
        }

        CoroutineBase child = firstChild;
        while (child != null)
        {
            child.Kill();
            child = child.nextSibling;
        }

        if (Parent != null && Parent.IsAlive)
        {
            Parent.OnChildStopped(this);
        }

        Stopped?.Invoke();
    }

    /// <summary>
    /// Adds a coroutine as a child.
    /// </summary>
    private void AddChild(CoroutineBase coroutine)
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
    private void RemoveChild(CoroutineBase coroutine)
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

    /// <summary>
    /// Called when the game is paused or unpaused.
    /// </summary>
    public void OnGamePausedChanged(bool isGamePaused)
    {
        UpdateRunState(isGamePaused);
        NotifyChildrenAboutPausedChanged(isGamePaused);
    }

    /// <summary>
    /// Informs the child coroutines that the game has been paused / unpaused.
    /// </summary>
    private void NotifyChildrenAboutPausedChanged(bool isGamePaused)
    {
        CoroutineBase child = firstChild;
        while (child != null)
        {
            child.OnGamePausedChanged(isGamePaused);

            child = child.nextSibling;
        }
    }


    private void UpdateRunState(bool isGamePaused)
    {
        bool shouldBeRunning = RunMode switch {
            CoRunMode.Always => true,
            CoRunMode.Pausable => !isGamePaused,
            CoRunMode.WhenPaused => isGamePaused
        };

        if (IsRunning == shouldBeRunning)
        {
            return;
        }

        IsRunning = shouldBeRunning;

        if (shouldBeRunning)
        {
            if (hasCalledStart)
            {
                OnResume();
            }
            else
            {
                hasCalledStart = true;
                OnStart();
            }
        }
        else
        {
            OnPause();
        }
    }
}