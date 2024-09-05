using Godot;
using System;
using System.Collections.Generic;
using HCoroutines.Util;

namespace HCoroutines;

public partial class CoroutineManager : Node
{
    public static CoroutineManager Instance { get; private set; }
    
    public float DeltaTime { get; private set; }
    public double DeltaTimeDouble { get; private set; }
    
    public float PhysicsDeltaTime { get; private set; }
    public double PhysicsDeltaTimeDouble { get; private set; }

    public bool IsPaused { get; private set; }

    private DeferredHashSet<CoroutineBase> activeProcessCoroutines = new();
    private DeferredHashSet<CoroutineBase> activePhysicsProcessCoroutines = new();
    private HashSet<CoroutineBase> aliveRootCoroutines = new();
    
    public void StartCoroutine(CoroutineBase coroutine)
    {
        coroutine.Manager = this;
        coroutine.Init();
        aliveRootCoroutines.Add(coroutine);
    }

    public void ActivateCoroutine(CoroutineBase coroutine)
    {
        GetUpdatePoolOfCoroutine(coroutine).Add(coroutine);
    }

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

    public override void _EnterTree()
    {
        Instance = this;
        IsPaused = GetTree().Paused;
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