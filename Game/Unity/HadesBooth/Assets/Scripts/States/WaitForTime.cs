
using UnityEngine;

public class WaitForTime : GameState<DefaultTransition>
{
    protected float timeToWait;
    
    public WaitForTime(GameStatus status, float timeToWait, string id = null) : base(status, id)
    {
        this.timeToWait = timeToWait;
    }

    protected override DefaultTransition Run()
    {
        DefaultTransition baseTrans = base.Run();
        if (baseTrans != null) return baseTrans;

        return timeSinceStart > timeToWait ? DefaultTransition.Default : null;
    }
}
