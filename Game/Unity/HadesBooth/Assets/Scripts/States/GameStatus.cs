
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

    public float conductorTutorialTimeForNote;

    [Header("Lyre")]
    public float timePerFlower;
    public int numLyreFlowers;

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
        // TODO current lyre note
        return null;
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
