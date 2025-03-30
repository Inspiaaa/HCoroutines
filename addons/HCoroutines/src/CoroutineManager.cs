using Godot;
using System;
using System.Collections.Generic;
using HCoroutines.Util;

namespace HCoroutines;

public partial class CoroutineManager : Node
{
    public static CoroutineManager Instance { get; private set; }
    private static CoroutineManager globalInstance;

    public float DeltaTime { get; private set; }
    public double DeltaTimeDouble { get; private set; }

    public float PhysicsDeltaTime { get; private set; }
    public double PhysicsDeltaTimeDouble { get; private set; }

    public bool IsPaused { get; private set; }

    private DeferredHashSet<CoroutineBase> activeProcessCoroutines = new();
    private DeferredHashSet<CoroutineBase> activePhysicsProcessCoroutines = new();
    private HashSet<CoroutineBase> aliveRootCoroutines = new();

    public override void _EnterTree()
    {
        Instance = this;
        ProcessMode = ProcessModeEnum.Always;
        IsPaused = GetTree().Paused;

        if (IsAutoloaded())
        {
            // This instance is the global (autoloaded) instance that is shared between scenes.
            globalInstance = this;
        }
    }

    public override void _ExitTree()
    {
        if (Instance == this)
        {
            // Switch back to the global (autoloaded) manager when the scene-local instance is removed (e.g. when
            // the current scene is changed).
            Instance = globalInstance;
        }
    }

    private bool IsAutoloaded()
    {
        return GetParent() == GetTree().Root && GetTree().CurrentScene != this;
    }

    /// <summary>
    /// Starts and initializes the given coroutine.
    /// </summary>
    public void StartCoroutine(CoroutineBase coroutine)
    {
        coroutine.Manager = this;
        coroutine.Stopped += () => aliveRootCoroutines.Remove(coroutine);
        coroutine.Init();
        aliveRootCoroutines.Add(coroutine);
    }

    /// <summary>
    /// Enables Update() calls to the coroutine.
    /// </summary>
    public void ActivateCoroutine(CoroutineBase coroutine)
    {
        GetUpdatePoolOfCoroutine(coroutine).Add(coroutine);
    }

    /// <summary>
    /// Disables Update() calls to the coroutine.
    /// </summary>
    public void DeactivateCoroutine(CoroutineBase coroutine)
    {
        GetUpdatePoolOfCoroutine(coroutine).Remove(coroutine);
    }

    private DeferredHashSet<CoroutineBase> GetUpdatePoolOfCoroutine(CoroutineBase coroutine)
    {
        return coroutine.ProcessMode switch {
            CoProcessMode.Normal or CoProcessMode.Inherit => activeProcessCoroutines,
            CoProcessMode.Physics => activePhysicsProcessCoroutines
        };
    }

    public override void _Process(double delta)
    {
        DeltaTime = (float)delta;
        DeltaTimeDouble = delta;
        SetGamePaused(GetTree().Paused);

        UpdateCoroutines(activeProcessCoroutines);
    }

    public override void _PhysicsProcess(double delta)
    {
        PhysicsDeltaTime = (float)delta;
        PhysicsDeltaTimeDouble = delta;

        UpdateCoroutines(activePhysicsProcessCoroutines);
    }

    private void UpdateCoroutines(DeferredHashSet<CoroutineBase> coroutines)
    {
        coroutines.Lock();

        foreach (CoroutineBase coroutine in coroutines.Items)
        {
            if (coroutine.IsAlive && coroutine.ShouldReceiveUpdates)
            {
                try
                {
                    coroutine.Update();
                }
                catch (Exception e)
                {
                    GD.PrintErr(e.ToString());
                }
            }
        }

        coroutines.Unlock();
    }

    private void SetGamePaused(bool isPaused)
    {
        if (this.IsPaused == isPaused)
        {
            return;
        }

        this.IsPaused = isPaused;

        foreach (CoroutineBase coroutine in aliveRootCoroutines)
        {
            coroutine.OnGamePausedChanged(isPaused);
        }
    }
}