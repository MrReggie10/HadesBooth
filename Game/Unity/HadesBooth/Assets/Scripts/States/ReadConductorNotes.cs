
public class ReadConductorNotes : ReadNotes
{
    public ReadConductorNotes(GameStatus status, int levelNum, OnNoteTransitionDelegate onNoteTransition = null, string id = null) 
        : base(status, levelNum, 1, status.levelTimings[levelNum].acceptableDifferenceMs, status.levelTimings[levelNum].acceptableDifferenceMs, onNoteTransition: onNoteTransition, id: id) {}

    protected override NoteTiming[] GetTimingsFromLevel(LevelTiming level)
    {
        return level.conductorNotes;
    }

    protected override Note? GetCurrentNote(int _)
    {
        return status.CurrentConductorNote();
    }
}
