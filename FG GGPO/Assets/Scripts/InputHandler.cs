using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.Controls;
using Sirenix.OdinInspector;

public class InputHandler : MonoBehaviour
{
    public bool deviceIsAssigned;
    public bool network;
    public int id;
    public int lastInput = 5;
    public ControlScheme controlScheme = ControlScheme.PS4;
    public delegate void InputEvent();

    public Controls controls = null;

    public List<int> inputQueue;
    public float bufferWindow;

    public InputEvent controlSchemeChange;
    public InputEvent keyboardEvent;
    public InputEvent gamepadEvent;

    public InputEvent westInput;
    public InputEvent northInput;
    public InputEvent eastInput;
    public InputEvent southInput;

    public InputEvent westRelease;
    public InputEvent northRelease;
    public InputEvent southRelease;
    public InputEvent eastRelease;

    public InputEvent startInput;
    public InputEvent selectInput;
    public InputEvent R1input;
    public InputEvent R1release;
    public InputEvent R2input;
    public InputEvent R2release;
    public InputEvent L1input;
    public InputEvent L2input;
    public InputEvent L2release;
    public InputEvent R3input;
    public InputEvent R3Right;
    public InputEvent R3Left;
    public InputEvent touchPadInput;
    public InputEvent leftInput;
    public InputEvent rightInput;

    [HideInInspector] public bool R1Hold;
    [HideInInspector] public bool R2Hold;
    [HideInInspector] public bool L1Hold;
    [HideInInspector] public bool L2Hold;

    public Vector2 dpad;
    public Vector2 inputDirection;
    [FoldoutGroup("Network Input")] public bool[] netDirectionals = new bool[4];
    [FoldoutGroup("Input Overlay")] public bool[] heldDirectionals = new bool[4];
    [FoldoutGroup("Input Overlay")] public bool[] netButtons = new bool[6];
    [FoldoutGroup("Input Overlay")] public bool[] heldButtons = new bool[6];
    [FoldoutGroup("Input Overlay")] public List<int> directionals;
    [FoldoutGroup("Input Overlay")] public bool updatedDirectionals;
    [FoldoutGroup("Input Overlay")] public bool updatedButtons;

    public bool debug;

    // private void OnEnable() => controls.Default.Enable();
    private void OnDisable()
    {
        if (deviceIsAssigned)
            controls.Default.Disable();
    }

    void Start()
    {
    }

    public void SetupControls(Gamepad device)
    {
        if (deviceIsAssigned) return;
        deviceIsAssigned = true;

        var user = InputUser.PerformPairingWithDevice(device, default(InputUser), InputUserPairingOptions.None);

        controls = new Controls();

        controls.Default.West.performed += context => OnWest(context);
        controls.Default.West.canceled += context => OnWest(context);
        controls.Default.North.performed += context => OnNorth(context);
        controls.Default.North.canceled += context => OnNorth(context);
        controls.Default.South.performed += context => OnSouth(context);
        controls.Default.South.canceled += context => OnSouth(context);
        controls.Default.East.performed += context => OnEast(context);
        controls.Default.East.canceled += context => OnEast(context);

        controls.Default.Up.performed += context => OnUp(context);
        controls.Default.Left.performed += context => OnLeft(context);
        controls.Default.Right.performed += context => OnRight(context);
        controls.Default.Down.performed += context => OnDown(context);
        controls.Default.Up.canceled += context => OnUp(context);
        controls.Default.Left.canceled += context => OnLeft(context);
        controls.Default.Right.canceled += context => OnRight(context);
        controls.Default.Down.canceled += context => OnDown(context);

        controls.Default.R1.performed += context => OnR1(context);
        controls.Default.R1.canceled += _ => OnR1Release();

        controls.Default.R2.performed += context => OnR1(context);
        controls.Default.R2.canceled += context => OnR1(context);

        controls.Default.L1.performed += context => OnL1(context);
        controls.Default.L1.canceled += context => OnL1(context);

        controls.Default.L2.performed += _ => OnL2Press();
        controls.Default.L2.canceled += _ => OnL2Release();

        controls.Default.R3.performed += context => OnR3();

        controls.Default.Start.performed += _ => OnStart();
        controls.Default.Select.performed += _ => OnSelect();

        controls.Default.Console.performed += _ => OnTouchPad();

        controls.Default._1.performed += _ => On1();
        controls.Default._2.performed += _ => On2();
        controls.Default._3.performed += _ => On3();
        controls.Default._4.performed += _ => On4();

        directionals = new List<int>();
        controls.Default.Enable();
        user.AssociateActionsWithUser(controls);
    }

