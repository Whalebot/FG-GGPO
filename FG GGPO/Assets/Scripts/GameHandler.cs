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
    public bool showHitboxes;
    public bool disableBlock;
    public int playersInGame;
    public Transform p1Transform;
    public Transform p2Transform;

    [HideInInspector] public Status p1Status;
    [HideInInspector] public Status p2Status;

    public List<GameState> gameStates;
    public int rollbackFrames;

    public delegate void GameEvent();
    public GameEvent advanceGameState;
    public GameEvent rollbackTick;

    public delegate void RollBackEvent(int i);
    public RollBackEvent rollbackEvent;

    public int gameFrameCount;
    public int counter;

    public float hitStop;

    [FoldoutGroup("Feedback")] public float slowMotionValue;
    [FoldoutGroup("Feedback")] public float slowMotionDuration;
    [FoldoutGroup("Feedback")] public float slowMotionSmoothing;
    float startTimeStep;

    private void Awake()
    {
        Instance = this;
        gameStates = new List<GameState>();
        isPaused = true;
        p1Status = p1Transform.GetComponent<Status>();
        p2Status = p2Transform.GetComponent<Status>();

    }

    private void Start()
    {
        Debug.developerConsoleVisible = true;
        startTimeStep = Time.fixedDeltaTime;
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

    void UpdateGameState()
    {

        GameState state = new GameState(p1Transform.position, p2Transform.position, p2Transform.rotation, p2Transform.rotation);
        state.p1Health = p1Status.Health;
        state.p2Health = p2Status.Health;

        state.p1Meter = p1Status.Meter;
        state.p2Meter= p2Status.Meter;
        gameStates.Add(state);
    }



    private void FixedUpdate()
    {
        if (GameManager.Instance != null)
            isPaused = !GameManager.Instance.IsRunning;
        else isPaused = false;

        //if (hitStop > 0) {
        //    Time.timeScale = 0;
        //    return;
        //}
        //else Time.timeScale = 1;

        UpdateGameState();
        if (!isPaused)
        {
            advanceGameState?.Invoke();
            gameFrameCount++;
        }


        counter++;

        //if (counter >= 20)
        //{
        //    counter = 0;
        //    RevertGameState(gameStates.Count - rollbackFrames);
        //    ResimulateGameState();
        //}
    }

    private void Update()
    {
        if (Keyboard.current.leftCtrlKey.wasPressedThisFrame)
        {
            ResimulateGameState();
        }
    }

    [Button("Revert Game State")]
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

    public void Slowmotion(float dur)
    {
        StartCoroutine("SetSlowmotion", hitStop);
    }

    IEnumerator SetSlowmotion(float dur)
    {
        Time.timeScale = slowMotionValue;
        Time.fixedDeltaTime = startTimeStep * Time.timeScale;
        yield return new WaitForSecondsRealtime(dur);
        StartCoroutine("RevertSlowmotion");
    }
    IEnumerator RevertSlowmotion()
    {
        while (Time.timeScale < 1 && !isPaused)
        {
            Time.timeScale = Time.timeScale + slowMotionSmoothing;

            Time.fixedDeltaTime = startTimeStep * Time.timeScale;
            yield return new WaitForEndOfFrame();
        }


        Time.timeScale = 1;
        Time.fixedDeltaTime = startTimeStep;

    }

}
