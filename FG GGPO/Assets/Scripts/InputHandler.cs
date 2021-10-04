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
    public ControlScheme controlScheme = ControlScheme.PS4;
    public delegate void InputEvent();

    public bool isBot;
    [FoldoutGroup("Input Buffer")] public List<int> inputQueue;
    [FoldoutGroup("Input Buffer")] public int bufferWindow = 10;
    [FoldoutGroup("Input Buffer")] public int dashInputWindow = 20;
    [FoldoutGroup("Input Buffer")] public int motionInputWindow = 40;
    [FoldoutGroup("Input Buffer")] public List<BufferedInput> bufferedInputs;
    [HideInInspector] public List<BufferedInput> deletedInputs;
    [HideInInspector] public int motionInputCounter;
    public Controls controls = null;



    public InputEvent controlSchemeChange;
    public InputEvent keyboardEvent;
    public InputEvent gamepadEvent;

    public InputEvent leftInput;
    public InputEvent upInput;
    public InputEvent rightInput;
    public InputEvent downInput;

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

    public InputEvent L3input;
    public InputEvent R3input;

    public InputEvent touchPadInput;


    public InputEvent dashInput;

    [HideInInspector] public bool R1Hold;
    [HideInInspector] public bool R2Hold;
    [HideInInspector] public bool L1Hold;
    [HideInInspector] public bool L2Hold;

    [FoldoutGroup("Input Overlay")] public Vector2 inputDirection;
    [FoldoutGroup("Input Overlay")] public bool[] netDirectionals = new bool[4];
    [FoldoutGroup("Input Overlay")] public bool[] heldDirectionals = new bool[4];
    [FoldoutGroup("Input Overlay")] public bool[] netButtons = new bool[6];
    [FoldoutGroup("Input Overlay")] public bool[] heldButtons = new bool[6];
    [FoldoutGroup("Input Overlay")] public List<int> directionals;
    [FoldoutGroup("Input Overlay")] public List<bool> buttons;
    [FoldoutGroup("Input Overlay")] public List<int> overlayDirectionals;
    [FoldoutGroup("Input Overlay")] public bool updatedDirectionals;
    [FoldoutGroup("Input Overlay")] public bool updatedButtons;

    [FoldoutGroup("Debug")] public bool dash;
    [FoldoutGroup("Debug")] public bool bf;
    [FoldoutGroup("Debug")] public bool fb;
    [FoldoutGroup("Debug")] public bool dd;
    [FoldoutGroup("Debug")] public int extraBuffer = 0;
    [FoldoutGroup("Debug")] public bool isPaused;
    [FoldoutGroup("Debug")] public bool debug;

    // private void OnEnable() => controls.Default.Enable();
    private void OnDisable()
    {
        DisableControls();
    }
    public void DisableControls()
    {
        if (deviceIsAssigned)
        {
            controls.Default.Disable();
            deviceIsAssigned = false;
        }
    }
    void Start()
    {
        buttons = new List<bool>();
        bufferedInputs = new List<BufferedInput>();
        deletedInputs = new List<BufferedInput>();

        if (GameHandler.Instance != null)
        {
            GameHandler.Instance.advanceGameState += ExecuteFrame;
        }
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
        controls.Default.R1.canceled += context => OnR1(context);

        controls.Default.R2.performed += context => OnR1(context);
        controls.Default.R2.canceled += context => OnR1(context);

        controls.Default.L1.performed += context => OnL1(context);
        controls.Default.L1.canceled += context => OnL1(context);

        controls.Default.L2.performed += _ => OnL2Press();
        controls.Default.L2.canceled += _ => OnL2Release();

        controls.Default.R3.performed += context => OnR3();
        controls.Default.L3.performed += context => OnL3();

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

    public void ResolveInputBuffer()
    {
        deletedInputs.Clear();
        if (isPaused) return;
        for (int i = 0; i < bufferedInputs.Count; i++)
        {
            bufferedInputs[i].frame--;
            if (bufferedInputs[i].frame <= 0)
            {
                deletedInputs.Add(bufferedInputs[i]);
            }
        }
        foreach (var item in deletedInputs)
        {
            if (bufferedInputs.Contains(item))
                bufferedInputs.Remove(item);
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

        if (input[0] && input[2])
        {
            input[0] = false;
            input[2] = false;
        }

        if (input[1] && input[3])
        {
            input[1] = false;
            input[3] = false;
        }

        Vector3 v = Vector2.zero;
        //Neutral
        if (!input[0] && !input[1] && !input[2] && !input[3]) { v = new Vector2(0, 0).normalized; }
        //F
        else if (input[0] && !input[1] && !input[2] && !input[3]) { v = new Vector2(0, 1).normalized; }
        //B
        else if (!input[0] && !input[1] && input[2] && !input[3]) { v = new Vector2(0, -1).normalized; }
        //R
        else if (!input[0] && input[1] && !input[2] && !input[3]) { v = new Vector2(1, 0).normalized; }
        //L
        else if (!input[0] && !input[1] && !input[2] && input[3]) { v = new Vector2(-1, 0).normalized; }
        //FR
        else if (input[0] && input[1] && !input[2] && !input[3]) { v = new Vector2(1, 1).normalized; }
        //FL
        else if (input[0] && !input[1] && !input[2] && input[3]) { v = new Vector2(-1, 1).normalized; }
        //BR
        else if (!input[0] && input[1] && input[2] && !input[3]) { v = new Vector2(1, -1).normalized; }
        //BL
        else if (!input[0] && !input[1] && input[2] && input[3]) { v = new Vector2(-1, -1).normalized; }
        return v;
    }

    public Vector2 InvertVector(Vector2 v1)
    {
        Vector2 v2 = new Vector2(-v1.x, -v1.y);
        return v2;
    }
    public Vector2 InvertX(Vector2 v1)
    {
        Vector2 v2 = new Vector2(-v1.x, v1.y);
        return v2;
    }
    private void FixedUpdate()
    {
       // ExecuteFrame();
    }

    public void ExecuteFrame()
    {
        if (!network)
        {
            if (!isBot)
            {
                for (int i = 0; i < heldDirectionals.Length; i++)
                {
                    netDirectionals[i] = heldDirectionals[i];
                }
                ResolveButtons(heldButtons);
            }
            ResolveInputBuffer();
        }

        //TRANSLATE DIRECTIONS TO INPUT INTERGERS
        inputDirection = TranslateInput(netDirectionals);
        if (inputDirection.y <= 0.5F && inputDirection.y >= -0.5F)
        {
            if (inputDirection.x > 0.5F) overlayDirectionals.Add(6);
            else if (inputDirection.x < -0.5F) overlayDirectionals.Add(4);
            else overlayDirectionals.Add(5);
        }
        else if (inputDirection.y > 0.5F)
        {
            if (inputDirection.x > 0.5F) overlayDirectionals.Add(9);
            else if (inputDirection.x < -0.5F) overlayDirectionals.Add(7);
            else overlayDirectionals.Add(8);
        }
        else if (inputDirection.y < -0.5F)
        {
            if (inputDirection.x > 0.5F) overlayDirectionals.Add(3);
            else if (inputDirection.x < -0.5F) overlayDirectionals.Add(1);
            else overlayDirectionals.Add(2);
        }

        //IF PLAYER 2, REVERSE INPUTS
        if (id == 1)
            inputDirection = TranslateInput(netDirectionals);
        else
        {
            inputDirection = InvertVector(TranslateInput(netDirectionals));
        }
        //TRANSLATE DIRECTIONS TO INPUT INTERGERS
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

        CheckMotionInputs();
        //CHECK IF INPUTS HAVE BEEN DUPLICATED
        if (directionals.Count <= 2) { updatedDirectionals = true; return; }
        if (directionals[directionals.Count - 1] == directionals[directionals.Count - 2]) return;

        updatedDirectionals = true;
    }

    public void ResolveButtons(bool[] temp)
    {
        bool foundA = false;
        bool foundB = false;
        bool foundJ = false;
        bool foundC = false;
        bool foundD = false;
        bool foundCrouch = false;


        for (int i = 0; i < temp.Length; i++)
        {
            if (temp[i] && !netButtons[i])
            {
                if (i == 0) foundA = true;
                if (i == 1) foundB = true;
                if (i == 2) foundJ = true;
                if (i == 3) foundC = true;
                if (i == 4) foundD = true;
                if (i == 5) foundCrouch = true;
                // InputBuffer(i + 1);
            }

            if (netButtons[i] != temp[i]) updatedButtons = true;
            netButtons[i] = temp[i];
        }

        if (foundB && netButtons[3] || foundC && netButtons[1])
        {
            InputBuffer(7);
        }
        else
        {
            if (foundA) InputBuffer(1);
            if (foundB) InputBuffer(2);
            if (foundJ) InputBuffer(3);
            if (foundC) InputBuffer(4);
            if (foundD) InputBuffer(5);
            if (foundCrouch) InputBuffer(6);
        }
        buttons.Add(netButtons[5]);
    }

    void CheckMotionInputs()
    {
        if (motionInputCounter > 0)
        {
            motionInputCounter--;
        }
        if (extraBuffer > 0)
        {
            motionInputCounter = motionInputWindow;
        }


        if (CheckDashInput())
            dash = true;
        else if (extraBuffer <= 0 && motionInputCounter <= 0)
            dash = false;
        if (CheckBackForward(8))
            bf = true;
        else if (extraBuffer <= 0 && motionInputCounter <= 0)
            bf = false;
        if (CheckBackForward(2))
            fb = true;
        else if (extraBuffer <= 0 && motionInputCounter <= 0)
            fb = false;
        if (CheckDownDown())
            dd = true;
        else if (extraBuffer <= 0 && motionInputCounter <= 0)
            dd = false;
    }


    public bool CheckDownDown()
    {
        bool result = false;
        bool foundNeutral = false;
        bool foundDown = false;
        bool foundNeutral2 = false;

        if (buttons.Count < 5) return false;

        if (buttons.Count <= 0) return false;

        for (int i = 1; i < motionInputWindow; i++)
        {
            if (buttons.Count <= i) return false;

            if (!buttons[buttons.Count - i] && foundDown)
            {
                foundNeutral2 = true;
            }
            if (buttons[buttons.Count - i] && foundNeutral)
            {
                foundDown = true;
            }
            if (buttons[buttons.Count - i] == false)
            {
                foundNeutral = true;
            }
            if (foundNeutral2 && buttons[buttons.Count - 1])
            {
                return true;
            }
        }

        return result;
    }
    public bool CheckBackForward(int dir)
    {
        bool result = false;
        if (directionals.Count <= 0) return false;
        int currentInput = directionals[directionals.Count - 1];
        bool foundNeutralInput = false;
        bool foundOppositeInput = false;
        bool foundNeutralInput2 = false;
        if (currentInput == 5) return false;
        if (currentInput != dir) return false;
        for (int i = 1; i < motionInputWindow; i++)
        {
            if (directionals.Count < i) return false;
            if (directionals[directionals.Count - 1] == 5)
            {
                return false;
            }
            if (directionals[directionals.Count - i] != ReverseDirection(currentInput) && foundOppositeInput)
            {
                foundNeutralInput2 = true;
            }
            if (directionals[directionals.Count - i] == ReverseDirection(currentInput) && foundNeutralInput)
            {
                foundOppositeInput = true;
            }
            if (directionals[directionals.Count - i] == 5)
            {
                foundNeutralInput = true;
            }

            if (foundNeutralInput & foundNeutralInput2 && foundOppositeInput)
            {
                return true;
            }
        }

        return result;
    }

    int ReverseDirection(int i)
    {
        switch (i)
        {
            case 2: return 8;
            case 8: return 2;
            case 4: return 6;
            case 6: return 4;
            default: return 5;
        }

    }

    public bool CheckDashInput()
    {
        bool result = false;
        int currentInput = overlayDirectionals[overlayDirectionals.Count - 1];
        bool foundNeutralInput = false;
        bool foundSameInput = false;
        bool foundNeutralInput2 = false;
        if (currentInput == 5) return false;
        for (int i = 1; i < dashInputWindow; i++)
        {
            if (overlayDirectionals.Count < i) { break; }

            if (overlayDirectionals[overlayDirectionals.Count - i] != 5 && overlayDirectionals[overlayDirectionals.Count - i] != currentInput)
            {
                return false;
            }
            if (overlayDirectionals[overlayDirectionals.Count - 2] == currentInput)
            {
                return false;
            }

            if (overlayDirectionals[overlayDirectionals.Count - i] == 5 && foundSameInput)
            {
                foundNeutralInput2 = true;
            }
            if (overlayDirectionals[overlayDirectionals.Count - i] == currentInput && foundNeutralInput)
            {
                foundSameInput = true;
            }
            if (overlayDirectionals[overlayDirectionals.Count - i] == 5)
            {
                foundNeutralInput = true;
            }

            if (foundNeutralInput & foundNeutralInput2 && foundSameInput)
            {
                if (currentInput == 2)
                {
                    dashInput?.Invoke();
                    if (id == 1)
                        InputBuffer(10);
                    else
                        InputBuffer(13);
                }
                else if (currentInput == 6)
                {
                    dashInput?.Invoke();
                    if (id == 1)
                        InputBuffer(11);
                    else InputBuffer(12);
                }
                if (currentInput == 4)
                {
                    dashInput?.Invoke();
                    if (id == 1)
                        InputBuffer(12);
                    else InputBuffer(11);
                }
                if (currentInput == 8)
                {
                    dashInput?.Invoke();
                    if (id == 1)
                        InputBuffer(13);
                    else
                        InputBuffer(10);
                }

                return true;
            }
        }

        return result;
    }

    private void OnLeft(InputAction.CallbackContext context)
    {
        if (debug) print("Left");
        
        heldDirectionals[3] = !context.canceled;
        if(context.performed)
        leftInput?.Invoke();
    }

    private void OnUp(InputAction.CallbackContext context)
    {
        if (debug) print("Up");
        heldDirectionals[0] = !context.canceled;
        if (context.performed)
            upInput?.Invoke();
    }
    private void OnDown(InputAction.CallbackContext context)
    {
        if (debug) print("Down");
        heldDirectionals[2] = !context.canceled;
        if (context.performed)
            downInput?.Invoke();
    }
    private void OnRight(InputAction.CallbackContext context)
    {
        if (debug) print("Right");
        heldDirectionals[1] = !context.canceled;
        if (context.performed)
            rightInput?.Invoke();
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

    public void OnWest(InputAction.CallbackContext context)
    {
        if (debug) print("Square");
        ChangeControlScheme(context);
        //updatedButtons = true;
        heldButtons[0] = !context.canceled;
        westInput?.Invoke();
    }
    public void OnNorth(InputAction.CallbackContext context)
    {
        if (debug) print("Triangle");
        ChangeControlScheme(context);
        // updatedButtons = true;
        heldButtons[1] = !context.canceled;
        northInput?.Invoke();
    }
    public void OnSouth(InputAction.CallbackContext context)
    {
        if (debug) print("X");
        ChangeControlScheme(context);
        //   updatedButtons = true;
        heldButtons[2] = !context.canceled;
        southInput?.Invoke();
    }
    public void OnEast(InputAction.CallbackContext context)
    {
        if (debug) print("O");
        ChangeControlScheme(context);
        //  updatedButtons = true;
        heldButtons[3] = !context.canceled;
        eastInput?.Invoke();
    }
    public void OnR1(InputAction.CallbackContext context)
    {
        if (debug) print("R1");

        R1input?.Invoke();
        heldButtons[4] = !context.canceled;
        R1Hold = true;
        //updatedButtons = true;
    }
    public void OnL1(InputAction.CallbackContext context)
    {
        if (debug) print("L1");
        heldButtons[5] = !context.canceled;
        L1input?.Invoke();
        //updatedButtons = true;
    }
    public void OnStart()
    {
        startInput?.Invoke();
        if (StageManager.Instance != null)
        {
            StageManager.Instance.RestartScene();
        }
    }
    public void OnSelect()
    {
        selectInput?.Invoke();
        if (GameHandler.Instance != null)
        {
            GameHandler.Instance.ResetRound();
        }
    }
    void OnR1Release()
    {
        R1release?.Invoke();

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
    void OnL3()
    {
        L3input?.Invoke();
        print("L3");
    }

    void OnR3()
    {
        R3input?.Invoke();
        print("R3");
    }

    void On1()
    {
        if (GameHandler.isPaused) return;
        InputBuffer(7);
    }
    void On2()
    {
        if (GameHandler.isPaused) return;
        InputBuffer(8);
    }
    void On3()
    {
        if (GameHandler.isPaused) return;
        InputBuffer(9);
    }
    void On4()
    {
        if (GameHandler.isPaused) return;
        InputBuffer(10);
    }

    int Direction()
    {

        if (heldDirectionals[0])
        {
            if (id == 1)
                return 8;
            else return 2;
        }
        else if (heldDirectionals[2])
        {
            if (id == 1)
                return 2;
            else return 8;
        }
        else if (heldDirectionals[1])
        {
            if (id == 1)
                return 6;
            else return 4;
        }
        else if (heldDirectionals[3])
        {
            if (id == 1)
                return 4;
            else return 6;
        }
        else
            return 5;
    }

    public void InputBuffer(int inputID)
    {
        bufferedInputs.Add(new BufferedInput(inputID, Direction(), netButtons[5], bufferWindow + extraBuffer));
    }
}
[System.Serializable]
public class BufferedInput
{
    public BufferedInput(int input, int direction, bool c, int bufferWindow)
    {
        id = input;
        frame = bufferWindow;
        crouch = c;
        dir = direction;

    }
    public int id;
    public int dir;
    public bool crouch;
    public int frame;
}

public enum ControlScheme { PS4, XBOX, MouseAndKeyboard }