    public void SetupKeyboard()
    {
        {
            if (deviceIsAssigned) return;
            deviceIsAssigned = true;
            var user = InputUser.PerformPairingWithDevice(Keyboard.current, default(InputUser), InputUserPairingOptions.None);

            controls = new Controls();

            controls.Default.West.performed += context => OnWest(context);
            controls.Default.West.canceled += context => OnWest(context);
            controls.Default.North.performed += context => OnNorth(context);
            controls.Default.North.canceled += context => OnNorth(context);
            controls.Default.South.performed += context => OnSouth(context);
            controls.Default.South.canceled += context => OnSouth(context);
            controls.Default.East.performed += context => OnEast(context);
            controls.Default.East.canceled += context => OnEast(context);

            controls.Default.Up.performed += context => OnUp(context);
            controls.Default.Left.performed += context => OnLeft(context);
            controls.Default.Right.performed += context => OnRight(context);
            controls.Default.Down.performed += context => OnDown(context);

            controls.Default.R1.performed += context => OnR1(context);
            controls.Default.R1.canceled += context => OnR1(context);

            controls.Default.R2.performed += _ => OnR2Press();
            controls.Default.R2.canceled += _ => OnR2Release();

            controls.Default.L1.performed += context => OnL1(context);
            controls.Default.L1.canceled += context => OnL1(context);

            controls.Default.L2.performed += _ => OnL2Press();
            controls.Default.L2.canceled += _ => OnL2Release();

            controls.Default.R3.performed += context => OnR3();

            controls.Default.Start.performed += _ => OnStart();
            controls.Default.Select.performed += _ => OnSelect();

            controls.Default.Console.performed += _ => OnTouchPad();

            controls.Default._1.performed += _ => On1();
            controls.Default._2.performed += _ => On2();
            controls.Default._3.performed += _ => On3();
            controls.Default._4.performed += _ => On4();

            directionals = new List<int>();
            controls.Default.Enable();
            user.AssociateActionsWithUser(controls);
        }
    }

    void Awake()
    {
    }
    private void Update()
    {
        // if (!deviceIsAssigned) return;
        //inputDirection = controls.Default.LAnalog.ReadValue<Vector2>();
        if (!network)
        {
            inputDirection = controls.Default.DPad.ReadValue<Vector2>();
        }
    }

    public Vector2 TranslateInput(int input)
    {
        Vector3 v = Vector2.zero;
        if (input == 1) { v = new Vector2(-1, -1).normalized; }
        if (input == 2) { v = new Vector2(0, -1).normalized; }
        if (input == 3) { v = new Vector2(1, -1).normalized; }
        if (input == 4) { v = new Vector2(-1, 0).normalized; }
        if (input == 5) { v = new Vector2(0, 0).normalized; }
        if (input == 6) { v = new Vector2(1, 0).normalized; }
        if (input == 7) { v = new Vector2(-1, 1).normalized; }
        if (input == 8) { v = new Vector2(0, 1).normalized; }
        if (input == 9) { v = new Vector2(1, 1).normalized; }
        return v;
    }

    public Vector2 TranslateInput(bool[] input)
    {

        if (input[0] && input[3])
        {
            input[0] = false;
            input[3] = false;
        }

        if (input[1] && input[2])
        {
            input[1] = false;
            input[2] = false;
        }

        Vector3 v = Vector2.zero;
        if (input[0] && !input[1] && input[2] && !input[3]) { v = new Vector2(-1, -1).normalized; }
        else if (!input[0] && !input[1] && input[2] && !input[3]) { v = new Vector2(0, -1).normalized; }
        else if (!input[0] && !input[1] && input[2] && input[3]) { v = new Vector2(1, -1).normalized; }
        else if (input[0] && !input[1] && !input[2] && !input[3]) { v = new Vector2(-1, 0).normalized; }
        else if (!input[0] && !input[1] && !input[2] && !input[3]) { v = new Vector2(0, 0).normalized; }
        else if (!input[0] && !input[1] && !input[2] && input[3]) { v = new Vector2(1, 0).normalized; }
        else if (input[0] && input[1] && !input[2] && !input[3]) { v = new Vector2(-1, 1).normalized; }
        else if (!input[0] && input[1] && !input[2] && !input[3]) { v = new Vector2(0, 1).normalized; }
        else if (!input[0] && input[1] && !input[2] && input[3]) { v = new Vector2(1, 1).normalized; }
        return v;
    }

