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
    public Moveset moveset;
    [HeaderAttribute("Attack attributes")] 
    public Move activeMove;
    public bool canGatling;
    public int attackID;
    public int gameFrames;
    [FoldoutGroup("Move properties")] public bool attacking;
    [FoldoutGroup("Move properties")] public bool attackString;
    [FoldoutGroup("Move properties")] public bool landCancel;
    [FoldoutGroup("Move properties")] public bool jumpCancel;
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
        movement.jumpEvent += Idle;
        movement.landEvent += Land;
        status.neutralEvent += ResetCombo;
        status.hurtEvent += HitstunEvent;
        status.deathEvent += HitstunEvent;
    }

    private void Awake()
    {
        usedMoves = new List<Move>();
    }

    private void FixedUpdate()
    {
        ExecuteFrame();
        if (status.currentState == Status.State.Neutral || status.currentState == Status.State.Blockstun || status.currentState == Status.State.Hitstun) usedMoves.Clear();


    }

    public void ExecuteFrame()
    {
        if (attacking)
        {
            if (status.hitstopCounter <= 0)
                gameFrames++;

            if (canGatling && gameFrames >= activeMove.firstStartupFrame + activeMove.attacks[0].gatlingFrames)
            {
                attackString = true; 
                newAttack = false;
            }

            //Execute properties
            ProcessInvul();


            for (int i = 0; i < activeMove.sfx.Length; i++)
            {
                if (gameFrames == activeMove.sfx[i].startup)
                    Instantiate(activeMove.sfx[i].prefab);
            }
            //Execute momentum
            for (int i = 0; i < activeMove.m.Length; i++)
            {

                if (gameFrames > activeMove.m[i].startFrame + activeMove.m[i].duration)
                {
                    if (activeMove.m[i].resetVelocityDuringRecovery)
                        status.rb.velocity = Vector3.zero;
                }
                else if (gameFrames > activeMove.m[i].startFrame)
                {
                    //if (activeMove.m.resetVelocityDuringRecovery)
                    //    status.rb.velocity = Vector3.zero;
                    if (activeMove.overrideVelocity)
                        status.rb.velocity = movement.CalculateRight(activeMove.m[i].momentum.x) + transform.up * activeMove.m[i].momentum.y + transform.forward * activeMove.m[i].momentum.z;
                }
            }

            int firstStartupFrame = activeMove.attacks[0].startupFrame;

            int lastActiveFrame = activeMove.attacks[activeMove.attacks.Length - 1].startupFrame + activeMove.attacks[activeMove.attacks.Length - 1].activeFrames - 1;
            int totalMoveDuration = lastActiveFrame + activeMove.recoveryFrames;





            if (gameFrames > totalMoveDuration)
            {
                Idle();
            }
            else if (gameFrames < firstStartupFrame)
            {
                //print(attack.gameFrames + " Startup");
                StartupFrames();
            }
            else if (gameFrames <= lastActiveFrame)
            {
                ActiveFrames();
            }

            else if (gameFrames <= totalMoveDuration)
            {
                RecoveryFrames();
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
        //  print("Auto destroy hitbox");
        hitboxes.Clear();
    }

    public void ActiveFrames()
    {
        for (int i = 0; i < activeMove.attacks.Length; i++)
        {
            if (gameFrames < activeMove.attacks[i].startupFrame + activeMove.attacks[i].activeFrames && gameFrames >= activeMove.attacks[i].startupFrame)
            {
          
                status.GoToState(Status.State.Active);
                if (hitboxes.Count < i + 1)
                {
                    if (activeMove.attacks[i].hitbox != null)
                    {
                        if (activeMove.type == MoveType.Special) hitboxes.Add(Instantiate(activeMove.attacks[i].hitbox, transform.position + activeMove.attacks[i].hitbox.transform.position, transform.rotation));
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
                        if (activeMove.type == MoveType.Special) hitboxes[i] = null;
                    }
                }
            }
            else if (gameFrames > activeMove.attacks[i].startupFrame + activeMove.attacks[i].activeFrames)
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
        status.GoToState(Status.State.Recovery);
        if (activeMove != null)
            if (activeMove.resetVelocityDuringRecovery)
                status.rb.velocity = Vector3.zero;

        ClearHitboxes();
    }

    void ProcessInvul() {
        //Execute properties
        //Invul
        if (activeMove.invincible)
        {
            if (gameFrames == activeMove.invincibleStart)
                status.DisableHurtboxes();
            else if (gameFrames >= activeMove.invincibleStart + activeMove.invincibleDuration)
            {
                status.EnableHurtboxes();
            }
        }
        //Noclip
        if (activeMove.noClip)
        {
            if (gameFrames == activeMove.noClipStart)
                status.DisableCollider();
            else if (gameFrames >= activeMove.noClipStart + activeMove.noClipDuration)
            {
                status.EnableCollider();
            }
        }
    }

    public void AttackProperties(Move move)
    {
        status.minusFrames = -(move.totalMoveDuration);
        status.SetBlockState(move.collissionState);
        activeMove = move;
        attackID = move.animationID;
        attackString = false;
        canGatling = false;
        gameFrames = 0;


        ClearHitboxes();


        Vector3 desiredDirection = movement.strafeTarget.position - transform.position;
        Quaternion desiredRotation = Quaternion.Euler(0, Vector3.SignedAngle(Vector3.forward, new Vector3(desiredDirection.x, 0, desiredDirection.z), Vector3.up), 0);
        transform.rotation = desiredRotation;

        //Run momentum
        if (move.overrideVelocity) status.rb.velocity = Vector3.zero;
        else if (move.runMomentum) status.rb.velocity = status.rb.velocity * 0.5F;
        movement.ResetRun();

        Startup();
        landCancel = activeMove.landCancel;
        ResetFrames();

        startupEvent?.Invoke();
        attacking = true;
        newAttack = true;
        movement.isMoving = false;
      //  ProcessInvul();
        ExecuteFrame();
    }

    public bool CanUseMove(Move move)
    {
        if (move == null) return false;
        if (attackString)
        {
            if (move == null) return true;
            if (!activeMove.gatlingMoves.Contains(move)) return false;
        }
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

        return true;
    }

    public bool Attack(Move move)
    {
        if (!CanUseMove(move)) return false;
        else
        {
            usedMoves.Add(move);
            AttackProperties(move);
            return true;
        }

    }

    public bool CanCancel(Move move)
    {

        if (!activeMove.gatlingMoves.Contains(move)) return false;
        else return true;
    }


    void Startup()
    {
        status.GoToState(Status.State.Startup);
    }

    public void ParticleStart()
    {
        //if (weaponParticles != null)
        //    weaponParticles.ActivateParticle(activeMove.particleID);

        containerScript.ActivateParticle(activeMove.particleID);
    }

    void Land()
    {

        if (landCancel)
        {
            newAttack = false;
            Idle();
        }
    }

    public void ParticleEnd()
    {
        containerScript.DeactivateParticles();
    }

    public void Recovery()
    {
        if (!newAttack) return;

        status.GoToState(Status.State.Recovery);
        //if (activeMove != null)
        //    if (activeMove.resetVelocityDuringRecovery)
        //        status.rb.velocity = Vector3.zero;

        containerScript.DeactivateHitboxes();
        newAttack = false;
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
        combo = 0;
        ClearHitboxes();
        containerScript.InterruptAttack();
        containerScript.DeactivateParticles();
    }

    public void Idle()
    {
        if (!newAttack)
        {
            attackString = false;
            activeMove = null;
            combo = 0;
            status.GoToState(Status.State.Neutral);
            ClearHitboxes();
            attacking = false;
            landCancel = false;
            recoveryEvent?.Invoke();

        }
    }
}
