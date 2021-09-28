
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class PlayerInputHandler : MonoBehaviour
{
    [FoldoutGroup("Components")] public InputHandler input;
    [FoldoutGroup("Components")] public Camera cam;

    [FoldoutGroup("Components")] public Status status;
    [FoldoutGroup("Components")] public Movement mov;
    [FoldoutGroup("Components")] public AttackScript attack;

    [FoldoutGroup("Auto Aim")]

    private Vector3 forwardVector;
    private Vector3 rightVector;
    [HideInInspector] public Vector3 relativeDirection;
    public bool frontTurned;
    Ray ray;
    RaycastHit hit;
    public bool rollbacking;
    public List<Vector2> rollbackInput;

    private void Awake()
    {


    }

    private void Start()
    {
        status = GetComponent<Status>();
        GameHandler.Instance.rollbackTick += RollbackTick;
        GameHandler.Instance.advanceGameState += ExecuteFrame;
        mov = GetComponent<Movement>();
    }

    Vector3 RelativeToCamera(Vector2 v)
    {
        if (InputManager.Instance.absoluteDirections)
        {
            forwardVector = (CameraManager.Instance.mainCamera.transform.forward);
            rightVector = Vector3.Cross(Vector3.up, forwardVector).normalized;
            Vector3 ab = ((rightVector * v.x) + (forwardVector * v.y));
            return ab;
        }
        forwardVector = (mov.strafeTarget.position - transform.position);
        forwardVector.y = 0;
        forwardVector = forwardVector.normalized;
        rightVector = Vector3.Cross(Vector3.up, forwardVector).normalized;
        Vector3 temp = ((rightVector * v.x) + (forwardVector * v.y));
        return temp;
    }

    void UpdateDirection()
    {
        input.directionOffset = 0;
        if (!InputManager.Instance.updateDirections) return;

        float angle = Vector3.SignedAngle(CameraManager.Instance.mainCamera.transform.forward, (GameHandler.Instance.ReturnPlayer(transform).position - transform.position).normalized, Vector3.up);
        print(angle);
        if (angle > 135 || angle < -135)
        {
            input.directionOffset = 2;
        }
        else if (angle > 45)
        {
            input.directionOffset = 1;
        }
        else if (angle < -45)
        {
            input.directionOffset = 3;
        }
        else
        {
            input.directionOffset = 0;
        }
    }

    private void FixedUpdate()
    {
        //ExecuteFrame();
    }

    public void ExecuteFrame()
    {
        if (GameHandler.isPaused || GameHandler.cutscene)
        {
            mov.direction = Vector3.zero;
            return;
        }
        relativeDirection = RelativeToCamera(input.inputDirection);
        float angle = Vector3.SignedAngle(transform.forward, (transform.position - GameHandler.Instance.ReturnPlayer(transform).position).normalized, Vector3.up);
        frontTurned = Mathf.Abs(angle) > 90;

        //UpdateDirection();
        if (status.currentState == Status.State.Neutral || status.currentState == Status.State.Blockstun)
        {
            if (mov.ground && !input.isDummy)
            {
                mov.crouching = input.netButtons[5];

                if (mov.crouching) status.SetBlockState(BlockState.Crouching);
                else if (mov.holdBack) status.SetBlockState(BlockState.Standing);
                else status.SetBlockState(BlockState.None);
            }
            else status.SetBlockState(BlockState.Airborne);
        }

        if (status.currentState == Status.State.Neutral)
        {
            NeutralInput();
        }
        else if (status.currentState == Status.State.Active || status.currentState == Status.State.Recovery)
        {
            InAnimationInput();
        }
        else if (status.currentState == Status.State.LockedAnimation)
        {
            LockedAnimationInput();
        }

        mov.direction = relativeDirection;
        input.isPaused = status.hitstopCounter > 0 || attack.jumpFrameCounter > 0;
        input.extraBuffer = status.hitstopCounter;
    }

    public void RollbackTick()
    {
        mov.isMoving = true;
        mov.direction = RelativeToCamera(rollbackInput[0]).normalized;
        //print(mov.direction);
        //if (rollbackInput.Count > 0)
        //    rollbackInput.RemoveAt(0);
    }

    void NeutralInput()
    {

        if (input.dash) mov.sprinting = true;
        if (input.directionals.Count > 0)
            //if (input.directionals[input.directionals.Count - 1] == 2 && mov.ground || input.directionals[input.directionals.Count - 1] == 5 && mov.ground) mov.sprinting = false;
            if (input.directionals[input.directionals.Count - 1] < 4 && mov.ground || input.directionals[input.directionals.Count - 1] == 5 && mov.ground) mov.sprinting = false;

        ProcessBuffer();
    }

    public void SpecialInputs()
    {
        foreach (var item in attack.moveset.specials)
        {
            if (item.motionInput == SpecialInput.BackForward)
            {

            }
        }
    }

    public virtual void ProcessBuffer()
    {
        int bufferID = -1;
        bool doSpecial = false;
        for (int i = 0; i < input.bufferedInputs.Count; i++)
        {
            //Jump Button
            if (input.bufferedInputs[i].id == 3)
            {
                bool canJump = status.currentState == Status.State.Neutral || attack.attackString && attack.jumpCancel;
                if (canJump)
                {
                    //print(GameHandler.Instance.gameFrameCount + " Jump input");
                    mov.JumpStartup();
                    bufferID = i;

                    break;
                }
                continue;
            }

            foreach (var item in attack.moveset.specials)
            {
                bool ground = (status.groundState == GroundState.Grounded);
                if (item.grounded == ground)
                    if (item.motionInput == SpecialInput.BackForward)
                    {
                        //
                        if (input.bf)
                        {
                            if (input.bufferedInputs[i].id - 1 == (int)item.buttonInput)
                            {
                                attack.Attack(item.move);
                                doSpecial = true;
                                bufferID = i;
                                break;
                            }
                        }

                    }
                    else if (item.motionInput == SpecialInput.DownDown)
                    {
                        if (input.CheckDownDown())
                        {
                            if (input.bufferedInputs[i].id - 1 == (int)item.buttonInput)
                            {
                                attack.Attack(item.move);
                                doSpecial = true;
                                bufferID = i;
                                break;
                            }
                        }
                    }

            }
            if (doSpecial) break;


            if (input.bufferedInputs[i].id == 10)
            {
                if (status.groundState == GroundState.Grounded) { attack.Attack(attack.moveset.backDash); }
                else
                {
                    if (frontTurned)
                        attack.Attack(attack.moveset.airdashB);
                    else attack.Attack(attack.moveset.airdashF);
                }
                bufferID = i;
                break;
            }
            if (input.bufferedInputs[i].id == 11)
            {
                if (status.groundState == GroundState.Grounded)
                {
                    attack.Attack(attack.moveset.rightDash);
                    bufferID = i;
                    break;
                }
            }
            if (input.bufferedInputs[i].id == 12)
            {
                if (status.groundState == GroundState.Grounded)
                {
                    attack.Attack(attack.moveset.leftDash);
                    bufferID = i;
                    break;
                }
            }
            if (input.bufferedInputs[i].id == 13)
            {
                if (status.groundState != GroundState.Grounded)
                {
                    if (frontTurned)
                        attack.Attack(attack.moveset.airdashF);
                    else
                        attack.Attack(attack.moveset.airdashB);
                    bufferID = i;
                    break;
                }
            }
            if (input.bufferedInputs[i].id == 7)
            {
                if (status.groundState == GroundState.Grounded)
                {
                    attack.Attack(attack.moveset.grabF);
                }
            }
            //A Button
            if (input.bufferedInputs[i].id == 1)
            {  //Ground
               //  print(GameHandler.Instance.gameFrameCount + " A start " + status.currentState + " " + status.groundState);
                if (mov.ground)
                {
                    if (input.bufferedInputs[i].crouch)
                    {
                        //Will execute attack if returns true
                        if (attack.Attack(attack.moveset.cA))
                        {
                            bufferID = i;
                            break;
                        }
                    }
                    if (input.bufferedInputs[i].dir == 6)
                    {
                        if (attack.Attack(attack.moveset.RA))
                        {
                            bufferID = i;
                            break;
                        }
                    }
                    else if (input.bufferedInputs[i].dir == 4)
                    {
                        if (attack.Attack(attack.moveset.LA))
                        {
                            bufferID = i;
                            break;
                        }
                    }
                    if (input.bufferedInputs[i].dir == 8)
                    {
                        if (attack.Attack(attack.moveset.fA))
                        {
                            bufferID = i;
                            break;
                        }
                    }
                    if (input.bufferedInputs[i].dir == 2)
                    {
                        if (attack.Attack(attack.moveset.bA))
                        {
                            bufferID = i;
                            break;
                        }
                    }
                    if (attack.Attack(attack.moveset.sA))
                    {
                        bufferID = i;
                        break;
                    }
                }
                //Airborne
                else
                {
                    if (input.bufferedInputs[i].crouch)
                    {
                        if (attack.Attack(attack.moveset.jcA))
                        {
                            bufferID = i;
                            break;
                        }
                    }
                    if (attack.Attack(attack.moveset.jA))
                    {
                        bufferID = i;
                        break;
                    }
                }
            }
            //B Button
            else if (input.bufferedInputs[i].id == 2)

            {
                // print(GameHandler.Instance.gameFrameCount + " B start " + status.currentState + " " + status.groundState);
                //Ground
                if (status.groundState == GroundState.Grounded)
                {
                    if (input.bufferedInputs[i].crouch)
                    {
                        //Will execute attack if returns true
                        if (attack.Attack(attack.moveset.cB))
                        {
                            bufferID = i;
                            break;
                        }
                    }
                    if (input.bufferedInputs[i].dir == 6)
                    {
                        if (attack.Attack(attack.moveset.RB))
                        {
                            bufferID = i;
                            break;
                        }
                    }
                    else if (input.bufferedInputs[i].dir == 4)
                    {
                        if (attack.Attack(attack.moveset.LB))
                        {
                            bufferID = i;
                            break;
                        }
                    }
                    if (input.bufferedInputs[i].dir == 8)
                    {
                        if (attack.Attack(attack.moveset.fB))
                        {
                            bufferID = i;
                            break;
                        }
                    }
                    if (input.bufferedInputs[i].dir == 2)
                    {
                        if (attack.Attack(attack.moveset.bB))
                        {
                            bufferID = i;
                            break;
                        }
                    }
                    if (attack.Attack(attack.moveset.sB))
                    {
                        bufferID = i;
                        break;
                    }
                }
                //Airborne
                else
                {
                    if (input.bufferedInputs[i].crouch)
                    {
                        if (attack.Attack(attack.moveset.jcB))
                        {
                            bufferID = i;
                            break;
                        }
                    }
                    if (attack.Attack(attack.moveset.jB))
                    {
                        bufferID = i;
                        break;
                    }
                }
            }
            //C Button
            else if (input.bufferedInputs[i].id == 4)
            {  //Ground
                if (status.groundState == GroundState.Grounded)
                {
                    if (input.bufferedInputs[i].crouch)
                    {
                        //Will execute attack if returns true
                        if (attack.Attack(attack.moveset.cC))
                        {
                            bufferID = i;
                            break;
                        }
                    }
                    if (input.bufferedInputs[i].dir == 6)
                    {
                        if (attack.Attack(attack.moveset.RC))
                        {
                            bufferID = i;
                            break;
                        }
                    }
                    else if (input.bufferedInputs[i].dir == 4)
                    {
                        if (attack.Attack(attack.moveset.LC))
                        {
                            bufferID = i;
                            break;
                        }
                    }
                    if (input.bufferedInputs[i].dir == 8)
                    {
                        if (attack.Attack(attack.moveset.fC))
                        {
                            bufferID = i;
                            break;
                        }
                    }
                    if (input.bufferedInputs[i].dir == 2)
                    {
                        if (attack.Attack(attack.moveset.bC))
                        {
                            bufferID = i;
                            break;
                        }
                    }
                    if (attack.Attack(attack.moveset.sC))
                    {
                        bufferID = i;
                        break;
                    }
                }
                //Airborne
                else
                {
                    if (input.bufferedInputs[i].crouch)
                    {
                        if (attack.Attack(attack.moveset.jcC))
                        {
                            bufferID = i;
                            break;
                        }
                    }
                    if (attack.Attack(attack.moveset.jC))
                    {
                        bufferID = i;
                        break;
                    }
                }
            }
            //D Button
            else if (input.bufferedInputs[i].id == 5)
            {  //Ground
                if (status.groundState == GroundState.Grounded)
                {
                    if (input.bufferedInputs[i].crouch)
                    {
                        //Will execute attack if returns true
                        if (attack.Attack(attack.moveset.cD))
                        {
                            bufferID = i;
                            break;
                        }
                    }
                    if (input.bufferedInputs[i].dir == 6)
                    {
                        if (attack.Attack(attack.moveset.RD))
                        {
                            bufferID = i;
                            break;
                        }
                    }
                    else if (input.bufferedInputs[i].dir == 4)
                    {
                        if (attack.Attack(attack.moveset.LD))
                        {
                            bufferID = i;
                            break;
                        }
                    }
                    if (input.bufferedInputs[i].dir == 8)
                    {
                        if (attack.Attack(attack.moveset.fD))
                        {
                            bufferID = i;
                            break;
                        }
                    }
                    if (input.bufferedInputs[i].dir == 2)
                    {
                        if (attack.Attack(attack.moveset.bD))
                        {
                            bufferID = i;
                            break;
                        }
                    }
                    if (attack.Attack(attack.moveset.sD))
                    {
                        print("5d");
                        bufferID = i;
                        break;
                    }
                }
                //Airborne
                else
                {
                    if (input.bufferedInputs[i].crouch)
                    {
                        if (attack.Attack(attack.moveset.jcD))
                        {
                            bufferID = i;
                            break;
                        }
                    }
                    if (attack.Attack(attack.moveset.jD))
                    {
                        bufferID = i;
                        break;
                    }
                }
            }
        }

        //Delete all inputs in buffer before buffer ID
        DeleteInputs(bufferID);
    }

    void InAnimationInput()
    {
        status.blocking = false;

        if (attack.attackString)
        {
            ProcessBuffer();
        }

    }

    void LockedAnimationInput()
    {
        int bufferID = -1;
        for (int i = 0; i < input.bufferedInputs.Count; i++)
        {

            if (status.currentState == Status.State.LockedAnimation)
            {
                if (input.bufferedInputs[i].id == 7)
                {
                    status.ThrowBreak();
                    GameHandler.Instance.ReturnPlayer(transform).GetComponent<Status>().ThrowBreak();
                    bufferID = i;
                    DeleteInputs(bufferID);
                }
            }
        }
    }

    public Move ReturnMove(int inputID)
    {
        switch (inputID)
        {
            case 1:
                if (mov.ground)
                {
                    if (input.netButtons[5])
                        return attack.moveset.cA;
                    else
                        return attack.moveset.sA;
                }
                else return attack.moveset.jA;
            case 2:
                if (mov.ground)
                {
                    if (input.netButtons[5])
                        return attack.moveset.cB;
                    else
                        return attack.moveset.sB;
                }
                else return attack.moveset.jB;
            case 3:
                //mov.Jump();
                return null;
            case 4:
                if (mov.ground)
                {
                    if (input.netButtons[5])
                        return attack.moveset.cC;
                    else
                        return attack.moveset.sC;
                }
                else return attack.moveset.jC;

            case 5:
                if (mov.ground)
                {
                    if (input.netButtons[5])
                        return attack.moveset.cD;
                    else
                        return attack.moveset.sD;
                }
                else return attack.moveset.jD;
            case 6:
                return null;
            case 7:
                return null;
            case 8:
                return null;
            case 9:
                return null;
            case 10:
                return attack.moveset.backDash;
            case 11:
                return attack.moveset.rightDash;
            case 12:
                return attack.moveset.leftDash;
            default: return null;
        }
    }

    bool InputAvailable()
    {
        return input.inputQueue.Count > 0;
    }

    public void DeleteInputs(int bufferIndex)
    {
        for (int i = 0; i < bufferIndex + 1; i++)
        {
            input.bufferedInputs.RemoveAt(0);
        }
    }

    public void Delete()
    {
        input.bufferedInputs.Clear();

        input.inputQueue.RemoveAt(0);
    }

    public Vector3 AngleToVector(float angleInDegrees)
    {
        if (relativeDirection.sqrMagnitude > 0.01F)
            angleInDegrees += (Quaternion.LookRotation(relativeDirection, Vector3.up).eulerAngles).y;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
