/*
Manages the game's state
see https://miro.com/app/board/uXjVGztfbBg=/ for the Finite State Machine
By: Taylor Roberts, Ben Morris, Devika Santosh
*/

using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    // TODO: add keybinds to skip through states
    public State<GameStatus, DefaultTransition> state;

    [SerializeField] protected GameStatus gameStatus;

    private void Awake()
    {
        GameNetworkedStateMachine stateMachine = new GameNetworkedStateMachine(gameStatus);

        ResetAndWait reset = new ResetAndWait(gameStatus, id:"Reset and wait for next game");
        ConductorTutorial conductorTutorial = new ConductorTutorial(gameStatus, "Conductor tutorial");
        LyreTutorial lyreTutorial = new LyreTutorial(gameStatus, "Lyre tutorial");
        LevelState[] levels = new LevelState[gameStatus.numLevels];
        for (int levelNum = 1; levelNum <= gameStatus.numLevels; levelNum++)
        {
            levels[levelNum - 1] = new LevelState(gameStatus, levelNum, $"Level {levelNum}");
        }
        ShowEnding ending = new ShowEnding(gameStatus);
        
        stateMachine.SetInitialState(reset);
        stateMachine.AddTransition(reset, conductorTutorial);
        stateMachine.AddTransition(conductorTutorial, lyreTutorial);
        stateMachine.AddTransition(lyreTutorial, levels[0]);
        for (int idx = 1; idx < levels.Length; idx++)
        {
            stateMachine.AddTransition(levels[idx - 1], levels[idx]);
        }
        stateMachine.AddTransition(levels[^1], ending);
        stateMachine.AddTransition(ending, reset);

        state = stateMachine;
    }

    void Start()
    {
        state.Setup();
    }

    void Update()
    {
        state.Update();
    }

    private void LateUpdate()
    {
        // probably not needed but just in case we throw things in there
        state.LateUpdate();
    }
}
