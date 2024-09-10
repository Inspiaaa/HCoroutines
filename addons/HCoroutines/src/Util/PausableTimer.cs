using Godot;
using System;

namespace HCoroutines.Util;

public class PausableTimer
{
    private readonly SceneTreeTimer timer;
    private float remainingTime;

    public PausableTimer(SceneTree sceneTree, float timeout, bool ignoreTimeScale, Action callback)
    {
        timer = sceneTree.CreateTimer(timeout, ignoreTimeScale: ignoreTimeScale);
        timer.Timeout += callback;
    }

    public void Pause()
    {
        remainingTime = (float)timer.TimeLeft;
        timer.TimeLeft = double.MaxValue;
    }

    public void Resume()
    {
        timer.TimeLeft = remainingTime;
    }
}