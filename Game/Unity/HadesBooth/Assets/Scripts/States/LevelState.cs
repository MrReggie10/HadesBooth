
public class LevelState : GameNetworkedStateMachine
{
    protected int levelNum;
    
    public LevelState(GameStatus status, int levelNum, string id = null) : base(status, id)
    {
        this.levelNum = levelNum;
        GameParallelState playAndReadNotes = new GameParallelState(status, $"Level {levelNum} play and read notes");
        playAndReadNotes.AddState(new PlayNotes(status, levelNum, $"Level {levelNum} play conductor notes"));
        playAndReadNotes.AddState(new ReadConductorNotes(status, levelNum, id: $"Level {levelNum} read conductor notes"));
        playAndReadNotes.AddState(new ReadLyreNotes(status, levelNum, true, id: $"Level {levelNum} read lyre notes"));
        DidLevelSucceed checkSuccess = new DidLevelSucceed(status, $"Check success of level {levelNum}");
        GiveFeedback success = new GiveFeedback(status, true, $"Successful level {levelNum}");
        GiveFeedback fail = new GiveFeedback(status, false, $"Failed level {levelNum}");

        SetInitialState(playAndReadNotes);
        AddTransition(playAndReadNotes, checkSuccess);
        AddTransition(checkSuccess, SuccessTransition.Success, success);
        AddTransition(checkSuccess, SuccessTransition.Fail, fail);
        AddExitTransition(success);
        AddExitTransition(fail);
    }

    public override void Setup()
    {
        base.Setup();
        status.OnLevelStart(levelNum);
    }

    public override void Cleanup()
    {
        base.Cleanup();
        status.OnLevelEnd();
    }
}
