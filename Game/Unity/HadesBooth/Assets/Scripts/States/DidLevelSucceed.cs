
using UnityEngine;

public class DidLevelSucceed : GameState<SuccessTransition>
{
    
    public DidLevelSucceed(GameStatus status, string id = null) : base(status, id) {}

    protected override SuccessTransition Run()
    {
        return status.WasLevelSuccessful() ? SuccessTransition.Success : SuccessTransition.Fail;
    }
}
