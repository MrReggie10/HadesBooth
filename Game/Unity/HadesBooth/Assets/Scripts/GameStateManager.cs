/*
Manages the game's state
see https://miro.com/app/board/uXjVGztfbBg=/ for the Finite State Machine
By: Taylor Roberts, 
*/


using UnityEngine;
using TMPro;

public enum GameState { Wait, Tutorial1, Tutorial1Main, Tutorial1Correct, Tutorial2 }

public class GameStateManager : MonoBehaviour
{
    public GameState state = GameState.Wait;

    [Header("Tutorials")]
    [SerializeField] private int tutorial1NotesPlayed;
    private int notesPlayed = 0;
    private Note note = Notes.Red;
    private Note[] lyreTutorial = new Note[]
    {
        Notes.Red,
        Notes.Blue,
        Notes.Yellow,
        Notes.Cyan
    };

    [Header("Misc")]
    [SerializeField] private bool debugMode;
    [SerializeField] public TextMeshProUGUI GameStateUI;
    [SerializeField] public TextMeshProUGUI miscUI;
    // TODO: add keybinds to skip through states

    void Start()
    {

    }

    void Update()
    {
        GameStateUI.text = state.ToString();
        switch (state)
        {
            case GameState.Wait:
                // TODO: start game only when the user presses a button on the lyre (or something)
                if (debugMode && Input.anyKeyDown)
                {
                    state = GameState.Tutorial1;
                    Debug.Log("Transitioning from Wait -> Tutorial1");
                }
                break;
            case GameState.Tutorial1:
                notesPlayed = 0;
                state = GameState.Tutorial1Main;
                Debug.Log("Transitioning from Tutorial1 -> Tutorial1Main");
                break;
            case GameState.Tutorial1Main:
                if (notesPlayed >= tutorial1NotesPlayed)
                {
                    state = GameState.Tutorial2;
                    Debug.Log("Transitioning from Tutorial1Main-> Tutorial2");
                }
                else
                {
                    note = Notes.Red; // TODO: note = get conductor note (wait for conductor to finish their stuff)

                    miscUI.text = "note: " + note.noteColor.ToString() + "\n" + "notesPlayed: " + notesPlayed.ToString();
                    if (note == lyreTutorial[notesPlayed])
                    {
                        state = GameState.Tutorial1Correct;
                        Debug.Log("Transitioning from Tutorial1Main -> Tutorial1Correct");
                    }
                }
                break;
            case GameState.Tutorial1Correct:
                // play note audio
                notesPlayed += 1;
                state = GameState.Tutorial1Main;
                Debug.Log("Transitioning from Tutorial1Correct -> Tutorial1Main");
                break;
            case GameState.Tutorial2:
                notesPlayed = 0;
                // state = GameState.Tutorial2Main;
                break;
        }
    }
}
