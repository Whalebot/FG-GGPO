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

    public Status p1Status;
    public Status p2Status;

    public List<GameState> gameStates;
    public int rollbackFrames;

    public delegate void GameEvent();
    public GameEvent advanceGameState;
    public GameEvent rollbackTick;

    public delegate void RollBackEvent(int i);
    public RollBackEvent rollbackEvent;
    
    public int gameFrameCount;
    public int counter;
    private void Awake()
    {
        Instance = this;
        gameStates = new List<GameState>();
        isPaused = true;

    }

    private void OnDestroy()
    {

    }

    void StartGame() {
        isPaused = false;
    }

    void UpdateGameState()
    {
      
        GameState state = new GameState(p1Transform.position, p2Transform.position, p2Transform.rotation, p2Transform.rotation);
        state.p1Health = p1Status.health;
        state.p2Health = p2Status.health;
        gameStates.Add(state);
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance != null)
            isPaused = !GameManager.Instance.IsRunning;
        else isPaused = false;


        UpdateGameState();
        if (!isPaused) {
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

        if (Keyboard.current.leftCtrlKey.wasPressedThisFrame) {
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
}