    public Vector2 InvertVector(Vector2 v1)
    {
        Vector2 v2 = new Vector2(-v1.x, -v1.y);
        return v2;
    }

    private void FixedUpdate()
    {
        //   if (!deviceIsAssigned) return;
        if (network)
        {
            // inputDirection = TranslateInput(lastInput);
            if (id == 1)
                inputDirection = TranslateInput(netDirectionals);
            else inputDirection = InvertVector(TranslateInput(netDirectionals));

            if (inputDirection.y <= 0.5F && inputDirection.y >= -0.5F)
            {
                if (inputDirection.x > 0.5F) directionals.Add(6);
                else if (inputDirection.x < -0.5F) directionals.Add(4);
                else directionals.Add(5);
            }
            else if (inputDirection.y > 0.5F)
            {
                if (inputDirection.x > 0.5F) directionals.Add(9);
                else if (inputDirection.x < -0.5F) directionals.Add(7);
                else directionals.Add(8);
            }
            else if (inputDirection.y < -0.5F)
            {
                if (inputDirection.x > 0.5F) directionals.Add(3);
                else if (inputDirection.x < -0.5F) directionals.Add(1);
                else directionals.Add(2);
            }


            if (directionals.Count <= 2) { updatedDirectionals = true; return; }
            if (directionals[directionals.Count - 1] == directionals[directionals.Count - 2]) return;

            updatedDirectionals = true;
        }
        else
        {
            if (inputDirection.y <= 0.5F && inputDirection.y >= -0.5F)
            {
                if (inputDirection.x > 0.5F) directionals.Add(6);
                else if (inputDirection.x < -0.5F) directionals.Add(4);
                else directionals.Add(5);
            }
            else if (inputDirection.y > 0.5F)
            {
                if (inputDirection.x > 0.5F) directionals.Add(9);
                else if (inputDirection.x < -0.5F) directionals.Add(7);
                else directionals.Add(8);
            }
            else if (inputDirection.y < -0.5F)
            {
                if (inputDirection.x > 0.5F) directionals.Add(3);
                else if (inputDirection.x < -0.5F) directionals.Add(1);
                else directionals.Add(2);
            }


            if (directionals.Count <= 2) { updatedDirectionals = true; return; }
            if (directionals[directionals.Count - 1] == directionals[directionals.Count - 2]) return;

            updatedDirectionals = true;
        }

    }

    private void OnLeft(InputAction.CallbackContext context)
    {
        if (debug) print("Left");
        heldDirectionals[0] = !context.canceled;

        leftInput?.Invoke();
    }

    private void OnUp(InputAction.CallbackContext context)
    {
        if (debug) print("Up");
        heldDirectionals[1] = !context.canceled;
    }
    private void OnDown(InputAction.CallbackContext context)
    {
        if (debug) print("Down");
        heldDirectionals[2] = !context.canceled;
    }
    private void OnRight(InputAction.CallbackContext context)
    {
        if (debug) print("Right");
        heldDirectionals[3] = !context.canceled;
    }
    void ChangeControlScheme(InputAction.CallbackContext context)
    {

        //if (context.control.device == Gamepad.current)
        //{
        //    if (controlScheme == ControlScheme.MouseAndKeyboard) gamepadEvent?.Invoke();
        //    if (Gamepad.current.name.Contains("Dual"))
        //    {
        //        //     if (controlScheme != ControlScheme.PS4)

        //        controlScheme = ControlScheme.PS4;
        //        controlSchemeChange?.Invoke();
        //    }

        //    else
        //    {

        //        if (controlScheme != ControlScheme.XBOX)
        //            controlSchemeChange?.Invoke();
        //        controlScheme = ControlScheme.XBOX;

        //    }
        //    //print(Gamepad.current.name);
        //    //print(Gamepad.current.device);

        //    Cursor.lockState = CursorLockMode.Locked;
        //    Cursor.visible = false;
        //}
        //else
        //{
        //    //if (controlScheme != ControlScheme.MouseAndKeyboard)

        //    controlScheme = ControlScheme.MouseAndKeyboard;
        //    controlSchemeChange?.Invoke();
        //    keyboardEvent?.Invoke();
        //    Cursor.lockState = CursorLockMode.Confined;
        //    Cursor.visible = true;
        //}
    }
    void OnTouchPad()
    {
        touchPadInput?.Invoke();
    }
    public void OnLAnalog(InputValue value)
    {
        inputDirection = value.Get<Vector2>();
    }
    public void OnRAnalog(InputValue value)
    {

    }

