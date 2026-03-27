
using UnityEngine;

public class LevelState : GameNetworkedStateMachine
{
    public LevelState(GameStatus status, int levelNum, string id = null) : base(status, id)
    {
        // TODO set level order from levelNum, probably make editable in GameStatus
        Note[] notes = { Notes.Red, Notes.Blue, Notes.Yellow, Notes.Cyan };

        WaitForConductorNote[] conductorNotes = new WaitForConductorNote[notes.Length];
        PlayLyreNote[] lyreNotes = new PlayLyreNote[notes.Length];
        DidLevelSucceed checkSuccess = new DidLevelSucceed(status, $"Check success of level {levelNum}");
        GiveFeedback success = new GiveFeedback(status, true, $"Successful level {levelNum}");
        GiveFeedback fail = new GiveFeedback(status, false, $"Failed level {levelNum}");

        for (int idx = 0; idx < notes.Length; idx++)
        {
            conductorNotes[idx] = new WaitForConductorNote(status, notes[idx], status.conductorTutorialTimeForNote, $"Level {levelNum} wait for conductor note {idx}");
            lyreNotes[idx] = new PlayLyreNote(status, notes[idx], $"Level {levelNum} play lyre note {idx}");

            if (idx == 0)
            {
                SetInitialState(conductorNotes[idx]);
            }
            else
            {
                AddTransition(lyreNotes[idx - 1], SuccessTransition.Success, conductorNotes[idx]);
                AddTransition(lyreNotes[idx - 1], SuccessTransition.Fail, conductorNotes[idx]);
            }
            
            AddTransition(conductorNotes[idx], SuccessTransition.Success, lyreNotes[idx]);
            AddTransition(conductorNotes[idx], SuccessTransition.Fail, lyreNotes[idx]);
        }
        
        AddTransition(lyreNotes[^1], SuccessTransition.Success, checkSuccess);
        AddTransition(lyreNotes[^1], SuccessTransition.Fail, checkSuccess);
        AddTransition(checkSuccess, SuccessTransition.Success, success);
        AddTransition(checkSuccess, SuccessTransition.Fail, fail);
        AddExitTransition(success);
        AddExitTransition(fail);
    }

    public override void Setup()
    {
        base.Setup();
        status.notesPlayedThisLevel = 0;
        status.successfulNotesPlayedThisLevel = 0;
    }

    public override void Cleanup()
    {
        base.Cleanup();
        status.score += status.successfulNotesPlayedThisLevel;
    }
}
