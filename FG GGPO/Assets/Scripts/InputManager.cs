using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.Controls;
using Sirenix.OdinInspector;
using System.IO;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    [HideInInspector] public InputHandler p1Input;
    [HideInInspector] public InputHandler p2Input;
    public static bool isServer;
    public int controllersConnected;
    public bool replay;
    public InputLog log;
    public InputLog replayLog;
    public int replayID;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        GameHandler.Instance.advanceGameState += UpdateLog;
        p1Input = GameHandler.Instance.p1Transform.GetComponent<InputHandler>();
        p2Input = GameHandler.Instance.p2Transform.GetComponent<InputHandler>();
        p1Input.id = 1;
        p2Input.id = 2;
        controllersConnected = Gamepad.all.Count;

        foreach (var item in Gamepad.all)
        {
            print(item.name);
        }

        //for (int i = 0; i < controllersConnected; i++)
        //{
        //    p1Input.SetupControls(Gamepad.all[i]);

        //}


        if (Gamepad.all.Count > 1)
        {

            p2Input.SetupControls(Gamepad.all[1]);
            p1Input.SetupControls(Gamepad.all[0]);
        }
        else p1Input.SetupControls(Gamepad.all[0]);

        InputSystem.onDeviceChange += (device, change) =>
            {
                switch (change)
                {
                    case InputDeviceChange.Added:
                        // New Device.
                        print("Added");
                        break;
                    case InputDeviceChange.Disconnected:
                        // Device got unplugged.
                        print("Disconeccted");
                        break;
                    case InputDeviceChange.Reconnected:
                        // Plugged back in.
                        print("Reconnected");
                        break;
                    case InputDeviceChange.Removed:
                        // Remove from Input System entirely; by default, Devices stay in the system once discovered.
                        print("Totally removed");
                        break;
                    default:
                        // See InputDeviceChange reference for other event types.
                        print("Default");
                        break;
                }
            };

        if (replay) LoadLog();
    }

    private void FixedUpdate()
    {
        if (replay) ReplayLog();
    }

    [Button]
    public void SaveLog()
    {
        string jsonData = JsonUtility.ToJson(log, true);
        File.WriteAllText(Application.persistentDataPath + "/inputLog.json", jsonData);
    }

    [Button]
    public void LoadLog()
    {
        replayLog = JsonUtility.FromJson<InputLog>(File.ReadAllText(Application.persistentDataPath + "/inputLog.json"));
    }

    public void ReplayLog()
    {

        if (replayLog.inputs.Count > GameHandler.Instance.gameFrameCount)
        {
            if (replayID == 0)
            {
                p1Input.ResolveButtons(replayLog.inputs[GameHandler.Instance.gameFrameCount].buttons);
                for (int i = 0; i < p1Input.netDirectionals.Length; i++)
                {
                    p1Input.netDirectionals[i] = replayLog.inputs[GameHandler.Instance.gameFrameCount].directionals[i];
                }
            }
            else
            {
                p2Input.ResolveButtons(replayLog.inputs[GameHandler.Instance.gameFrameCount].buttons);
                for (int i = 0; i < p2Input.netDirectionals.Length; i++)
                {
                    p2Input.netDirectionals[i] = replayLog.inputs[GameHandler.Instance.gameFrameCount].directionals[i];
                }
            }
        }
    }

    public void UpdateLog()
    {
        Input temp = new Input();
        temp.frame = GameHandler.Instance.gameFrameCount;


        for (int i = 0; i < p1Input.netButtons.Length; i++)
        {
            temp.buttons[i] = p1Input.netButtons[i];
        }
        for (int i = 0; i < p1Input.netDirectionals.Length; i++)
        {
            temp.directionals[i] = p1Input.netDirectionals[i];
        }

        log.inputs.Add(temp);

    }

    public InputHandler OnlineInput()
    {
        if (isServer) return p1Input;
        else return p2Input;
    }
    public InputHandler OpponentInput()
    {
        if (!isServer) return p1Input;
        else return p2Input;
    }

    void AssignInputsToDevices()
    {

    }

    void Update()
    {
        //if (Keyboard.current.anyKey.wasPressedThisFrame) {
        //    p2Input.SetupKeyboard();
        //}

        //p1Input.network = !isServer;
        //p2Input.network = isServer;
    }


}
