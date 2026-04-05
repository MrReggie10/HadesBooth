
using UnityEngine;

public class GiveFeedback : GameState<DefaultTransition>
{
    protected bool didGood;
    protected float displayTime;
    
    public GiveFeedback(GameStatus status, bool didGood, string id = null, float displayTime = 1f) : base(status, id)
    {
        this.didGood = didGood;
        this.displayTime = displayTime;
    }

    public override void Setup()
    {
        base.Setup();
        if (didGood)
        {
            // TODO play good sound
        }
        else
        {
            // TODO play bad sound
        }
    }

    protected override DefaultTransition Run()
    {
        DefaultTransition baseTrans = base.Run();
        if (baseTrans != null) return baseTrans;

        // TODO return DefaultTransition.Default when sound is over
        // For now, returns after displayTime
        if (timeSinceStart > displayTime) {
            return DefaultTransition.Default;
        }
        return null;
    }

    public override void Cleanup()
    {
        base.Cleanup();
    }
}
