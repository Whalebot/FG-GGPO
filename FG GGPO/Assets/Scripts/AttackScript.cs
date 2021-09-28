using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class AttackScript : MonoBehaviour
{
    private Status status;
    [FoldoutGroup("Components")] public HitboxContainer containerScript;
    [FoldoutGroup("Components")] public Transform hitboxContainer;
    [FoldoutGroup("Components")] public List<GameObject> hitboxes;

    Movement movement;
    CharacterSFX sfx;

    public delegate void AttackEvent();
    public AttackEvent startupEvent;
    public AttackEvent activeEvent;
    public AttackEvent recoveryEvent;
    public AttackEvent parryEvent;
    public AttackEvent blockEvent;
    public Moveset mainMoveset;
    public Moveset moveset;
    [HeaderAttribute("Attack attributes")]
    [FoldoutGroup("Debug")] public Move activeMove;
    [FoldoutGroup("Debug")] public bool gatling;
    [FoldoutGroup("Debug")] public int attackID;
    [FoldoutGroup("Debug")] public int attackFrames;

    [FoldoutGroup("Debug")] public int movementFrames;


    [FoldoutGroup("Debug")] public Move movementOption;
    [FoldoutGroup("Jump Startup")] public int jumpMinusFrames;
    [FoldoutGroup("Jump Startup")] public int jumpFrameCounter;
    [FoldoutGroup("Move properties")] public bool attacking;
    [FoldoutGroup("Move properties")] public bool attackString;
    [FoldoutGroup("Move properties")] public bool landCancel;
    [FoldoutGroup("Move properties")] public bool jumpCancel;
    [FoldoutGroup("Move properties")] public bool specialCancel;
    bool newAttack;
    [HideInInspector] public int combo;
    List<Move> usedMoves;

    // Start is called before the first frame update
    void Start()
    {
        containerScript = GetComponentInChildren<HitboxContainer>();
        status = GetComponent<Status>();
        movement = GetComponent<Movement>();
        sfx = GetComponentInChildren<CharacterSFX>();
        movement.jumpStartEvent += JumpCancel;
        movement.landEvent += Land;
        status.neutralEvent += ResetCombo;
        status.hurtEvent += HitstunEvent;
        status.deathEvent += HitstunEvent;
        status.throwBreakEvent += ThrowBreak;


        if (mainMoveset != null) moveset = mainMoveset;
        GameHandler.Instance.advanceGameState += ExecuteFrame;
    }

    private void Awake()
    {
        usedMoves = new List<Move>();
    }

    private void FixedUpdate()
    {
        //ExecuteFrame();

    }

    public void ExecuteFrame()
    {
        if (status.hitstopCounter <= 0)
        {
            if (jumpFrameCounter > 0)
            {
                jumpFrameCounter--;
                if (jumpFrameCounter <= 0)
                    status.GoToState(Status.State.Neutral);
            }

            if (movementOption != null)
            {
                for (int i = 0; i < movementOption.m.Length; i++)
                {
                    if (GameHandler.Instance.gameFrameCount > movementFrames + movementOption.m[i].startFrame + movementOption.m[i].duration)
                    {
                        if (movementOption.m[i].resetVelocityDuringRecovery)
                        {
                            status.rb.velocity = Vector3.zero;
                        }
                        if (i == movementOption.m.Length - 1)
                        {
                            movementOption = null;
                            break;
                        }
                    }
                    else if (GameHandler.Instance.gameFrameCount > movementFrames + movementOption.m[i].startFrame)
                    {
                        // movement.storedDirection = movement.rb.velocity;
                        if (movementOption.m[i].resetVelocityDuringRecovery)
                            status.rb.velocity = Vector3.zero;
                        if (movementOption.overrideVelocity)
                        {
                            if (movementOption.m[i].homing)
                                status.rb.velocity = movement.CalculateRight(movementOption.m[i].momentum.x) + transform.up * movementOption.m[i].momentum.y + transform.forward * movementOption.m[i].momentum.z;
                            else status.rb.velocity = movementOption.m[i].momentum.x * transform.right + transform.up * movementOption.m[i].momentum.y + transform.forward * movementOption.m[i].momentum.z;
                            if (movementOption.m[i].momentum.y > 1) status.GoToGroundState(GroundState.Airborne);
                        }
                    }
                }
            }

            if (attacking)
            {
                attackFrames++;
                if (attackFrames >= activeMove.firstStartupFrame + activeMove.attacks[0].gatlingFrames)
                {

                    attackString = true;
                    newAttack = false;
                }

                //Execute properties
                ProcessInvul();


                for (int i = 0; i < activeMove.sfx.Length; i++)
                {
                    if (attackFrames == activeMove.sfx[i].startup)
                        Instantiate(activeMove.sfx[i].prefab);
                }
                //Execute momentum
                for (int i = 0; i < activeMove.m.Length; i++)
                {

                    if (attackFrames > activeMove.m[i].startFrame + activeMove.m[i].duration)
                    {
                        if (activeMove.m[i].resetVelocityDuringRecovery)
                        {
                            status.rb.velocity = Vector3.zero;
                            //movement.storedDirection = Vector3.zero;

                        }
                    }
                    else if (attackFrames > activeMove.m[i].startFrame)
                    {
                        movement.storedDirection = movement.rb.velocity;
                        if (activeMove.m[i].resetVelocityDuringRecovery)
                            status.rb.velocity = Vector3.zero;
                        if (activeMove.overrideVelocity)
                        {
                            if (activeMove.m[i].homing)
                                status.rb.velocity = movement.CalculateRight(activeMove.m[i].momentum.x) + transform.up * activeMove.m[i].momentum.y + transform.forward * activeMove.m[i].momentum.z;
                            else status.rb.velocity = activeMove.m[i].momentum.x * transform.right + transform.up * activeMove.m[i].momentum.y + transform.forward * activeMove.m[i].momentum.z;

                            if (activeMove.m[i].momentum.y > 1) status.GoToGroundState(GroundState.Airborne);
                        }
                    }
                }


                int firstStartupFrame = activeMove.attacks[0].startupFrame;
                int lastActiveFrame = activeMove.attacks[activeMove.attacks.Length - 1].startupFrame + activeMove.attacks[activeMove.attacks.Length - 1].activeFrames - 1;
                int totalMoveDuration = lastActiveFrame + activeMove.recoveryFrames;

                if (attackFrames > totalMoveDuration)
                {
                    Idle();
                }
                else if (attackFrames < firstStartupFrame)
                {
                    //print(attack.attackFrames + " Startup");
                    StartupFrames();
                }
                else if (attackFrames <= lastActiveFrame)
                {
                    ActiveFrames();
                }

                else if (attackFrames <= totalMoveDuration)
                {
                    RecoveryFrames();
                }
            }

            if (status.currentState == Status.State.Neutral || status.currentState == Status.State.Blockstun || status.currentState == Status.State.Hitstun) usedMoves.Clear();
        }
    }


    public void StartupFrames()
    {
        status.GoToState(Status.State.Startup);
        status.counterhitState = true;
        //ClearHitboxes();
    }
    void ClearHitboxes()
    {
        for (int i = 0; i < hitboxes.Count; i++)
        {
            if (hitboxes[i] != null)
            {
                Destroy(hitboxes[i]);
            }
        }
        //  print("Auto destroy hitbox");
        hitboxes.Clear();
    }

    public void ActiveFrames()
    {
        for (int i = 0; i < activeMove.attacks.Length; i++)
        {
            if (attackFrames < activeMove.attacks[i].startupFrame + activeMove.attacks[i].activeFrames && attackFrames >= activeMove.attacks[i].startupFrame)
            {

                status.GoToState(Status.State.Active);
                if (hitboxes.Count < i + 1)
                {
                    if (activeMove.attacks[i].hitbox != null)
                    {
                        if (activeMove.attacks[i].attackType == AttackType.Projectile)
                        {
                            hitboxes.Add(Instantiate(activeMove.attacks[i].hitbox, hitboxContainer.position, transform.rotation, hitboxContainer));
                            hitboxes[i].transform.localPosition = activeMove.attacks[i].hitbox.transform.localPosition;
                            hitboxes[i].transform.SetParent(null);
                        }
                        else
                        {
                            hitboxes.Add(Instantiate(activeMove.attacks[i].hitbox, hitboxContainer.position, transform.rotation, hitboxContainer));
                            hitboxes[i].transform.localPosition = activeMove.attacks[i].hitbox.transform.localPosition;
                        }
                        Hitbox hitbox = hitboxes[i].GetComponentInChildren<Hitbox>();
                        hitbox.hitboxID = i;
                        hitbox.attack = this;
                        hitbox.status = status;
                        hitbox.move = activeMove;
                        if (activeMove.isProjectile) hitboxes[i] = null;
                    }
                }
            }
            else if (attackFrames > activeMove.attacks[i].startupFrame + activeMove.attacks[i].activeFrames)
            {
                if (hitboxes.Count == i + 1)
                {
                    if (activeMove.attacks[i].hitbox != null)
                    {
                        Destroy(hitboxes[i]);
                    }
                }
            }
        }
    }

    public void RecoveryFrames()
    {
        newAttack = false;
        status.counterhitState = activeMove.forcedCounterHit;
        status.GoToState(Status.State.Recovery);
        //if (activeMove != null)
        //    if (activeMove.resetVelocityDuringRecovery)
        //        status.rb.velocity = Vector3.zero;

        ClearHitboxes();
    }

    void ProcessInvul()
    {
        //Execute properties
        //Invul
        if (activeMove.invincible)
        {
            if (attackFrames == activeMove.invincibleStart)
            {
                status.invincible = true;
                status.DisableHurtboxes();
            }
            else if (attackFrames >= activeMove.invincibleStart + activeMove.invincibleDuration)
            {
                status.invincible = false;
                status.EnableHurtboxes();
            }
        }
        //Noclip
        if (activeMove.noClip)
        {
            if (attackFrames == activeMove.noClipStart)
                status.DisableCollider();
            else if (attackFrames >= activeMove.noClipStart + activeMove.noClipDuration)
            {
                status.EnableCollider();
            }
        }
        //Noclip
        if (activeMove.linearInvul)
        {
            if (attackFrames == activeMove.linearInvulStart)
                status.linearInvul = true;
            else if (attackFrames >= activeMove.linearInvulStart + activeMove.linearInvulDuration)
            {
                status.linearInvul = false;
            }
        }
    }

    public void AttackProperties(Move move)
    {
        usedMoves.Add(move);
        FrameDataManager.Instance.UpdateFrameData();
        status.minusFrames = -(move.totalMoveDuration);
        status.EnableCollider();
        status.SetBlockState(move.collissionState);

        if (move.resetGatling) usedMoves.Clear();

        if (move.type == MoveType.Movement)
        {
            movementOption = move;
            movementFrames = GameHandler.Instance.gameFrameCount;
        }
        if (move.type != MoveType.Movement)
            movement.ResetRun();



        activeMove = move;
        attackID = move.animationID;
        attackString = false;
        jumpCancel = false;
        specialCancel = false;

        attackFrames = 0;

        ClearHitboxes();
        status.crossupState = move.crossupState;

        if (movement.ground) movement.LookAtOpponent();
        //Run momentum
        if (move.overrideVelocity) status.rb.velocity = Vector3.zero;
        else if (move.runMomentum) status.rb.velocity = status.rb.velocity * 0.5F;

        //Air properties
        if (move.useAirAction) movement.performedJumps++;

        Startup();
        landCancel = move.landCancel;
        ResetFrames();

        startupEvent?.Invoke();
        attacking = true;
        newAttack = true;
        movement.isMoving = false;
        if (move.stance != null)
        {
            moveset = move.stance;
        }
        else if (mainMoveset != null) moveset = mainMoveset;



        ExecuteFrame();
    }

    public bool CanUseMove(Move move)
    {
        if (move == null) return false;
        if (jumpFrameCounter > 0) return false;

        if (move.useAirAction)
        {
            if (movement.performedJumps <= 0)
            {
                print("Air Action");
                movement.performedJumps++;
                return true;
            }
            else return false;
        }

        if (!attacking) return true;


        if (attacking && gatling)
        {
            if (move.type == MoveType.Special && specialCancel)
            {
                print("Special Cancel");
                return true;
            }
            if (activeMove.gatlingMoves.Count <= 0) return false;
            if (move == null) return true;
            if (!activeMove.gatlingMoves.Contains(move)) return false;
            else
            {
                if (usedMoves.Contains(move))
                {
                    int duplicates = 1;
                    foreach (var item in move.gatlingMoves)
                    {
                        if (item == move) duplicates++;
                    }
                    foreach (var item in usedMoves)
                    {
                        if (item == move) duplicates--;
                    }
                    return duplicates > 0;
                }
                else return true;
            }
        }
        return false;
    }

    public bool TargetCombo(Move move)
    {
        if (move == null) return false;
        if (jumpFrameCounter > 0) return false;

        if (move.useAirAction)
        {
            if (movement.performedJumps > movement.multiJumps)
            {
                return false;
            }
        }
        if (attacking && attackString)
        {

            if (activeMove.targetComboMoves.Count > 0)
            {
                if (activeMove == move || move.targetComboMoves.Contains(activeMove))
                {
                    Attack(move.targetComboMoves[0]);
                    return true;
                }
            }
            if (activeMove.targetComboMoves.Contains(move))
            {
                //   print("Rekka cancel");
                AttackProperties(move);
                return true;
            }

        }
        return false;

    }

    public bool Attack(Move move)
    {

        if (TargetCombo(move)) return true;
        //print(GameHandler.Instance.gameFrameCount +" " +  move + " Attack start");
        if (!CanUseMove(move)) return false;
        else
        {
            AttackProperties(move);
            return true;
        }

    }

    void Startup()
    {
        status.GoToState(Status.State.Startup);
    }


    void Land()
    {

        if (landCancel)
        {
            newAttack = false;
            Idle();
            status.frameDataEvent?.Invoke();
        }
    }

    public void ResetFrames()
    {
        containerScript.DeactivateHitboxes();
        recoveryEvent?.Invoke();
        attacking = false;
        attackString = false;
    }

    void ResetCombo()
    {
        combo = 0;
    }

    void HitstunEvent()
    {
        newAttack = false;
        attacking = false;
        attackString = false;
        combo = 0;
        ClearHitboxes();
        containerScript.InterruptAttack();
        containerScript.DeactivateParticles();
    }

    public void JumpCancel()
    {

        {
            if (attacking) status.rb.velocity = Vector3.zero;
            //print(GameHandler.Instance.gameFrameCount + " Jump Cancel");
            status.GoToState(Status.State.Recovery);
            attackString = false;
            movementOption = null;
            activeMove = null;
            combo = 0;
            ClearHitboxes();
            attacking = false;
            landCancel = false;
            recoveryEvent?.Invoke();

            jumpFrameCounter = jumpMinusFrames;
            status.minusFrames = -jumpMinusFrames;
            status.frameDataEvent?.Invoke();
            movement.LookAtOpponent();
        }
    }

    public void ResetAllValues()
    {
        if (mainMoveset != null) moveset = mainMoveset;

        attackString = false;
        activeMove = null;
        combo = 0;
        status.GoToState(Status.State.Neutral);
        specialCancel = false;
        attacking = false;
        gatling = false;
        landCancel = false;
        status.counterhitState = false;
        recoveryEvent?.Invoke();

        //movement.storedDirection = Vector3.zero;

        ClearHitboxes();
    }

    public void ThrowBreak()
    {
        ResetAllValues();

        movement.LookAtOpponent();
    }

    public void Idle()
    {
        if (!newAttack)
        {
            ResetAllValues();

            if (status.groundState == GroundState.Grounded)
                movement.LookAtOpponent();
        }
    }
}
