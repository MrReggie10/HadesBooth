
using System.Linq;
using UnityEngine;

public abstract class ReadNotes : GameState<DefaultTransition>
{
    public delegate void OnNoteTransitionDelegate(int numSuccess);
    
    protected int levelNum;
    protected int acceptableBeforeMs;
    protected int acceptableAfterMs;
    protected bool failOnIncorrect;
    protected OnNoteTransitionDelegate additionalNoteTransition;
    
    protected NoteTiming[] targetNotes => GetTimingsFromLevel(status.levelTimings[levelNum]);
    protected SuccessTransition[][] playerStatuses;
    protected int numPlayers => playerStatuses.Length;
    protected int lastNoteIdx;
    
    public ReadNotes(GameStatus status, int levelNum, int numPlayers, int acceptableBeforeMs, int acceptableAfterMs,
        bool failOnIncorrect = false, OnNoteTransitionDelegate onNoteTransition = null, string id = null) : base(status, id)
    {
        this.levelNum = levelNum;
        this.acceptableBeforeMs = acceptableBeforeMs;
        this.acceptableAfterMs = acceptableAfterMs;
        this.failOnIncorrect = failOnIncorrect;
        additionalNoteTransition = onNoteTransition;
        
        playerStatuses = new SuccessTransition[numPlayers][];
        for (int idx = 0; idx < numPlayers; idx++)
        {
            playerStatuses[idx] = new SuccessTransition[targetNotes.Length];
        }
    }

    protected abstract NoteTiming[] GetTimingsFromLevel(LevelTiming level);
    protected abstract Note? GetCurrentNote(int playerIdx);

    protected int CurrentNoteIndex()
    {
        int currentTimeMs = (int)(timeSinceStart * 1000);
        
        for (int idx = 0; idx < targetNotes.Length; idx++)
        {
            NoteTiming noteTiming = targetNotes[idx];
            int upperTimeMs = noteTiming.msSinceLevelStart + acceptableAfterMs;
            if (currentTimeMs <= upperTimeMs) return idx;
        }

        return -1;
    }

    protected Note? CurrentTargetNote()
    {
        int currentTimeMs = (int)(timeSinceStart * 1000);
        
        for (int idx = 0; idx < targetNotes.Length; idx++)
        {
            NoteTiming noteTiming = targetNotes[idx];
            int upperTimeMs = noteTiming.msSinceLevelStart + acceptableAfterMs;
            int lowerTimeMs = noteTiming.msSinceLevelStart - acceptableBeforeMs;
            if (currentTimeMs <= upperTimeMs && currentTimeMs >= lowerTimeMs) return noteTiming.note;
        }
        
        return null;
    }

    private void OnNoteTransition(int newNoteIndex)
    {
        int numSuccess = 0;
        for (int playerIdx = 0; playerIdx < numPlayers; playerIdx++)
        {
            if (playerStatuses[playerIdx][lastNoteIdx] == null) playerStatuses[playerIdx][lastNoteIdx] = SuccessTransition.Fail;
            if (playerStatuses[playerIdx][lastNoteIdx].Equals(SuccessTransition.Success)) numSuccess++;
        }

        if (numSuccess > 0) status.successfulNotesPlayedThisLevel++;
        additionalNoteTransition?.Invoke(numSuccess);
        lastNoteIdx = newNoteIndex;
    }

    public override void Setup()
    {
        base.Setup();
        for (int playerIdx = 0; playerIdx < playerStatuses.Length; playerIdx++)
        {
            for (int noteIdx = 0; noteIdx < playerStatuses[playerIdx].Length; noteIdx++)
            {
                playerStatuses[playerIdx][noteIdx] = null;
            }
        }

        lastNoteIdx = 0;
    }

    protected override DefaultTransition Run()
    {
        DefaultTransition baseTrans = base.Run();
        if (baseTrans != null) return baseTrans;

        int currentNoteIdx = CurrentNoteIndex();
        if (lastNoteIdx != currentNoteIdx)
        {
            OnNoteTransition(currentNoteIdx);
            if (currentNoteIdx == -1) return DefaultTransition.Default;
        }
        
        Note? currentNote = CurrentTargetNote();
        for (int playerIdx = 0; playerIdx < numPlayers; playerIdx++)
        {
            if (playerStatuses[playerIdx][currentNoteIdx]?.Equals(SuccessTransition.Success) ?? false) continue;
            Note? notePlayed = GetCurrentNote(playerIdx);
            if (notePlayed == null) continue;
            if (notePlayed.Value.Equals(currentNote)) playerStatuses[playerIdx][currentNoteIdx] = SuccessTransition.Success;
            else if (failOnIncorrect) playerStatuses[playerIdx][currentNoteIdx] = SuccessTransition.Fail;
        }

        if (status.miscUi && currentNote.HasValue)
        {
            status.miscUi.text = $"Note index {currentNoteIdx}, color {currentNote?.noteColor}";
        }

        return null;
    }

    public override void Cleanup()
    {
        base.Cleanup();
        
        if (status.miscUi)
        {
            status.miscUi.text = "";
        }
    }
}
