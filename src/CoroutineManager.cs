using Godot;
using System;
using System.Collections.Generic;

namespace HCoroutines;

public partial class CoroutineManager : Node
{
    public static CoroutineManager Instance { get; private set; }
    public float DeltaTime { get; private set; }
    public double DeltaTimeDouble { get; private set; }

    private bool isIteratingActiveCoroutines = false;
    private HashSet<CoroutineBase> activeCoroutines = new();
    private HashSet<CoroutineBase> coroutinesToDeactivate = new();
    private HashSet<CoroutineBase> coroutinesToActivate = new();

    public void StartCoroutine(CoroutineBase coroutine)
    {
        coroutine.Manager = this;
        coroutine.OnEnter();
    }

    public void ActivateCoroutine(CoroutineBase coroutine)
    {
        if (isIteratingActiveCoroutines)
        {
            coroutinesToActivate.Add(coroutine);
            coroutinesToDeactivate.Remove(coroutine);
        }
        else
        {
            activeCoroutines.Add(coroutine);
        }
    }

    public void DeactivateCoroutine(CoroutineBase coroutine)
    {
        if (isIteratingActiveCoroutines)
        {
            coroutinesToDeactivate.Add(coroutine);
            coroutinesToActivate.Remove(coroutine);
        }
        else
        {
            activeCoroutines.Remove(coroutine);
        }
    }

    public override void _EnterTree()
    {
        Instance = this;
    }

    public override void _Process(double delta)
    {
        DeltaTime = (float)delta;
        DeltaTimeDouble = delta;

        isIteratingActiveCoroutines = true;

        foreach (CoroutineBase coroutine in activeCoroutines)
        {
            if (coroutine.IsAlive && coroutine.IsPlaying)
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

        isIteratingActiveCoroutines = false;

        foreach (CoroutineBase coroutine in coroutinesToActivate)
        {
            activeCoroutines.Add(coroutine);
        }
        coroutinesToActivate.Clear();

        foreach (CoroutineBase coroutine in coroutinesToDeactivate)
        {
            activeCoroutines.Remove(coroutine);
        }
        coroutinesToDeactivate.Clear();
    }
}