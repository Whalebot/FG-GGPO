using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;
using UnityEngine.InputSystem;
using SharedGame;

public class GameHandler : MonoBehaviour
{
    public static GameHandler Instance;
    public static bool isPaused;
    public static bool cutscene;
    public static int p1CharacterID;
    public static int p2CharacterID;
    public bool runNormally = true;

    public bool showHitboxes;
    public bool showHurtboxes;
    public static bool staticHurtboxes;
    public int playersInGame;
    public Transform p1Transform;
    public Transform p2Transform;

    [FoldoutGroup("Gameplay Settings")] public int maxRoundTime;
    [FoldoutGroup("Gameplay Settings")] public int roundTime;
    [FoldoutGroup("Gameplay Settings")] public int roundsToWin;
    [FoldoutGroup("Gameplay Settings")] public int p1RoundWins;
    [FoldoutGroup("Gameplay Settings")] public int p2RoundWins;
    [FoldoutGroup("Starting Position")] public int p1CharacterTrainingID;
    [FoldoutGroup("Starting Position")] public int p2CharacterTrainingID;
    [FoldoutGroup("Starting Position")] public GameObject[] characters;
    [FoldoutGroup("Starting Position")] public Transform p1StartPosition;
    [FoldoutGroup("Starting Position")] public Transform p2StartPosition;

    [HideInInspector] public Status p1Status;
    [HideInInspector] public Status p2Status;

    public List<GameState> gameStates;
    [FoldoutGroup("Rollback")] public int rollbackFrames;

    public delegate void GameEvent();
    public GameEvent advanceGameState;
    public GameEvent revertGameState;
    public GameEvent rollbackTick;

    public delegate void RollBackEvent(int i);
    public RollBackEvent rollbackEvent;

    [HideInInspector] public int gameFrameCount;
    [HideInInspector] public int counter;

    [HideInInspector] public float hitStop;
    float startTimeStep;

    private void Awake()
    {
        Instance = this;
        gameStates = new List<GameState>();
        isPaused = true;
        LoadCharacters();
        p1Status = p1Transform.GetComponent<Status>();
        p2Status = p2Transform.GetComponent<Status>();

    }

    private void Start()
    {
        Debug.developerConsoleVisible = true;
        startTimeStep = Time.fixedDeltaTime;

        p1Status.deathEvent += P2Win;
        p2Status.deathEvent += P1Win;
    }



    void LoadCharacters() {
        if (p1CharacterTrainingID != -1) {
            p1CharacterID = p1CharacterTrainingID;
        }
        if (p2CharacterTrainingID != -1) {
            p2CharacterID = p2CharacterTrainingID;
        }

        if (p1Transform == null) {
           GameObject GO = Instantiate(characters[p1CharacterID], p1StartPosition.position, p1StartPosition.rotation);
            p1Transform = GO.transform;
            p1Status = GO.GetComponent<Status>();
        }
        if (p2Transform == null)
        {
            GameObject GO = Instantiate(characters[p2CharacterID], p2StartPosition.position, p2StartPosition.rotation);
            p2Transform = GO.transform;
            p2Status = GO.GetComponent<Status>();
        }
    }

    public void GameEnd() {
        StartCoroutine(DelayGameWin());
    }


    IEnumerator DelayGameWin()
    {
        cutscene = true;
        yield return new WaitForSeconds(3);
        cutscene = false;
        StageManager.Instance.RestartScene();
    }


    public void P1Win()
    {
        p1RoundWins++;
        if (p1RoundWins >= roundsToWin)
        {
            GameEnd();
        }
        else
        {
            StartCoroutine(DelayRoundReset());
        }
    }

    public void P2Win()
    {
        p2RoundWins++;
        if (p2RoundWins >= roundsToWin)
        {
            GameEnd();
        }
        else
        {
            StartCoroutine(DelayRoundReset());
        }
    }

    IEnumerator DelayRoundReset() {
        cutscene = true;
        yield return new WaitForSeconds(3);
        cutscene = false;
        ResetRound();
    }

    [Button]
    public void ResetRound()
    {
        p1Status.ResetStatus();
        p2Status.ResetStatus();
        ResetPosition();
    }

    [Button]
    void ResetPosition()
    {
        p1Transform.position = p1StartPosition.position;
        p1Transform.rotation = p1StartPosition.rotation;

        p2Transform.position = p2StartPosition.position;
        p2Transform.rotation = p2StartPosition.rotation;
    }


    private void OnDestroy()
    {

    }

    public Transform ReturnPlayer(Transform source)
    {
        if (source == p1Transform) return p2Transform;
        else return p1Transform;
    }

    void StartGame()
    {
        isPaused = false;
    }

    [Button]
    public void AdvanceGameState()
    {
        advanceGameState?.Invoke();
        UpdateGameState();
        gameFrameCount++;
        counter++;
        roundTime = Mathf.Clamp(maxRoundTime - (int)(counter / 60), 0, maxRoundTime);
    }

    [Button]
    public void AdvanceGameStateButton()
    {
        runNormally = false;
        Physics.autoSimulation = false;
        Physics.Simulate(Time.fixedDeltaTime);
        rollbackTick?.Invoke();
        advanceGameState?.Invoke();
        UpdateGameState();
        gameFrameCount++;
        counter++;
        roundTime = Mathf.Clamp(maxRoundTime - (int)(counter / 60), 0, maxRoundTime);


    }

    [Button]
    public void RevertGameState()
    {
        revertGameState?.Invoke();
        //UpdateGameState();
        gameFrameCount--;
        counter--;
        roundTime = Mathf.Clamp(maxRoundTime - (int)(counter / 60), 0, maxRoundTime);
    }


    void UpdateGameState()
    {

        GameState state = new GameState(p1Transform.position, p2Transform.position, p2Transform.rotation, p2Transform.rotation);
        state.p1Health = p1Status.Health;
        state.p2Health = p2Status.Health;

        state.p1Meter = p1Status.Meter;
        state.p2Meter = p2Status.Meter;
        gameStates.Add(state);
    }



    private void FixedUpdate()
    {
        if (GameManager.Instance != null)
            isPaused = !GameManager.Instance.IsRunning;
        else isPaused = false;

        CameraManager.Instance.canCrossUp = p1Status.groundState == GroundState.Airborne || p2Status.groundState == GroundState.Airborne || p1Status.crossupState || p2Status.crossupState;


        if (!isPaused && runNormally)
        {
            AdvanceGameState();

        }
    }

    private void OnValidate()
    {
        staticHurtboxes = showHurtboxes;
    }

    private void Update()
    {
        staticHurtboxes = showHurtboxes;
        if (Keyboard.current.numpad4Key.wasPressedThisFrame)
        {
            ResimulateGameState();
        }
        else if (Keyboard.current.numpad6Key.wasPressedThisFrame)
        {
            AdvanceGameStateButton();
        }
    }


    void RevertGameState(int i)
    {
        rollbackEvent?.Invoke(i);
        gameStates.RemoveRange(i, gameStates.Count - i);
        p1Transform.position = gameStates[gameStates.Count - 1].p1Position;
    }

    [Button("Simulate Game State")]
    public void ResimulateGameState()
    {
        Physics.autoSimulation = false;
        rollbackEvent?.Invoke(rollbackFrames);
        for (int i = 0; i < rollbackFrames; i++)
        {
            rollbackTick?.Invoke();
            Physics.Simulate(Time.fixedDeltaTime);
            UpdateGameState();
        }
        Physics.autoSimulation = true;
    }
}
