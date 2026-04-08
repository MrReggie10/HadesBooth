
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class NoteTiming
{
    public NoteColor noteColor;
    public int msSinceLevelStart;

    public Note note
    {
        get
        {
            switch (noteColor)
            {
                case NoteColor.Red: return Notes.Red;
                case NoteColor.Blue: return Notes.Blue;
                case NoteColor.Yellow: return Notes.Yellow;
                case NoteColor.Cyan: return Notes.Cyan;
                default: throw new ArgumentException($"Unknown note color: {noteColor}");
            }
        }
    }
}

public struct FlowerWallTiming
{
    public Note note;
    public bool turnOn;
    public int flowerIdx;
    public int delayMs;

    public FlowerWallTiming(Note note, bool turnOn, int flowerIdx, int delayMs)
    {
        this.note = note;
        this.turnOn = turnOn;
        this.flowerIdx = flowerIdx;
        this.delayMs = delayMs;
    }
}

[Serializable]
public class LevelTiming
{
    public NoteTiming[] conductorNotes;
    public NoteTiming[] lyreNotes;
    public int lyreMsPerFlower;
    public int measureLengthMs;
    public int acceptableDifferenceMs;

    public List<FlowerWallTiming> GetFlowerWallTimings(int numFlowers)
    {
        List<FlowerWallTiming> timings = new List<FlowerWallTiming>();
        foreach (NoteTiming note in lyreNotes)
        {
            for (int flowerIdx = 0; flowerIdx < numFlowers; flowerIdx++)
            {
                FlowerWallTiming onTiming = new FlowerWallTiming(note.note, true, flowerIdx, note.msSinceLevelStart + flowerIdx * lyreMsPerFlower);
                FlowerWallTiming offTiming = new FlowerWallTiming(note.note, false, flowerIdx, note.msSinceLevelStart + (flowerIdx + 1) * lyreMsPerFlower);
                timings.Add(onTiming);
                timings.Add(offTiming);
            }
        }

        timings.Sort((a, b) => a.delayMs - b.delayMs);
        HashSet<FlowerWallTiming> toRemove = new HashSet<FlowerWallTiming>();
        for (int idx = 0; idx < timings.Count - 1; idx++)
        {
            FlowerWallTiming current = timings[idx];
            List<List<FlowerWallTiming>> within5ms = new List<List<FlowerWallTiming>>();
            for (int flowerIdx = 0; flowerIdx < numFlowers; flowerIdx++) within5ms.Add(new List<FlowerWallTiming>());
            within5ms[current.flowerIdx].Add(current);
            
            int nextIdx;
            for (nextIdx = idx + 1;
                 nextIdx < timings.Count && timings[nextIdx].delayMs - current.delayMs <= 5;
                 nextIdx++)
            {
                within5ms[timings[nextIdx].flowerIdx].Add(timings[nextIdx]);
            }

            for (int flowerIdx = 0; flowerIdx < numFlowers; flowerIdx++)
            {
                List<FlowerWallTiming> sameTime = within5ms[flowerIdx];
                if (sameTime.Count <= 1) continue;
                int toKeepIdx = sameTime.FindIndex(t => t.turnOn);
                if (toKeepIdx == -1) continue;
                for (int removeIdx = 0; removeIdx < sameTime.Count; removeIdx++)
                {
                    if (removeIdx != toKeepIdx) toRemove.Add(sameTime[removeIdx]);
                }
            }
            
            idx = nextIdx - 1;
        }

        timings.RemoveAll(toRemove.Contains);

        return timings;
    }
}

public class GameStatus : MonoBehaviour
{
    [HideInInspector] public int score;
    [HideInInspector] public int levelNum;
    [HideInInspector] public int performanceRating; // 0-3
    [HideInInspector] public int notesPlayedThisLevel;
    [HideInInspector] public int successfulNotesPlayedThisLevel;

    [Header("Levels")]
    public LevelTiming[] levelTimings; // level 0 will be tutorial
    public int numLevels => levelTimings.Length - 1;  // -1 because tutorial

    [Header("Conductor")]
    [SerializeField] protected ConductorDetector conductorDetector;
    public SerialController conductorController;

    [Header("Lyre")]
    public int numLyreFlowers;
    public float lyreAcceptWindow;
    protected Note?[] currentLyreNotes = {null, null};
    public int numLyres => currentLyreNotes.Length;
    public SerialController lyreController;

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

    public Note? CurrentLyreNote(int lyreIdx = 0)
    {
        return currentLyreNotes[lyreIdx];
    }

    public void HandleMessage(string msg, int lyreIdx = 0)
    {
        Note note;
        switch (msg)
        {
            case "r":
                note = Notes.Red;
                break;
            case "b":
                note = Notes.Blue;
                break;
            case "y":
                note = Notes.Yellow;
                break;
            case "c":
                note = Notes.Cyan;
                break;
            default:
                throw new ArgumentException($"Unknown lyre message: {msg}");
        }

        currentLyreNotes[lyreIdx] = note;
    }

    public void ClearLyreNote(int lyreIdx = 0)
    {
        currentLyreNotes[lyreIdx] = null;
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
        if (conductorController != null)
        {
            if (note == null)
            {
                conductorController.SendSerialMessage("k1");
                return;
            }
            conductorController.SendSerialMessage(note?.GetColorString() + "1");
        }
    }

    public void SetConductorLed(Note? note)
    {
        if (conductorController != null)
        {
            // always sets LED to black if necessary
            if (note == null)
            {
                conductorController.SendSerialMessage("k2");
                return;
            }
            // if we've finalized the LED, ignore color sends until reset
            if (conductorLedFinalized) return;
                conductorController.SendSerialMessage(note?.GetColorString() + "2");
        }
    }

    public void SetConductorLedFinalized(bool finalized)
    {
        conductorLedFinalized = finalized;
    }

    public void SetConductorLevel()
    {
        if (conductorController != null)
        {
            Debug.Log($"Sending conductor performance rating: {performanceRating}");
            conductorController.SendSerialMessage($"{performanceRating}");
            miscUi.text = $"Performance: {performanceRating}";
        }
    }

    public void SendFlowerWallTimings(int levelNum)
    {
        List<FlowerWallTiming> timings = levelTimings[levelNum].GetFlowerWallTimings(numLyreFlowers);
        string message = $"L {timings.Count}";
        int offset = levelTimings[levelNum].lyreMsPerFlower * numLyreFlowers;
        foreach (FlowerWallTiming timing in timings)
        {
            message += $" {timing.delayMs - offset} {timing.flowerIdx} {timing.note.GetColorString()}";
        }
        lyreController?.SendSerialMessage(message);
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
