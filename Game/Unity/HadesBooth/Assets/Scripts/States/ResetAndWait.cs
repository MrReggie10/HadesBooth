
using UnityEngine;

public class ResetAndWait : GameState<DefaultTransition>
{
    protected KeyCode? key;
    
    public ResetAndWait(GameStatus status, KeyCode? key = null, string id = null) : base(status, id)
    {
        this.key = key;
    }

    public override void Setup()
    {
        base.Setup();
        status.Reset();
    }

    protected override DefaultTransition Run()
    {
        DefaultTransition baseTrans = base.Run();
        if (baseTrans != null) return baseTrans;

        Note? lyre1Note = status.CurrentLyreNote(0);
        Note? lyre2Note = status.CurrentLyreNote(1);
        bool lyreStrum = (lyre1Note?.Equals(Notes.Red) ?? false) && (lyre2Note?.Equals(Notes.Red) ?? false);
        if (lyreStrum || (key.HasValue && Input.GetKeyDown(key.Value)) || (!key.HasValue && Input.anyKeyDown)) return DefaultTransition.Default;

        return null;
    }

    public override void Cleanup()
    {
        base.Cleanup();
        // TODO turn off all the ending lights
        status.dmx.PlayCue(Cue.Blackout);
    }
}
