
using System;
using TMPro;
using UnityEngine;

public class GameStatus : MonoBehaviour
{
    [HideInInspector] public int score;
    [HideInInspector] public int levelNum;
    [HideInInspector] public int performanceRating; // 0-3
    [HideInInspector] public int notesPlayedThisLevel;
    [HideInInspector] public int successfulNotesPlayedThisLevel;
    
    [Header("Levels")]
    public int numLevels = 3;

    [Header("Conductor")]
    [SerializeField] protected ConductorDetector conductorDetector;
    public float conductorTimePerNoteOnFlower;
    public float conductorTimeToPlayNote;
    public SerialController conductorController;

    [Header("Lyre")]
    public float lyreTimePerFlower;
    public int numLyreFlowers;
    public float lyreAcceptWindow;
    protected Note? currentLyreNote = null;

    [Header("Misc")]
    public bool debugMode;
    public TextMeshProUGUI gameStateUI;
    public TextMeshProUGUI miscUi;

    // blocks indicator LED change
    protected bool conductorLedFinalized = false;

    public Note? CurrentConductorNote()
    {
    return conductorDetector.currentNote;
    }

    public Note? CurrentLyreNote()
    {
        return currentLyreNote;
    }

    public void HandleMessage(string msg)
    {
        switch (msg)
        {
            case "r":
                currentLyreNote = Notes.Red;
                break;
            case "b":
                currentLyreNote = Notes.Blue;
                break;
            case "y":
                currentLyreNote = Notes.Yellow;
                break;
            case "c":
                currentLyreNote = Notes.Cyan;
                break;
            default:
                throw new ArgumentException($"Unknown lyre message: {msg}");
        }
    }

    public void ClearLyreNote()
    {
        currentLyreNote = null;
    }

    public bool WasLevelSuccessful()
    {
        // TODO WasLevelSuccessful - did the players succeed on the current level?
        return false;
    }

    public bool DidPlayersWin()
    {
        // TODO DidPlayersWin - did the players get the good ending?
        return false;
    }

    public void SetConductorFlower(Note? note)
    {
        Debug.Log($"Setting conductor flower to {note?.GetColorString()}");
            if (note == null)
            {
                conductorController.SendSerialMessage("k1");
                return;
            }
        if (conductorController != null)
        {
            conductorController.SendSerialMessage(note?.GetColorString() + "1");
        }
    }

    public void SetConductorLed(Note? note)
    {
        // always sets LED to black if necessary
        if (note == null)
        {
            conductorController.SendSerialMessage("k2");
            return;
        }
        // if we've finalized the LED, ignore color sends until reset
        if (conductorLedFinalized) return;
        if (conductorController != null)
        {
            conductorController.SendSerialMessage(note?.GetColorString() + "2");
        }
    }

    public void SetConductorLedFinalized(bool finalized)
    {
        conductorLedFinalized = finalized;
    }

    public void SetConductorLevel()
    {
        // TODO: this is untested/broken
        Debug.Log($"Sending conductor performance rating: {performanceRating}");
        conductorController.SendSerialMessage($"{performanceRating}");
        miscUi.text = $"Performance: {performanceRating}";
    }
}

public class GameState<TTransition> : State<GameStatus, TTransition> where TTransition : ITransition
{
    public GameState(GameStatus status, string id = null) : base(status, id) {}

    public override void Setup()
    {
        base.Setup();
        status.gameStateUI.text = ToString();
    }
}

public class GameLinearStateMachine : LinearStateMachine<GameStatus>
{
    public GameLinearStateMachine(GameStatus status, string id = null) : base(status, id) {}
}

public class GameNetworkedStateMachine : NetworkedStateMachine<GameStatus>
{
    public GameNetworkedStateMachine(GameStatus status, string id = null) : base(status, id) {}
}

public class GameParallelState : ParallelState<GameStatus>
{
    public GameParallelState(GameStatus status, string id = null) : base(status, id) {}
}

public class GameParallelDecisionState<TTransition> : ParallelDecisionState<GameStatus, TTransition> where TTransition : ITransition
{
    public GameParallelDecisionState(GameStatus status, string id = null) : base(status, id) {}
}
