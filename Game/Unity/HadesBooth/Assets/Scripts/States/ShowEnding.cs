
using UnityEngine;

public class ShowEnding : GameState<DefaultTransition>
{
    
    public ShowEnding(GameStatus status, string id = null) : base(status, id) {}

    public override void Setup()
    {
        base.Setup();
        // TODO turn on the ending lights and play sound depending on status.DidPlayersWin
    }

    protected override DefaultTransition Run()
    {
        return DefaultTransition.Default;
    }
}
