

public class PlayConductorNotes : GameLinearStateMachine
{
    public PlayConductorNotes(GameStatus status, Note[] notes, float timePerNote, string id = null) : base(status, id)
    {
        for (int idx = 0; idx < notes.Length; idx++)
        {
            Note note = notes[idx];
            AddState(new WaitForTime(status, timePerNote, setup: () => status.SetConductorFlower(note), id: $"Playing note {idx} ({note.noteColor})"));
        }
    }

    public override void Cleanup()
    {
        base.Cleanup();
        status.SetConductorFlower(null);
    }
}
