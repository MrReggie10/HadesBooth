
using System;
using TMPro;
using UnityEngine;

public class GameStatus : MonoBehaviour
{
    [HideInInspector] public int score;
    [HideInInspector] public int levelNum;
    [HideInInspector] public int notesPlayedThisLevel;
    [HideInInspector] public int successfulNotesPlayedThisLevel;
    
    [Header("Levels")]
    public int numLevels = 3;

    [Header("Conductor")]
    [SerializeField] protected ConductorDetector conductorDetector;
    public float conductorTimePerNoteOnFlower;
    public float conductorTimeToPlayNote;

    [Header("Lyre")]
    public float lyreTimePerFlower;
    public int numLyreFlowers;
    public float lyreAcceptWindow;
    protected Note? currentLyreNote = null;

    [Header("Misc")]
    public bool debugMode;
    public TextMeshProUGUI gameStateUI;
    public TextMeshProUGUI miscUi;

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
        Debug.Log($"Setting conductor flower to {note?.noteColor}");
        // TODO SetConductorFlower
        // if note is null, turn off flower
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
