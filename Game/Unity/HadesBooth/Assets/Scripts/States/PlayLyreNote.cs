
using UnityEngine;

public class PlayLyreNote : GameState<SuccessTransition>
{
    protected Note targetNote;
    protected int currentFlowerIdx;
    
    public PlayLyreNote(GameStatus status, Note targetNote, string id = null) : base(status, id)
    {
        this.targetNote = targetNote;
    }

    public override void Setup()
    {
        base.Setup();
        
        // if this is slow we probably don't need to do all of them?
        // would just need to be careful that theyre actually set to black
        currentFlowerIdx = 0;
        for (int idx = 0; idx < status.numLyreFlowers; idx++)
        {
            SetFlowerColor(idx, idx == currentFlowerIdx ? targetNote.GetColor() : Color.black);
        }
    }

    protected override SuccessTransition Run()
    {
        SuccessTransition baseTrans = base.Run();
        if (baseTrans != null) return baseTrans;

        int newFlowerIdx = (int)(timeSinceStart / status.lyreTimePerFlower);
        if (newFlowerIdx != currentFlowerIdx)
        {
            if (newFlowerIdx >= status.numLyreFlowers) return SuccessTransition.Fail;
            SetFlowerColor(currentFlowerIdx, Color.black);
            currentFlowerIdx++;
            SetFlowerColor(currentFlowerIdx, targetNote.GetColor());
        }

        Note? lyreNote = status.CurrentLyreNote();
        if (lyreNote == null) return null;
        if (lyreNote == targetNote && currentFlowerIdx == status.numLyreFlowers - 1)
        {
            status.successfulNotesPlayedThisLevel++;
            return SuccessTransition.Success;;
        }
        return SuccessTransition.Fail;
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
        status.notesPlayedThisLevel++;
        status.ClearLyreNote();
    }
}
