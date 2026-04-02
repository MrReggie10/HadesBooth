
using System;
using UnityEngine;

public class WaitForTime : GameState<DefaultTransition>
{
    protected float timeToWait;
    protected Action setup;
    protected Action cleanup;
    
    public WaitForTime(GameStatus status, float timeToWait, Action setup = null, Action cleanup = null, string id = null) : base(status, id)
    {
        this.timeToWait = timeToWait;
    }

    public override void Setup()
    {
        base.Setup();
        setup?.Invoke();
    }

    protected override DefaultTransition Run()
    {
        DefaultTransition baseTrans = base.Run();
        if (baseTrans != null) return baseTrans;

        return timeSinceStart > timeToWait ? DefaultTransition.Default : null;
    }

    public override void Cleanup()
    {
        base.Cleanup();
        cleanup?.Invoke();
    }
}
