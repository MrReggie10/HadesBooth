using UnityEngine;

public class WaitForLyreNote : GameState<SuccessTransition>
{
    protected NoteColor targetNote;
    protected float maxWaitTime;
    protected bool failOnIncorrectNote;

    public WaitForLyreNote(GameStatus status, NoteColor targetNote, float maxWaitTime, bool failOnIncorrectNote = true, string id = null) :
        base(status, id)
    {
        this.targetNote = targetNote;
        this.maxWaitTime = maxWaitTime;
        this.failOnIncorrectNote = failOnIncorrectNote;
    }

    protected override SuccessTransition Run()
    {
        SuccessTransition baseTrans = base.Run();
        if (baseTrans != null) return baseTrans;

        Note? currentNote = status.CurrentLyreNote();
        if (currentNote.HasValue)
        {
            Debug.Log($"WaitForLyreNote target={targetNote} got={currentNote.Value.noteColor} time={timeSinceStart:F2}");
            if (currentNote.Value.noteColor == targetNote) return SuccessTransition.Success;
            if (failOnIncorrectNote) return SuccessTransition.Fail;
        }

        return timeSinceStart > maxWaitTime ? SuccessTransition.Fail : null;
    }
}
