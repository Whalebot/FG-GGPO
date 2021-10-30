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
    public static int gameModeID = -1;
    public static int p1Wins;
    public static int p2Wins;
    public static int p1WinStreak;
    public static int p2WinStreak;
    public GameMode gameMode;
    public bool network;
    public static bool isPaused;
    public static bool cutscene;
    public bool superFlash;
    public static int p1CharacterID;
    public static int p2CharacterID;
    public bool runNormally = true;
    public bool isMirror;
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
    [FoldoutGroup("Starting Position")] public CharacterSelectProfile[] characters;
    [FoldoutGroup("Starting Position")] public Transform p1StartPosition;
    [FoldoutGroup("Starting Position")] public Transform p2StartPosition;
    [FoldoutGroup("Starting Position")] public StagePosition startPosition;
    public int introCounter;
    public GameObject introCam1;
    public GameObject introCam2;
    public GameObject winCam1;
    public GameObject winCam2;
    [HideInInspector] public Status p1Status;
    [HideInInspector] public Status p2Status;

    public List<GameState> gameStates;
    [FoldoutGroup("Rollback")] public int rollbackFrames;

    public delegate void GameEvent();
    public GameEvent advanceGameState;
    public GameEvent revertGameState;
    public GameEvent rollbackTick;

    public GameEvent p1WinEvent;
    public GameEvent p2WinEvent;    
    public GameEvent p1RoundEvent;
    public GameEvent p2RoundEvent;

    public GameEvent hideP1Event;
    public GameEvent hideP2Event;
    public GameEvent displayP1Event;
    public GameEvent displayP2Event;

    public GameEvent p1IntroEvent;
    public GameEvent p2IntroEvent;

    public GameEvent gameStartEvent;
    public GameEvent resetEvent;

    public GameEvent roundStartEvent;
    public GameEvent gameEndEvent;
    public GameEvent rematchScreenEvent;
    public GameEvent superFlashStartEvent;
    public GameEvent superFlashEndEvent;

    public bool gameStarted;
    public delegate void RollBackEvent(int i);
    public RollBackEvent rollbackEvent;
    public RollBackEvent frameCounterEvent;

    [FoldoutGroup("Analytics")] public int roundCount;
    [FoldoutGroup("Analytics")] public int round1Time;
    [FoldoutGroup("Analytics")] public int round2Time;
    [FoldoutGroup("Analytics")] public int round3Time;

    [FoldoutGroup("Analytics")] public int round1Winner;
    [FoldoutGroup("Analytics")] public int round2Winner;
    [FoldoutGroup("Analytics")] public int round3Winner;


    public GameObject pauseMenu;
    public GameObject trainingMenu;

    [HideInInspector] public int gameFrameCount;
    [HideInInspector] public int counter;
    [HideInInspector] public float hitStop;
    float startTimeStep;

    private void Awake()
    {
        Instance = this;
        startTimeStep = Time.fixedDeltaTime;
        gameStates = new List<GameState>();
        if (gameModeID != -1)
        {
            gameMode = (GameMode)gameModeID;
        }
        isPaused = true;
        LoadCharacters();
        p1Status = p1Transform.GetComponent<Status>();
        p2Status = p2Transform.GetComponent<Status>();

    }
    private void Start()
    {
        network = FgGameManager.Instance != null;
        // Debug.developerConsoleVisible = true;

        p1Status.deathEvent += P2Win;
        p2Status.deathEvent += P1Win;

        isPaused = false;
        //cutscene = false;

        ChangeGameMode(gameMode);
        ResetAnalyticsData();
    }
    public void PauseMenu()
    {
        if (!cutscene)
        {
            if (isPaused) ResumeGame();
            else
            {
                //   if (gameMode == GameMode.VersusMode)
                {
                    pauseMenu.SetActive(true);
                    isPaused = true;
                    Time.timeScale = 0;
                }
            }
        }
        else
        {
            Skip();
        }
    }
    public void Skip()
    {
        introCounter = 0;
    }

    public void CancelPauseMenu()
    {

    }
    public void ResumeGame()
    {
        StartCoroutine(DelayResume());
    }

    IEnumerator DelayResume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        yield return new WaitForSecondsRealtime(0.2F);
        isPaused = false;

    }

    void LoadCharacters()
    {
        if (p1CharacterTrainingID != -1)
        {
            p1CharacterID = p1CharacterTrainingID;
        }
        if (p2CharacterTrainingID != -1)
        {
            p2CharacterID = p2CharacterTrainingID;
        }

        isMirror = p1CharacterID == p2CharacterID;


        if (p1Transform == null)
        {
            GameObject GO = Instantiate(characters[p1CharacterID].prefab, p1StartPosition.position, p1StartPosition.rotation);
            p1Transform = GO.transform;
            p1Status = GO.GetComponent<Status>();
        }
        if (p2Transform == null)
        {
            GameObject GO = Instantiate(characters[p2CharacterID].prefab, p2StartPosition.position, p2StartPosition.rotation);
            p2Transform = GO.transform;
            p2Status = GO.GetComponent<Status>();
        }
    }
    public void RoundStart()
    {
        // ResetAnalyticsData();
        roundStartEvent?.Invoke();
        ResetPosition();
        StartCoroutine(DelayRoundStart());
    }

    void ResetAnalyticsData()
    {
        counter = 0;

        round1Time = 0;
        round2Time = 0;
        round3Time = 0;

        round1Winner = 0;
        round2Winner = 0;
        round3Winner = 0;
    }
    IEnumerator DelayRoundStart()
    {
        cutscene = true;
        yield return new WaitForSeconds(2.5F);
        cutscene = false;
    }
    public void GameEnd(int frames)
    {
        StartCoroutine(DelayGameWin((float)frames / 60));
    }
    IEnumerator DelayGameWin(float dur)
    {
        cutscene = true;

        yield return new WaitForFixedUpdate();
        Time.timeScale = 0.1F;
        Time.fixedDeltaTime = startTimeStep * Time.timeScale;
        yield return new WaitForSecondsRealtime(2);
        Time.timeScale = 1;
        Time.fixedDeltaTime = startTimeStep;

        if (gameMode == GameMode.VersusMode)
        {
            if (winCam1 == null && p1RoundWins > p2RoundWins)
            {
                hideP2Event?.Invoke();
                displayP1Event?.Invoke();

                winCam1 = Instantiate(characters[p1CharacterID].outroPrefab, p1Transform);
                winCam1.transform.localPosition = characters[p1CharacterID].outroPrefab.transform.localPosition;
                winCam1.transform.localRotation = characters[p1CharacterID].outroPrefab.transform.localRotation;
                p1WinEvent?.Invoke();
            }
            else if (winCam2 == null && p1RoundWins < p2RoundWins)
            {
                hideP1Event?.Invoke();
                displayP2Event?.Invoke();

                winCam2 = Instantiate(characters[p2CharacterID].outroPrefab, p2Transform);
                winCam2.transform.localPosition = characters[p2CharacterID].outroPrefab.transform.localPosition;
                winCam2.transform.localRotation = characters[p2CharacterID].outroPrefab.transform.localRotation;

                p2WinEvent?.Invoke();
            }
        }

        gameEndEvent?.Invoke();
        yield return new WaitForSeconds(dur + 0.5F);
        rematchScreenEvent?.Invoke();
    }
    public void P1Win()
    {
        if (gameMode == GameMode.VersusMode)
        {
            if (roundCount == 1)
            {
                round1Time = roundTime;
                round1Winner = 1;
            }
            if (roundCount == 2)
            {
                round2Time = roundTime;
                round2Winner = 1;
            }
            if (roundCount == 3)
            {
                round3Time = roundTime;
                round3Winner = 1;
            }
            roundCount++;
            p1RoundWins++;


            if (p1RoundWins >= roundsToWin)
            {
                p1Wins++;
                p1WinStreak++;
                p2WinStreak = 0;

                GameEnd(characters[p2CharacterID].outroDuration);
            }
            else
            {
                p1RoundEvent?.Invoke();
                StartCoroutine(DelayRoundReset());
            }
        }
    }
    public void P2Win()
    {
        if (gameMode == GameMode.VersusMode)
        {
            if (roundCount == 1)
            {
                round1Time = roundTime;
                round1Winner = 2;
            }
            if (roundCount == 2)
            {
                round2Time = roundTime;
                round2Winner = 2;
            }
            if (roundCount == 3)
            {
                round3Time = roundTime;
                round3Winner = 2;
            }
            roundCount++;
            p2RoundWins++;


            if (p2RoundWins >= roundsToWin)
            {
                p2Wins++;
                p2WinStreak++;
                p1WinStreak = 0;

                GameEnd(characters[p2CharacterID].outroDuration);
            }
            else
            {
                p2RoundEvent?.Invoke();
                StartCoroutine(DelayRoundReset());
            }
        }
    }
    public void Draw()
    {
        StartCoroutine(DelayRoundReset());
    }
    IEnumerator DelayRoundReset()
    {
        if (gameMode == GameMode.VersusMode)
        {
            cutscene = true;
            yield return new WaitForSecondsRealtime(1);
            Time.timeScale = 1F;
            Time.fixedDeltaTime = startTimeStep;
 
            yield return new WaitForSecondsRealtime(2);

            cutscene = false;
        }
        ResetRound();

    }
    [Button]
    public void ResetGame()
    {
        p1Status.ResetStatus();
        p2Status.ResetStatus();
        counter = 0;
        ResetPosition();
        CameraManager.Instance.ResetCamera();
        if (gameMode == GameMode.VersusMode)
            RoundStart();
    }

    [Button]
    public void ResetRound()
    {
        resetEvent?.Invoke();
        p1Status.ResetStatus();
        p2Status.ResetStatus();
        counter = 0;
        ResetPosition();
        CameraManager.Instance.ResetCamera();
        if (gameMode == GameMode.VersusMode)
            RoundStart();
    }
    [Button]
    void ResetPosition()
    {
        switch (startPosition)
        {
            case StagePosition.RoundStart:
                p1StartPosition = StageScript.Instance.roundStartPosition[0];
                p2StartPosition = StageScript.Instance.roundStartPosition[1];
                break;
            case StagePosition.Wall1:
                p1StartPosition = StageScript.Instance.roundStartPosition[0];
                p2StartPosition = StageScript.Instance.roundStartPosition[1];
                break;
            case StagePosition.Wall2:
                p1StartPosition = StageScript.Instance.roundStartPosition[0];
                p2StartPosition = StageScript.Instance.roundStartPosition[1];
                break;
            case StagePosition.MidScreen:
                p1StartPosition = StageScript.Instance.midScreenCloseRange[0];
                p2StartPosition = StageScript.Instance.midScreenCloseRange[1];
                break;
            default:
                break;
        }
        p1Transform.position = p1StartPosition.position;
        p1Transform.rotation = p1StartPosition.rotation;

        p2Transform.position = p2StartPosition.position;
        p2Transform.rotation = p2StartPosition.rotation;
    }
    public Transform ReturnPlayer(Transform source)
    {
        if (source == p1Transform) return p2Transform;
        else return p1Transform;
    }
    public bool IsPlayer1(Transform source)
    {
        if (source == p1Transform) return true;
        else return false;
    }
    void StartGame()
    {
        isPaused = false;
    }
    [Button]
    public void AdvanceGameState()
    {
        //if (isPaused) {
        //    Physics.autoSimulation = false;
        //    return;
        //}

        if (!gameStarted)
        {
            introCounter--;
            if (gameMode == GameMode.VersusMode)
            {
                if (introCam1 == null)
                {
                    cutscene = true;
                    ResetPosition();
                    hideP2Event?.Invoke();
                    displayP1Event?.Invoke();
                    introCounter = characters[p1CharacterID].introDuration;
                    introCam1 = Instantiate(characters[p1CharacterID].introPrefab, p1Transform);
                    introCam1.transform.localPosition = characters[p1CharacterID].introPrefab.transform.localPosition;
                    introCam1.transform.localRotation = characters[p1CharacterID].introPrefab.transform.localRotation;

                    p1IntroEvent?.Invoke();
                    return;
                }
                if (introCam2 == null && introCounter <= 0)
                {
                    hideP1Event?.Invoke();
                    displayP2Event?.Invoke();
                    introCounter = characters[p2CharacterID].introDuration;
                    introCam2 = Instantiate(characters[p2CharacterID].introPrefab, p2Transform);
                    introCam2.transform.localPosition = characters[p2CharacterID].introPrefab.transform.localPosition;
                    introCam2.transform.localRotation = characters[p2CharacterID].introPrefab.transform.localRotation;
                    p2IntroEvent?.Invoke();
                    return;
                }
            }
            if (introCounter <= 0)
            {
                cutscene = false;
                if (gameMode == GameMode.VersusMode)
                {
                    
                    displayP1Event?.Invoke();
                    displayP2Event?.Invoke();
                    RoundStart();
                }
                gameStarted = true;
                //gameStartEvent?.Invoke();
            }
            return;
        }

        CameraManager.Instance.canCrossUp = p1Status.groundState == GroundState.Airborne || p2Status.groundState == GroundState.Airborne || p1Status.crossupState || p2Status.crossupState;
        advanceGameState?.Invoke();

        gameFrameCount++;
        frameCounterEvent?.Invoke(gameFrameCount);
        if (!cutscene && gameMode == GameMode.VersusMode)
        {
            counter++;
        }
        roundTime = Mathf.Clamp(maxRoundTime - (int)(counter / 60), 0, maxRoundTime);
        UpdateGameState();
        if (roundTime <= 0 && !cutscene)
        {
            TimeoutFinish();
        }
    }
    public void StartSuperFlash()
    {
        superFlash = true;
        superFlashStartEvent?.Invoke();
        Physics.autoSimulation = false;
    }
    public void EndSuperFlash()
    {
        superFlash = false;
        superFlashEndEvent?.Invoke();
        if (runNormally)
            Physics.autoSimulation = true;
    }
    [Button]
    public void AdvanceGameStateButton()
    {
        runNormally = false;

        if (!superFlash)
        {
            Physics.autoSimulation = false;
            Physics.Simulate(Time.fixedDeltaTime);
        }
        AdvanceGameState();
    }
    public void TimeoutFinish()
    {
        if (p1Status.Health > p2Status.Health)
            P1Win();
        else if (p1Status.Health < p2Status.Health)
            P2Win();
        else Draw();
    }
    [Button]
    public void NormalGameState()
    {
        runNormally = true;
        Physics.autoSimulation = true;
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
        else
        {
            // isPaused = false;
        }

        if (runNormally && !network)
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
        else if (Keyboard.current.numpad5Key.wasPressedThisFrame)
        {
            NormalGameState();
        }

        if (Keyboard.current.f1Key.wasPressedThisFrame)
        {
            ChangeGameMode(GameMode.VersusMode);
        }
        else if (Keyboard.current.f2Key.wasPressedThisFrame)
        {
            ChangeGameMode(GameMode.TrainingMode);
        }
        else if (Keyboard.current.f3Key.wasPressedThisFrame)
        {
            ChangeGameMode(GameMode.TrialMode);
        }
    }
    public void ChangeGameMode(GameMode mode)
    {
        gameMode = mode;
        UIManager.Instance.GameModeUI(gameMode);
    }
    public void Rollback(int frameTarget)
    {
        Debug.Log("Rollbacking from " + gameFrameCount + " to " + frameTarget);
        gameFrameCount = frameTarget;
        RevertGameState(gameFrameCount);
    }
    void RevertGameState(int i)
    {
        //rollbackEvent?.Invoke(i);

        p1Transform.position = gameStates[gameStates.Count - 1].p1Position;
        p2Transform.position = gameStates[gameStates.Count - 1].p2Position;

        p1Status.rb.velocity = gameStates[gameStates.Count - 1].p1Velocity;
        p2Status.rb.velocity = gameStates[gameStates.Count - 1].p2Velocity;
        gameStates.RemoveRange(i, gameStates.Count - i);
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
