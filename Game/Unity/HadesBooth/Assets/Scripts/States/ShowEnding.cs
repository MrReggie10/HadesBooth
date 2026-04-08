
using UnityEngine;

public class ShowEnding : GameState<DefaultTransition>
{
    
    public ShowEnding(GameStatus status, string id = null) : base(status, id) {}

    public override void Setup()
    {
        base.Setup();
        // TODO play sound
        Debug.Log("Playing ending lights"); 
        if (status.DidPlayersWin())
        {
            status.dmx.PlayCue(Cue.GoodEnding);
        }
        else
        {
            status.dmx.PlayCue(Cue.BadEnding);            
        }
    }

    protected override DefaultTransition Run()
    {
        return DefaultTransition.Default;
    }
}
