/* For Kenechukwu debugging*/
//using UnityEngine;

public class PlayNotes : GameLinearStateMachine
{
    protected int levelNum;

    public PlayNotes(GameStatus status, int level, string id = null) : base(status, id)
    {
        levelNum = level;
        /* Kenechukwu Debug */
        //Debug.Log("levelNum = " + levelNum + " out of size " + status.levelTimings.Length);

        NoteTiming firstNote = status.levelTimings[levelNum].conductorNotes[0];
        int prevTimeMs = firstNote.msSinceLevelStart;
        //AddState(new WaitForTime(status, prevTimeMs / 1000f, setup: () => status.SetConductorFlower(firstNote.note), id: $"Playing conductor note {0} ({firstNote.noteColor})"));

        for (int idx = 0; idx < status.levelTimings[levelNum].conductorNotes.Length; idx++)
        //for (int idx = 1; idx < status.levelTimings[levelNum].conductorNotes.Length; idx++)
        {
            /*Kenechukwu Debugging*/
            //NoteTiming prevNote = status.levelTimings[levelNum].conductorNotes[idx - 1];
            NoteTiming note = status.levelTimings[levelNum].conductorNotes[idx];
            AddState(new WaitForTime(status, (note.msSinceLevelStart - prevTimeMs) / 1000f, setup: () => status.SetConductorFlower(note.note), id: $"Playing conductor note {idx} ({note.noteColor})"));
            //AddState(new WaitForTime(status, (note.msSinceLevelStart - prevTimeMs) / 1000f, setup: () => status.SetConductorFlower(prevNote.note), id: $"Playing conductor note {idx - 1} ({note.noteColor})"));
            prevTimeMs = note.msSinceLevelStart;
        }
        /*NoteTiming finalNote = status.levelTimings[levelNum].conductorNotes[^1];
        AddState(new WaitForTime(status,
                                 (status.levelTimings[levelNum].conductorNotes[0].msSinceLevelStart + status.levelTimings[levelNum].measureLengthMs
                                     - finalNote.msSinceLevelStart) / 1000f,
                                 setup: () => status.SetConductorFlower(finalNote.note), id: $"Playing conductor note {status.levelTimings[levelNum].conductorNotes.Length - 1} ({finalNote.noteColor})"));*/
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
