
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

    public string colorString => turnOn ? note.GetColorString() : "k";

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
    public int minNotesToSucceed;

    public List<FlowerWallTiming> GetFlowerWallTimings(int numFlowers)
    {
        List<FlowerWallTiming> timings = new List<FlowerWallTiming>();
        foreach (NoteTiming note in lyreNotes)
        {
            for (int flowerIdx = 0; flowerIdx < numFlowers; flowerIdx++)
            {
                int timeOn = note.msSinceLevelStart - lyreMsPerFlower * (numFlowers - 1 - flowerIdx) - lyreMsPerFlower / 2;
                FlowerWallTiming onTiming = new FlowerWallTiming(note.note, true, flowerIdx, timeOn);
                FlowerWallTiming offTiming = new FlowerWallTiming(note.note, false, flowerIdx, timeOn + lyreMsPerFlower);
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
    [HideInInspector] public int performanceRating; // 0-6
    [HideInInspector] public int notesPlayedThisLevel;
    [HideInInspector] public int successfulNotesPlayedThisLevel;

    [Header("Levels")]
    public LevelTiming[] levelTimings;
    public int numLevels => levelTimings.Length;
    protected bool?[] didSucceedLevel;
    public int minScoreToWin;
    public AudioClip levelWinSfx;
    public AudioClip levelLoseSfx;

    [Header("Conductor")]
    [SerializeField] protected ConductorDetector conductorDetector;
    public SerialController conductorController;
    public AudioClip conductorSuccessSfx;
    public AudioClip conductorFailSfx;
    // blocks indicator LED change
    protected bool conductorLedFinalized = false;

    [Header("Lyre")]
    public int numLyreFlowers;
    protected Note?[] currentLyreNotes = {null, null};
    public int numLyres => currentLyreNotes.Length;
    public SerialController lyreController;
    public AudioClip lyreSuccessSfx;
    public AudioClip lyreFailSfx;
    
    [Header("BGM")]
    public AudioSource[] bgmSources; // must be exactly 8 elements
    public AudioClip[] bgms; // must be exactly 8 elements
    public SerialController flowerWall;

    [Header("Misc")]
    public TextMeshProUGUI gameStateUI;
    public TextMeshProUGUI miscUi;
    public DmxSender dmx;
    public float endingTime;
    public AudioSource sfxSource;
    public bool useConductor;
    public bool backgroundConductorLed;

    public void Awake()
    {
        didSucceedLevel = new bool?[levelTimings.Length];
        Reset();
    }

    public void Update()
    {
        if (backgroundConductorLed)
        {
            SetConductorLed(CurrentConductorNote());
        }
    }

    public void Reset()
    {
        for (int idx = 0; idx < numLevels; idx++)
        {
            didSucceedLevel[idx] = null;
        }
        score = 0;
        levelNum = 0;
        notesPlayedThisLevel = 0;
        successfulNotesPlayedThisLevel = 0;
        performanceRating = 1;
        SetConductorLevel();
        dmx.PlayCue(Cue.Wait);
    }

    public void OnLevelStart(int level)
    {
        for (int idx = 0; idx < 8; idx++)
        {
            bgmSources[idx].PlayOneShot(bgms[idx]);
            bgmSources[idx].volume = idx == 0 ? 1f : 0f;
        }
        notesPlayedThisLevel = 0;
        successfulNotesPlayedThisLevel = 0;
        levelNum = level;
        SetConductorLedFinalized(false);
        SetConductorLevel();
        SetLevelLights();
    }

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

    public void OnLevelEnd()
    {
        score += successfulNotesPlayedThisLevel;
        didSucceedLevel[levelNum] = successfulNotesPlayedThisLevel >= levelTimings[levelNum].minNotesToSucceed;

        int activeIdx = 0;
        if (didSucceedLevel[3] ?? false) activeIdx += 4;
        if (didSucceedLevel[5] ?? false) activeIdx += 2;
        if (levelNum >= numLevels && DidPlayersWin()) activeIdx += 1;
        for (int idx = 0; idx < 8; idx++)
        {
            bgmSources[idx].volume = idx == activeIdx ? 1f : 0f;
        }
    }

    public bool WasLevelSuccessful()
    {
        return successfulNotesPlayedThisLevel >= levelTimings[levelNum].minNotesToSucceed;
    }

    public bool DidPlayersWin()
    {
        return score >= minScoreToWin;
    }

    public void SetConductorFlower(Note? note)
    {
        // Debug.Log($"Setting conductor flower to {note?.GetColorString()}");
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
        if (flowerWall != null)
        {
            Debug.Log($"Sending conductor performance rating: {performanceRating}");
            flowerWall.SendSerialMessage($"{performanceRating}");
            miscUi.text = $"Performance: {performanceRating}";
        }
    }
    
    public void SetLevelLights()
    {
        // performance in 0-6, but cues are in 0-3, so divide by 2 and floor
        int cue = (int) Mathf.Clamp(performanceRating / 2, 0, 3);
        Cue[] cues = { Cue.Spring0, Cue.Spring1, Cue.Spring2, Cue.Spring3 };
        dmx.PlayCue(cues[cue]);
    }

    public void SendFlowerWallTimings(int levelNum)
    {
        List<FlowerWallTiming> timings = levelTimings[levelNum].GetFlowerWallTimings(numLyreFlowers);
        string message = $"L {timings.Count}";
        foreach (FlowerWallTiming timing in timings)
        {
            message += $" {timing.delayMs} {timing.flowerIdx} {timing.colorString}";
        }
        Debug.Log("message: {message}");
        flowerWall?.SendSerialMessage(message);
    }

    public void PlayLyreSfx(bool success)
    {
        PlaySfx(success, lyreSuccessSfx, lyreFailSfx);
    }

    public void PlayConductorSfx(bool success)
    {
        PlaySfx(success, conductorSuccessSfx, conductorFailSfx);
    }

    public void PlayLevelEndSfx(bool success)
    {
        PlaySfx(success, levelWinSfx, levelLoseSfx);
    }

    protected void PlaySfx(bool success, AudioClip onSuccess, AudioClip onFail)
    {
        AudioClip sfx = success ? onSuccess : onFail;
        if (sfx) sfxSource.PlayOneShot(sfx);
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
