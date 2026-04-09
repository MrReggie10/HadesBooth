
public class ReadConductorNotes : ReadNotes
{
    public ReadConductorNotes(GameStatus status, int levelNum, OnNoteTransitionDelegate onNoteTransition = null,
        string id = null)
        : base(status, levelNum, 1, status.levelTimings[levelNum].acceptableDifferenceMs,
            status.levelTimings[levelNum].acceptableDifferenceMs, id: id)
    {
        additionalNoteTransition = (numSuccess) =>
        {
            OnNoteTransition(numSuccess);
            if (onNoteTransition != null) onNoteTransition(numSuccess);
        };
    }

    protected override NoteTiming[] GetTimingsFromLevel(LevelTiming level)
    {
        return level.conductorNotes;
    }

    protected override Note? GetCurrentNote(int _)
    {
        return status.useConductor
            ? status.CurrentConductorNote()
            : status.CurrentLyreNote(1);
    }

    protected void OnNoteTransition(int numSuccess)
    {
        status.PlayConductorSfx(numSuccess > 0);
    }
}
