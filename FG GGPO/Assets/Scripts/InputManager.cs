using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.Controls;
using Sirenix.OdinInspector;
using System.IO;
using System;

public class InputManager : MonoBehaviour
{
    public static Controls p1Controls;
    public static Controls p2Controls;


    public static InputManager Instance;
    public InputHandler p1Input;
    public InputHandler p2Input;
    public static bool isServer;
    public int controllersConnected;
    public List<InputDevice> controllers;
    public float switchDelay;
    public float switchCounter;

    private void Awake()
    {
        Instance = this;
        if (p1Controls == null)
            p1Controls = new Controls();
        if (p2Controls == null)
            p2Controls = new Controls();

        controllers = new List<InputDevice>();
    }

    public static string GetBindingNameP1(string actionName, int bindingIndex)
    {
        if (p1Controls == null)
            p1Controls = new Controls();

        InputAction action = p1Controls.asset.FindAction(actionName);
        return action.GetBindingDisplayString(bindingIndex);
    }

    public static string GetBindingNameP2(string actionName, int bindingIndex)
    {
        if (p2Controls == null)
            p2Controls = new Controls();

        InputAction action = p2Controls.asset.FindAction(actionName);
        return action.GetBindingDisplayString(bindingIndex);
    }

    void Start()
    {
        if (GameHandler.Instance != null)
        {
            p1Input = GameHandler.Instance.p1Transform.GetComponent<InputHandler>();
            p2Input = GameHandler.Instance.p2Transform.GetComponent<InputHandler>();
        }
        p1Input.id = 1;
        p2Input.id = 2;
        controllersConnected = Gamepad.all.Count + Joystick.all.Count;

        foreach (var item in InputSystem.devices)
        {
            if (item is Gamepad || item is Joystick)
                controllers.Add(item);
        }

        if (controllers.Count > 1)
        {
            if (controllers[1] is Gamepad)
                p2Input.SetupControls((Gamepad)controllers[1]);
            else if (controllers[1] is Joystick)
                p2Input.SetupControls((Joystick)controllers[1]);

            if (controllers[0] is Gamepad)
                p1Input.SetupControls((Gamepad)controllers[0]);
            else if (controllers[0] is Joystick)
                p1Input.SetupControls((Joystick)controllers[0]);
        }
        else if (controllers.Count == 1)
        {
            if (controllers[0] is Gamepad)
                p1Input.SetupControls((Gamepad)controllers[0]);
            else if (controllers[0] is Joystick)
                p1Input.SetupControls((Joystick)controllers[0]);

            if (GameHandler.gameModeID <= 0)
            {
                p2Input.SetupKeyboard();
            }
        }
        else
        {

            p1Input.SetupKeyboard();
        }

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
                        //print("Default");
                        break;
                }
            };
    }

    public void SwitchControls()
    {
        if (switchCounter <= 0)
        {
            switchCounter = switchDelay;
            if (GameHandler.Instance.gameMode == GameMode.TrainingMode)
            {
                if (p1Input.deviceIsAssigned)
                {
                    p1Input.DisableControls();
                    p2Input.SetupControls(Gamepad.all[0]);
                }
                else if (p2Input.deviceIsAssigned)
                {
                    p2Input.DisableControls();
                    p1Input.SetupControls(Gamepad.all[0]);
                }
            }
        }
    }

    private void FixedUpdate()
    {
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
        if (switchCounter > 0) switchCounter -= Time.unscaledDeltaTime;
    }

    public static void SaveBindingOverrideP1(InputAction action)
    {
        for (int i = 0; i < action.bindings.Count; i++)
        {
            PlayerPrefs.SetString("1" + action.actionMap + action.name + i, action.bindings[i].overridePath);
        }
    }
    public static void SaveBindingOverrideP2(InputAction action)
    {
        for (int i = 0; i < action.bindings.Count; i++)
        {
            PlayerPrefs.SetString("2" + action.actionMap + action.name + i, action.bindings[i].overridePath);
        }
    }

    public static void LoadBindingOverrideP1(string actionName)
    {
        if (p1Controls == null) p1Controls = new Controls();

        InputAction action = p1Controls.asset.FindAction(actionName);

        for (int i = 0; i < action.bindings.Count; i++)
        {
            if (!string.IsNullOrEmpty(PlayerPrefs.GetString(action.actionMap + action.name + i)))
            {
                action.ApplyBindingOverride(i, PlayerPrefs.GetString("1" + action.actionMap + action.name + i));
            }
        }
    }

    public static void LoadBindingOverrideP2(string actionName)
    {
        if (p2Controls == null) p2Controls = new Controls();

        InputAction action = p2Controls.asset.FindAction(actionName);

        for (int i = 0; i < action.bindings.Count; i++)
        {
            if (!string.IsNullOrEmpty(PlayerPrefs.GetString(action.actionMap + action.name + i)))
            {
                action.ApplyBindingOverride(i, PlayerPrefs.GetString("2" + action.actionMap + action.name + i));
            }
        }
    }

    public static void ResetBindingP1(string actionName, int bindingIndex)
    {
        InputAction action = p1Controls.asset.FindAction(actionName);

        if (action == null || action.bindings.Count <= bindingIndex)
        {
            Debug.Log("Error reset");
        }

        action.RemoveBindingOverride(bindingIndex);
        SaveBindingOverrideP1(action);
    }

    public static void ResetBindingP2(string actionName, int bindingIndex)
    {
        InputAction action = p2Controls.asset.FindAction(actionName);

        if (action == null || action.bindings.Count <= bindingIndex)
        {
            Debug.Log("Error reset");
        }

        action.RemoveBindingOverride(bindingIndex);
        SaveBindingOverrideP2(action);
    }
}