    public void ResolveButtons(bool[] temp)
    {
        for (int i = 0; i < temp.Length; i++)
        {
            if (temp[i] && !netButtons[i])
            {
                print("netbutton " + netButtons[i] + " & temp " + temp[i]);
                StartCoroutine("InputBuffer", i + 1);
            }
            netButtons[i] = temp[i];
        }
    }

    public void OnWest(InputAction.CallbackContext context)
    {
        if (debug) print("Square");
        ChangeControlScheme(context);
        updatedButtons = true;
        heldButtons[0] = !context.canceled;
        westInput?.Invoke();
    }
    public void OnNorth(InputAction.CallbackContext context)
    {
        if (debug) print("Triangle");
        ChangeControlScheme(context);
        updatedButtons = true;
        heldButtons[1] = !context.canceled;
        northInput?.Invoke();
    }
    public void OnSouth(InputAction.CallbackContext context)
    {
        if (debug) print("X");
        ChangeControlScheme(context);
        updatedButtons = true;
        heldButtons[2] = !context.canceled;
        southInput?.Invoke();
    }
    public void OnEast(InputAction.CallbackContext context)
    {
        if (debug) print("O");
        ChangeControlScheme(context);
        updatedButtons = true;
        heldButtons[3] = !context.canceled;
        eastInput?.Invoke();
    }
    public void OnR1(InputAction.CallbackContext context)
    {
        if (debug) print("R1");

        R1input?.Invoke();
        heldButtons[4] = !context.canceled;
        R1Hold = true; updatedButtons = true;
    }
    public void OnL1(InputAction.CallbackContext context)
    {
        if (debug) print("L1");
        heldButtons[5] = !context.canceled;
        L1input?.Invoke(); updatedButtons = true;
    }
    public void OnStart()
    {
        startInput?.Invoke();
    }
    public void OnSelect()
    {
        selectInput?.Invoke();
    }
    void OnR1Release()
    {
        R1release?.Invoke();
        StartCoroutine("InputBuffer", 5);
        R1Hold = false;
    }
    void OnSouthRelease()
    {
        southRelease?.Invoke();
        R1Hold = false;
    }
    void OnWestRelease()
    {
        westRelease?.Invoke();
    }
    void OnNorthRelease()
    {
        northRelease?.Invoke();

    }
    void OnEastRelease()
    {
        eastRelease?.Invoke();

    }
    void OnR2Press()
    {
        if (R2Hold) return;
        R2Hold = true;
        R2input?.Invoke();
    }

    void OnR2Release()
    {
        if (!R2Hold) return;
        R2Hold = false;
        R2release?.Invoke();
    }

    void OnL2Press()
    {
        if (L2Hold) return;
        L2Hold = true;
        L2input?.Invoke();
    }

    void OnL2Release()
    {
        if (!L2Hold) return;
        L2release?.Invoke();
        L2Hold = false;
    }

    void OnL1Release()
    {
        L1Hold = false;
    }

    void OnR3()
    {
        R3input?.Invoke();
    }

    void On1()
    {
        if (GameHandler.isPaused) return;
        StartCoroutine("InputBuffer", 7);
    }
    void On2()
    {
        if (GameHandler.isPaused) return;
        StartCoroutine("InputBuffer", 8);
    }
    void On3()
    {
        if (GameHandler.isPaused) return;
        StartCoroutine("InputBuffer", 9);
    }
    void On4()
    {
        if (GameHandler.isPaused) return;
        StartCoroutine("InputBuffer", 10);
    }

    IEnumerator InputBuffer(int inputID)
    {
        // if (GameManager.isPaused) yield break;
        inputQueue.Add(inputID);
        for (int i = 0; i < bufferWindow; i++)
        {
            yield return new WaitForFixedUpdate();
        }
        if (inputQueue.Count > 0)
        {
            if (inputQueue[0] == inputID)
                inputQueue.RemoveAt(0);
        }
    }
}

public enum ControlScheme { PS4, XBOX, MouseAndKeyboard }
