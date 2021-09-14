﻿
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
        mov = GetComponent<Movement>();
        input.dashInput += BackDash;
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

    void Update()
    {
        if (GameHandler.isPaused) return;
        relativeDirection = RelativeToCamera(input.inputDirection);
    }

    void UpdateDirection()
    {
        input.directionOffset = 0;
        if (!InputManager.Instance.updateDirections) return;
        //Vector3 dist = (GameHandler.Instance.p2Transform.position - GameHandler.Instance.p1Transform.position);
        //dist = dist / 2;
        //Vector3 center = GameHandler.Instance.p1Transform.position + dist;


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
        if (GameHandler.isPaused)
        {
            mov.direction = Vector3.zero;
            return;
        }

        input.isPaused = status.hitstopCounter > 0;

        UpdateDirection();

        if (status.currentState == Status.State.Neutral)
        {
            if (mov.ground && !input.isDummy)
            {
                mov.crouching = input.netButtons[5];

                if (mov.crouching) status.SetBlockState(BlockState.Crouching);
                else if (mov.holdBack) status.SetBlockState(BlockState.Standing);
                else status.SetBlockState(BlockState.None);
            }
            else status.SetBlockState(BlockState.Airborne);

            NeutralInput();
        }
        else if (status.currentState == Status.State.Active || status.currentState == Status.State.Recovery)
        {
            InAnimationInput();
        }
        if (Physics.autoSimulation)
            mov.direction = relativeDirection;
    }
    public void RollbackTick()
    {
        mov.isMoving = true;
        mov.direction = RelativeToCamera(rollbackInput[0]);
        //print(mov.direction);
        //if (rollbackInput.Count > 0)
        //    rollbackInput.RemoveAt(0);
    }

    void BackDash()
    {
        //.inputQueue.Add()

    }

    void NeutralInput()
    {
    
        if (input.dash) mov.sprinting = true;
        if (input.directionals.Count > 0)
            if (input.directionals[input.directionals.Count - 1] < 7 && mov.ground) mov.sprinting = false;

        ProcessBuffer();
    }

    public virtual void ProcessBuffer()
    {
        int bufferID = -1;
        for (int i = 0; i < input.bufferedInputs.Count; i++)
        {
            //Jump Button
            if (input.bufferedInputs[i].id == 3)
            {
                mov.Jump();
                bufferID = i;
                break;
            }
            if (input.bufferedInputs[i].id == 10) {
                attack.AttackProperties(attack.moveset.backDash);
                bufferID = i;
                break;
            }
            if (input.bufferedInputs[i].id == 11)
            {
                attack.AttackProperties(attack.moveset.rightDash);
                bufferID = i;
                break;
            }
            if (input.bufferedInputs[i].id == 12)
            {
                attack.AttackProperties(attack.moveset.leftDash);
                bufferID = i;
                break;
            }

            //A Button
            if (input.bufferedInputs[i].id == 1)
            {  //Ground
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
                    if (attack.Attack(attack.moveset.A5))
                    {
                        bufferID = i;
                        break;
                    }
                }
                //Airborne
                else
                {
                    if (mov.crouching)
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
            {  //Ground
                if (mov.ground)
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
                    if (attack.Attack(attack.moveset.B5))
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
                if (mov.ground)
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
                    if (attack.Attack(attack.moveset.C5))
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
                if (mov.ground)
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
                    if (attack.Attack(attack.moveset.D5))
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
        //mov.sprinting = false;
        attack.block = false;
        status.blocking = false;

        if (InputAvailable() && attack.canGatling)
        {

            if (attack.attackString)
            {
                ProcessBuffer();
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
                        return attack.moveset.A5;
                }
                else return attack.moveset.jA;
            case 2:
                if (mov.ground)
                {
                    if (input.netButtons[5])
                        return attack.moveset.cB;
                    else
                        return attack.moveset.B5;
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
                        return attack.moveset.C5;
                }
                else return attack.moveset.jC;

            case 5:
                if (mov.ground)
                {
                    if (input.netButtons[5])
                        return attack.moveset.cD;
                    else
                        return attack.moveset.D5;
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
