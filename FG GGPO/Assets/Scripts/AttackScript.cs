using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class AttackScript : MonoBehaviour
{
    private Status status;
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
    [FoldoutGroup("Debug")] public List<GameObject> projectiles;

    [FoldoutGroup("Debug")] public Move movementOption;
    [FoldoutGroup("Jump Startup")] public int jumpFrameCounter;
    [FoldoutGroup("Jump Startup")] public int jumpActionDelay;
    [FoldoutGroup("Jump Startup")] public int jumpActionDelayCounter;
    [FoldoutGroup("Jump Startup")] public bool jumpDelay;
    [FoldoutGroup("Move properties")] public bool attacking;
    [FoldoutGroup("Move properties")] public bool attackString;
    [FoldoutGroup("Move properties")] public bool landCancel;
    [FoldoutGroup("Move properties")] public bool jumpCancel;
    [FoldoutGroup("Move properties")] public bool specialCancel;
    [HideInInspector] public bool newAttack;
    [HideInInspector] public int combo;
    List<Move> usedMoves;

    // Start is called before the first frame update
    void Start()
    {
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

    public void ExecuteFrame()
    {
        if (status.hitstopCounter <= 0)
        {
            if (jumpFrameCounter > 0)
            {
                jumpFrameCounter--;
                if (jumpFrameCounter <= 0)
                {
                    status.GoToState(Status.State.Neutral);
                    jumpActionDelayCounter = jumpActionDelay;
                }

            }
            if (jumpActionDelayCounter > 0)
            {
                jumpDelay = true;
                jumpActionDelayCounter--;
                if (jumpActionDelayCounter <= 0)
                {
                    jumpDelay = false;
                }

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
                if (attackFrames > activeMove.firstStartupFrame + activeMove.attacks[0].gatlingFrames)
                {

                    attackString = true;
                    newAttack = false;
                }

                //Execute properties
                ProcessInvul();

                SpawnFX();

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

    public void SpawnFX()
    {
        if (activeMove != null)
        {
            foreach (var item in activeMove.vfx)
            {
                if (attackFrames == item.startup)
                {
                    GameObject fx = Instantiate(item.prefab, transform.position, transform.rotation, hitboxContainer);
                    fx.transform.localPosition = item.position;
                    fx.transform.localRotation = Quaternion.Euler(item.rotation);
                    fx.transform.localScale = item.scale;
                    if (GameHandler.Instance.IsPlayer1(transform))
                        fx.GetComponent<VFXScript>().ID = 1;
                    else fx.GetComponent<VFXScript>().ID = 2;
                    fx.transform.SetParent(null);
                }
            }
            foreach (var item in activeMove.sfx)
            {
                if (attackFrames == item.startup)
                {
                    GameObject fx = Instantiate(item.prefab, transform.position, transform.rotation, hitboxContainer);
                    fx.transform.localPosition = item.prefab.transform.localPosition;
                    fx.transform.localRotation = item.prefab.transform.rotation;
                    fx.transform.SetParent(null);
                }
            }
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
                            hitboxes[i].transform.localRotation = activeMove.attacks[i].hitbox.transform.rotation;
                            hitboxes[i].transform.SetParent(null);
                        }
                        else
                        {
                            hitboxes.Add(Instantiate(activeMove.attacks[i].hitbox, hitboxContainer.position, transform.rotation, hitboxContainer));
                            hitboxes[i].transform.localPosition = activeMove.attacks[i].hitbox.transform.localPosition;
                            hitboxes[i].transform.localRotation = activeMove.attacks[i].hitbox.transform.rotation;
                        }
                        Hitbox hitbox = hitboxes[i].GetComponentInChildren<Hitbox>();
                        hitbox.hitboxID = i;
                        hitbox.attack = this;
                        hitbox.status = status;
                        hitbox.move = activeMove;
                        if (activeMove.attacks[i].attackType == AttackType.Projectile)
                        {
                            projectiles.Add(hitboxes[i]);
                            hitboxes[i] = null;
                        }
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
                status.invincibleEvent?.Invoke();
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
        //Projectile Invul
        if (activeMove.projectileInvul)
        {
            if (attackFrames == activeMove.projectileInvulStart)
                status.projectileInvul = true;
            else if (attackFrames >= activeMove.projectileInvulStart + activeMove.projectileInvulDuration)
            {
                status.projectileInvul = false;
            }
        }
        //Linear Invul
        if (activeMove.linearInvul)
        {
            if (attackFrames == activeMove.linearInvulStart)
                status.linearInvul = true;
            else if (attackFrames >= activeMove.linearInvulStart + activeMove.linearInvulDuration)
            {
                status.linearInvul = false;
            }
        }
        //air Invul
        if (activeMove.airInvul)
        {
            if (attackFrames == activeMove.airInvulStart)
                status.airInvul = true;
            else if (attackFrames >= activeMove.airInvulStart + activeMove.airInvulDuration)
            {
                status.airInvul = false;
            }
        }
        //head Invul
        if (activeMove.headInvul)
        {
            if (attackFrames == activeMove.headInvulStart)
                status.headInvul = true;
            else if (attackFrames >= activeMove.headInvulStart + activeMove.headInvulDuration)
            {
                status.headInvul = false;
            }
        }
        //body Invul
        if (activeMove.bodyInvul)
        {
            if (attackFrames == activeMove.bodyInvulStart)
                status.bodyInvul = true;
            else if (attackFrames >= activeMove.bodyInvulStart + activeMove.bodyInvulDuration)
            {
                status.bodyInvul = false;
            }
        }
        //foot Invul
        if (activeMove.footInvul)
        {
            if (attackFrames == activeMove.footInvulStart)
                status.footInvul = true;
            else if (attackFrames >= activeMove.footInvulStart + activeMove.footInvulDuration)
            {
                status.footInvul = false;
            }
        }
    }

    public void AttackProperties(Move move)
    {
        usedMoves.Add(move);
        FrameDataManager.Instance.UpdateFrameData();
        if (move.targetComboMoves.Count > 0)
        {
            status.cancelMinusFrames = move.totalMoveDuration - move.firstGatlingFrame + 1;
        }

        status.Meter -= move.meterCost;
        status.minusFrames = -(move.totalMoveDuration);
        status.EnableCollider();
        status.SetBlockState(move.collissionState);

        float angle = Vector3.Angle(transform.forward, movement.strafeTarget.position - transform.position);

        if (move.resetGatling) usedMoves.Clear();

        if (move.type == MoveType.Movement)
        {
            movement.runMomentumCounter = 0;
            movementOption = move;
            movementFrames = GameHandler.Instance.gameFrameCount;

        }
        if (move.type != MoveType.Movement)
            movement.ResetRun();



        activeMove = move;
        attackID = move.animationID;
        attackString = false;
        gatling = false;
        jumpCancel = false;
        specialCancel = false;

        attackFrames = 0;

        ClearHitboxes();
        status.crossupState = move.crossupState;
        if (movement.ground) movement.LookAtOpponent();
        else if (angle < 90 && move.aimOnStartup) movement.LookAtOpponent();
        //Run momentum
        if (move.overrideVelocity) status.rb.velocity = Vector3.zero;
        else if (move.runMomentum) status.rb.velocity = status.rb.velocity * 0.5F;

        //Air properties
        if (move.useAirAction) movement.performedJumps++;

        Startup();
        landCancel = move.landCancel;

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

        if (move.meterCost > status.Meter) return false;
        if (move.useAirAction && !attacking)
        {
            if (movement.performedJumps <= 0)
            {
                movement.performedJumps++;
                return true;
            }
            else return false;
        }

        if (!attacking) return true;

        if (specialCancel)
        {
            if (move.type == MoveType.Special || move.type == MoveType.Super)
                return true;
        }

        if (attacking && gatling)
        {
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
                if (activeMove.targetComboMoves.Contains(move))
                {
                    AttackProperties(move);
                    return true;
                }

                if (usedMoves.Contains(move) && activeMove == move || move.targetComboMoves.Contains(activeMove))
                {
                    Attack(move.targetComboMoves[0]);
                    return true;
                }

            }
        }
        return false;

    }

    public bool ProjectileLimit(Move move)
    {
        if (move != null)
        {
            if (move.projectileLimit > 0 && projectiles.Count > 0)
            {
                int projectileCount = 0;
                for (int i = projectiles.Count - 1; i > -1; i--)
                {
                    if (projectiles[i] == null) projectiles.RemoveAt(i);
                    else if (move.sharedLimitProjectiles.Contains(projectiles[i].GetComponent<Hitbox>().move)) projectileCount++;

                }
                if (projectileCount >= move.projectileLimit) return true;
            }
        }
        return false;
    }

    public bool Attack(Move move)
    {
        if (jumpDelay) return false;

        if (TargetCombo(move)) return true;
        if (ProjectileLimit(move)) return false;
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
            status.minusFrames = 0;
            status.frameDataEvent?.Invoke();
        }
    }

    void ResetCombo()
    {
        combo = 0;
    }

    void HitstunEvent()
    {
        movementOption = null;

        ResetAllValues();
    }

    public void JumpCancel()
    {
        if (attacking) status.rb.velocity = Vector3.zero;
        if (mainMoveset != null) moveset = mainMoveset;

        status.GoToState(Status.State.Recovery);
        attackString = false;
        movementOption = null;

        if (activeMove != null)
        {
            activeMove = null;
        }

        combo = 0;
        ClearHitboxes();
        attacking = false;
        landCancel = false;
        recoveryEvent?.Invoke();

        jumpFrameCounter = movement.jumpStartFrames;
        status.minusFrames = -movement.jumpStartFrames;
        status.frameDataEvent?.Invoke();
        movement.LookAtOpponent();
    }

    public void ResetAllValues()
    {
        if (mainMoveset != null) moveset = mainMoveset;

        newAttack = false;
        attackString = false;
        if (activeMove != null)
        {
            if (activeMove.type != MoveType.Movement)
            {
                movement.ResetRun();
            }
            activeMove = null;
        }

        combo = 0;
        jumpFrameCounter = 0;
        specialCancel = false;
        attacking = false;
        gatling = false;
        landCancel = false;

        status.counterhitState = false;
        status.projectileInvul = false;
        status.invincible = false;
        status.linearInvul = false;

        recoveryEvent?.Invoke();
        usedMoves.Clear();
        //Cucks airdash
        //movement.storedDirection = Vector3.zero;

        ClearHitboxes();
    }

    public void ThrowBreak()
    {
        ResetAllValues();
        status.GoToState(Status.State.Neutral);
        movement.LookAtOpponent();
    }

    public void Idle()
    {
        if (!newAttack)
        {
            ResetAllValues();
            status.GoToState(Status.State.Neutral);
            if (status.groundState == GroundState.Grounded)
                movement.LookAtOpponent();
        }
    }
}
