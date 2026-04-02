
using UnityEngine;

public class ConductorTutorial : GameNetworkedStateMachine
{
    public ConductorTutorial(GameStatus status, string id = null) : base(status, id)
    {
        // TODO set tutorial order, probably make editable in GameStatus
        Note[] notes = { Notes.Red, Notes.Blue, Notes.Yellow, Notes.Cyan };

        PlayConductorNotes playNotes = new PlayConductorNotes(status, notes, status.conductorTimePerNoteOnFlower, "Play notes for conductor tutorial");
        WaitForConductorNote[] waitForNotes = new WaitForConductorNote[notes.Length];
        GiveFeedback[] successes = new GiveFeedback[notes.Length];
        GiveFeedback[] fails = new GiveFeedback[notes.Length];

        SetInitialState(playNotes);
        for (int idx = 0; idx < notes.Length; idx++)
        {
            waitForNotes[idx] = new WaitForConductorNote(status, notes[idx], status.conductorTimeToPlayNote, $"Wait for conductor note {idx}");
            successes[idx] = new GiveFeedback(status, true, $"Conductor note {idx} success");
            fails[idx] = new GiveFeedback(status, false, $"Conductor note {idx} fail");

            if (idx == 0)
            {
                AddTransition(playNotes, waitForNotes[idx]);
            }
            else
            {
                AddTransition(successes[idx - 1], waitForNotes[idx]);
                AddTransition(fails[idx - 1], waitForNotes[idx]);
            }
            
            AddTransition(waitForNotes[idx], SuccessTransition.Success, successes[idx]);
            AddTransition(waitForNotes[idx], SuccessTransition.Fail, fails[idx]);
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
