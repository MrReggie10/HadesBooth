
using UnityEngine;

public class GiveFeedback : GameState<DefaultTransition>
{
    protected bool didGood;
    
    public GiveFeedback(GameStatus status, bool didGood, string id = null) : base(status, id)
    {
        this.didGood = didGood;
    }

    public override void Setup()
    {
        base.Setup();
        if (didGood)
        {
            // TODO play good sound, make lights green or something
        }
        else
        {
            // TODO play bad sound, make lights red or something
        }
    }

    protected override DefaultTransition Run()
    {
        DefaultTransition baseTrans = base.Run();
        if (baseTrans != null) return baseTrans;

        // TODO return DefaultTransition.Default when sound is over
        return null;
    }

    public override void Cleanup()
    {
        base.Cleanup();
        // TODO reset lights
    }
}
