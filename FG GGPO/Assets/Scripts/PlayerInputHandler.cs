
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
        status = GetComponent<Status>();
    }

    private void Start()
    {
        GameHandler.Instance.rollbackTick += RollbackTick;
        mov = GetComponent<Movement>();
        input.dashInput += BackDash;
    }

    Vector3 RelativeToCamera(Vector2 v)
    {
        forwardVector = (mov.strafeTarget.position - transform.position).normalized;
        rightVector = Vector3.Cross(Vector3.up, forwardVector).normalized;
        Vector3 temp = ((rightVector * v.x) + (forwardVector * v.y));
        return temp;
    }

    void Update()
    {
        if (GameHandler.isPaused) return;
        relativeDirection = RelativeToCamera(input.inputDirection);
    }

    private void FixedUpdate()
    {
        if (GameHandler.isPaused)
        {
            mov.direction = Vector3.zero;
            return;
        }



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
            print("Bob");
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

        if (input.directionals[input.directionals.Count - 1] < 7 && mov.ground) mov.sprinting = false;

        if (InputAvailable())
        {
            switch (input.inputQueue[0])
            {
                case 1:
                    if (mov.ground)
                    {
                        if (input.netButtons[5])
                            attack.Attack(attack.moveset.cA);
                        else
                            attack.Attack(attack.moveset.A5);
                    }
                    else attack.Attack(attack.moveset.jA);
                    Delete();
                    break;
                case 2:
                    if (mov.ground)
                    {
                        if (input.netButtons[5])
                            attack.Attack(attack.moveset.cB);
                        else
                            attack.Attack(attack.moveset.B5);
                    }
                    else attack.Attack(attack.moveset.jB);
                    Delete();
                    break;
                case 3:

                    mov.Jump();
                    Delete();

                    break;
                case 4:
                    if (mov.ground)
                    {
                        if (input.netButtons[5])
                            attack.Attack(attack.moveset.cC);
                        else
                            attack.Attack(attack.moveset.C5);
                    }
                    else attack.Attack(attack.moveset.jC);
                    Delete();
                    break;
                case 5:
                    if (mov.ground)
                    {
                        if (input.netButtons[5])
                            attack.Attack(attack.moveset.cD);
                        else
                            attack.Attack(attack.moveset.D5);
                    }
                    else attack.Attack(attack.moveset.jD);
                    Delete();
                    break;

                case 6:
                    Delete();
                    break;
                case 7:
                    Delete();
                    break;
                case 8:
                    Delete();
                    break;
                case 9:
                    Delete();
                    break;
                case 10:
                    attack.AttackProperties(attack.moveset.backDash);
                    Delete();
                    break;
                case 11:
                    attack.AttackProperties(attack.moveset.rightDash);
                    Delete();
                    break;
                case 12:
                    attack.AttackProperties(attack.moveset.leftDash);
                    Delete();
                    break;
                default: break;
            }
        }
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
                if (attack.CanUseMove(ReturnMove(input.inputQueue[0])))
                {
                    NeutralInput();
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

    public void Delete()
    {
        input.inputQueue.RemoveAt(0);
    }

    public Vector3 AngleToVector(float angleInDegrees)
    {
        if (relativeDirection.sqrMagnitude > 0.01F)
            angleInDegrees += (Quaternion.LookRotation(relativeDirection, Vector3.up).eulerAngles).y;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
