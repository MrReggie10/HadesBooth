
using System.Linq;
using UnityEngine;

public class PlayLyreNote : GameState<PartialSuccessTransition>
{
    protected Note targetNote;
    protected int currentFlowerIdx;
    protected SuccessTransition[] lyreStatuses;
    
    public PlayLyreNote(GameStatus status, Note targetNote, string id = null) : base(status, id)
    {
        this.targetNote = targetNote;
        lyreStatuses = new SuccessTransition[status.numLyres];
    }

    public override void Setup()
    {
        base.Setup();
        for (int idx = 0; idx < lyreStatuses.Length; idx++)
        {
            lyreStatuses[idx] = null;
        }
        
        // if this is slow we probably don't need to do all of them?
        // would just need to be careful that theyre actually set to black
        currentFlowerIdx = 0;
        for (int idx = 0; idx < status.numLyreFlowers; idx++)
        {
            SetFlowerColor(idx, idx == currentFlowerIdx ? targetNote.GetColor() : Color.black);
        }
    }

    protected PartialSuccessTransition GetTransition()
    {
        int numSuccess = lyreStatuses.Aggregate(0, (sum, stat) => sum + ((stat?.Equals(SuccessTransition.Success) ?? false) ? 1 : 0));
        if (numSuccess == lyreStatuses.Length) return PartialSuccessTransition.Success;
        if (numSuccess == 0) return PartialSuccessTransition.Fail;
        return PartialSuccessTransition.PartialSuccess;
    }

    protected override PartialSuccessTransition Run()
    {
        PartialSuccessTransition baseTrans = base.Run();
        if (baseTrans != null) return baseTrans;

        int newFlowerIdx = (int)(timeSinceStart / status.lyreTimePerFlower);
        if (newFlowerIdx != currentFlowerIdx)
        {
            if (newFlowerIdx >= status.numLyreFlowers) return GetTransition();
            SetFlowerColor(currentFlowerIdx, Color.black);
            currentFlowerIdx++;
            SetFlowerColor(currentFlowerIdx, targetNote.GetColor());
        }

        if (status.miscUi != null)
        {
            status.miscUi.text = $"Lyre: Flower {currentFlowerIdx+1}/{status.numLyreFlowers}";
        }

        string lyreNoteStr = "";
        for (int lyreIdx = 0; lyreIdx < status.numLyres; lyreIdx++)
        {
            Note? note = status.CurrentLyreNote(lyreIdx);
            lyreNoteStr += (note?.ToString() ?? "null") + " ";
            if (lyreStatuses[lyreIdx] != null || note == null) continue;
            lyreStatuses[lyreIdx] = currentFlowerIdx == status.numLyreFlowers - 1 && note.Value.Equals(targetNote)
                ? SuccessTransition.Success
                : SuccessTransition.Fail;
        }
        Debug.Log($"PlayLyreNote target={targetNote} got={lyreNoteStr} flowerIdx={currentFlowerIdx} time={timeSinceStart:F2}");

        if (lyreStatuses.Any(s => s == null)) return null;
        return GetTransition();
    }

    protected void SetFlowerColor(int flower, Color color)
    {
        // TODO set flower color
        // assumes that we start at the 1 o'clock flower and end at 12
        // if we both start and end at 12 we'll need to change some stuff here
        // idx 0 is the 1 o'clock flower
    }

    public override void Cleanup()
    {
        base.Cleanup();
        if (transition.Equals(PartialSuccessTransition.Success) ||
            transition.Equals(PartialSuccessTransition.PartialSuccess))
        {
            status.successfulNotesPlayedThisLevel++;
        }
        status.notesPlayedThisLevel++;
        for (int lyreIdx = 0; lyreIdx < status.numLyres; lyreIdx++)
        {
            status.ClearLyreNote(lyreIdx);
        }
        if (status.miscUi != null)
        {
            status.miscUi.text = "";
        }
    }
}
