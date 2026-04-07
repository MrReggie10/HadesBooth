
using UnityEngine;

public class LyreTutorial : GameNetworkedStateMachine
{
    public LyreTutorial(GameStatus status, string id = null) : base(status, id)
    {
        // Kenechukwu: also update lyre tutorial if you want to change any timings/charting
        Note[] notes = { Notes.Red, Notes.Blue, Notes.Yellow, Notes.Cyan };

        PlayLyreNote[] waitForNotes = new PlayLyreNote[notes.Length];
        GiveFeedback[] successes = new GiveFeedback[notes.Length];
        GiveFeedback[] fails = new GiveFeedback[notes.Length];

        for (int idx = 0; idx < notes.Length; idx++)
        {
            waitForNotes[idx] = new PlayLyreNote(status, notes[idx], $"Play lyre note {idx}");
            successes[idx] = new GiveFeedback(status, true, $"Lyre note {idx} success");
            fails[idx] = new GiveFeedback(status, false, $"Lyre note {idx} fail");

            if (idx == 0)
            {
                SetInitialState(waitForNotes[idx]);
            }
            else
            {
                AddTransition(successes[idx - 1], waitForNotes[idx]);
                AddTransition(fails[idx - 1], waitForNotes[idx]);
            }
            
            AddTransition(waitForNotes[idx], PartialSuccessTransition.Success, successes[idx]);
            AddTransition(waitForNotes[idx], PartialSuccessTransition.PartialSuccess, successes[idx]);
            AddTransition(waitForNotes[idx], PartialSuccessTransition.Fail, fails[idx]);
        }
        
        AddExitTransition(successes[^1]);
        AddExitTransition(fails[^1]);
    }

    public override void Setup()
    {
        base.Setup();
        status.notesPlayedThisLevel = 0;
    }
}
