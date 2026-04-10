/* For Kenechukwu debugging*/
using UnityEngine;

public class PlayNotes : GameLinearStateMachine
{
    protected int levelNum;
    
    public PlayNotes(GameStatus status, int level, string id = null) : base(status, id)
    {
        levelNum = level;

        int prevTimeMs = status.levelTimings[levelNum].conductorNotes[0].msSinceLevelStart;
        for (int idx = 0; idx < status.levelTimings[levelNum].conductorNotes.Length; idx++)
        {
            int prevTimeMs = status.levelTimings[levelNum].conductorNotes[0].msSinceLevelStart;
            for (int idx = 0; idx < status.levelTimings[levelNum].conductorNotes.Length; idx++)
            {
                NoteTiming note = status.levelTimings[levelNum].conductorNotes[idx];
                AddState(new WaitForTime(status, (note.msSinceLevelStart - prevTimeMs) / 1000f, setup: () => status.SetConductorFlower(note.note), id: $"Playing conductor note {idx} ({note.noteColor})"));
                prevTimeMs = note.msSinceLevelStart;
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
