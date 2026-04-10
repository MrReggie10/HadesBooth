/* For Kenechukwu debugging*/
using UnityEngine;

public class PlayNotes : GameLinearStateMachine
{
    protected int levelNum;
    
    public PlayNotes(GameStatus status, int level, string id = null) : base(status, id)
    {
        levelNum = level;

        if (levelNum >= 0 && levelNum < status.levelTimings.Length && status.levelTimings[levelNum].conductorNotes.Length > 0)
        {
            NoteTiming[] notes = status.levelTimings[levelNum].conductorNotes;
            for (int idx = 0; idx < notes.Length; idx++)
            {
                NoteTiming note = notes[idx];
                int nextNoteStart = idx < notes.Length - 1
                    ? notes[idx + 1].msSinceLevelStart
                    : status.levelTimings[levelNum].measureLengthMs * 2;
                AddState(new WaitForTime(status, (nextNoteStart - note.msSinceLevelStart) / 1000f, setup: () => status.SetConductorFlower(note.note), id: $"Playing conductor note {idx} ({note.noteColor})"));
            }
        }
    }

    public override void Setup()
    {
        base.Setup();
        status.SendFlowerWallTimings(levelNum);
    }

    public override void Cleanup()
    {
        base.Cleanup();
        status.SetConductorFlower(null);
    }
}
