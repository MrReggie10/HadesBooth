
public class ReadLyreNotes : ReadNotes
{
    public ReadLyreNotes(GameStatus status, int levelNum, bool failOnIncorrect = false, OnNoteTransitionDelegate onNoteTransition = null, string id = null) 
        : base(status, levelNum, status.numLyres, status.levelTimings[levelNum].acceptableDifferenceMs, status.levelTimings[levelNum].acceptableDifferenceMs, failOnIncorrect, onNoteTransition: onNoteTransition, id: id) {}

    protected override NoteTiming[] GetTimingsFromLevel(LevelTiming level)
    {
        return level.lyreNotes;
    }

    protected override Note? GetCurrentNote(int playerIdx)
    {
        return status.CurrentLyreNote(playerIdx);
    }
}
