
public class WaitForConductorNote : GameState<SuccessTransition>
{
    protected Note targetNote;
    protected float maxWaitTime;

    public WaitForConductorNote(GameStatus status, Note targetNote, float maxWaitTime, string id = null) :
        base(status, id)
    {
        this.targetNote = targetNote;
        this.maxWaitTime = maxWaitTime;
    }

    protected override SuccessTransition Run()
    {
        SuccessTransition baseTrans = base.Run();
        if (baseTrans != null) return baseTrans;

        Note? currentNote = status.CurrentConductorNote();
        status.miscUi.text = currentNote.HasValue ? $"Note {currentNote.Value.noteColor}" : "Waiting for note";
        if (currentNote == targetNote)
        {
            status.successfulNotesPlayedThisLevel++;
            return SuccessTransition.Success;
        }

        return timeSinceStart > maxWaitTime ? SuccessTransition.Fail : null;
    }

    public override void Cleanup()
    {
        base.Cleanup();
        status.notesPlayedThisLevel++;
    }
}
