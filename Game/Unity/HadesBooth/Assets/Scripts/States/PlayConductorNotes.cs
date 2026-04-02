

public class PlayConductorNotes : GameLinearStateMachine
{
    public PlayConductorNotes(GameStatus status, Note[] notes, float timePerNote, string id = null) : base(status, id)
    {
        foreach (Note note in notes)
        {
            AddState(new WaitForTime(status, timePerNote, setup: () => status.SetConductorFlower(note)));
        }
    }

    public override void Cleanup()
    {
        base.Cleanup();
        status.SetConductorFlower(null);
    }
}
