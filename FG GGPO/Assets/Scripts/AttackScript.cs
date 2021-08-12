using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackScript : MonoBehaviour
{
    [SerializeField] private Status status;
    public HitboxContainer containerScript;
    public Moveset moveset;


    Movement movement;

    public Move activeMove;
    public bool canGatling;
    CharacterSFX sfx;
    public Transform hitboxContainer;
    public GameObject activeHitbox;

    public delegate void AttackEvent();
    public AttackEvent startupEvent;
    public AttackEvent activeEvent;
    public AttackEvent recoveryEvent;
    public AttackEvent parryEvent;
    public AttackEvent blockEvent;

    [HeaderAttribute("Attack attributes")]
    public int attackID;
    public int gameFrames;
    [HideInInspector] public bool canAttack;
    [HideInInspector] public bool attacking;
    public bool attackString;
    public bool landCancel;
    bool newAttack;
    [HideInInspector] public int combo;
    int lastAttackID;
    public bool block;
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
        if (attacking)
        {
            gameFrames++;

            for (int i = 0; i < activeMove.m.Length; i++)
            {
                if (!activeMove.m[i].impulse) {
                    if (gameFrames > activeMove.m[i].startFrame + activeMove.m[i].duration)
                    {
                        if (activeMove.m[i].resetVelocityDuringRecovery)
                            status.rb.velocity = Vector3.zero;
                    }
                    else if (gameFrames > activeMove.m[i].startFrame)
                    {
                        //if (activeMove.m.resetVelocityDuringRecovery)
                        //    status.rb.velocity = Vector3.zero;
                        //Vector3 desiredDirection = movement.strafeTarget.position - transform.position;
                        //Quaternion desiredRotation = Quaternion.Euler(0, Vector3.SignedAngle(Vector3.forward, new Vector3(desiredDirection.x, 0, desiredDirection.z), Vector3.up), 0);
                        //transform.rotation = desiredRotation;
                        status.rb.velocity = (CalculateRight() * activeMove.m[i].momentum.x + transform.up * activeMove.m[i].momentum.y + transform.forward * activeMove.m[i].momentum.z);
                    }
                }
            else
                if (gameFrames == activeMove.m[i].startFrame)
                {
                    Vector3 desiredDirection = movement.strafeTarget.position - transform.position;
                    Quaternion desiredRotation = Quaternion.Euler(0, Vector3.SignedAngle(Vector3.forward, new Vector3(desiredDirection.x, 0, desiredDirection.z), Vector3.up), 0);
                    transform.rotation = desiredRotation;
                    status.rb.velocity = Vector3.zero;
                    status.rb.AddForce(transform.right * activeMove.m[i].momentum.x + transform.up * activeMove.m[i].momentum.y + transform.forward * activeMove.m[i].momentum.z, ForceMode.VelocityChange);
                }
            }

        }
        if (status.currentState == Status.State.Neutral || status.currentState == Status.State.Blockstun || status.currentState == Status.State.Hitstun) usedMoves.Clear();


    }

    Vector3 CalculateRight()
    {
        Vector3 v1 = transform.right;
        //float distance = Vector3.Distance(movement.strafeTarget.position, transform.position);
        //v1 = transform.right + transform.forward * (0.6F/distance);
        return v1.normalized;
    }

    public void ExecuteFrame()
    {
        if (activeMove == null) return;
        if (status.currentState == Status.State.Startup) StartupFrames();
        if (status.currentState == Status.State.Active) ActiveFrames();
        if (status.currentState == Status.State.Recovery) Recovery();
    }

    public void StartupFrames()
    {
        status.GoToState(Status.State.Startup);
        if (activeHitbox != null)
        {
            if (Application.isEditor)
                DestroyImmediate(activeHitbox);
            else
            {
                Destroy(activeHitbox);
            }
        }
    }

    public void ActiveFrames()
    {
        status.GoToState(Status.State.Active);
        if (activeMove.type == Move.MoveType.Special)
        {
            if (activeHitbox == null)
            {
                activeHitbox = Instantiate(activeMove.attackPrefab, transform.position + activeMove.attackPrefab.transform.position, transform.rotation);

                Hitbox hitbox = activeHitbox.GetComponentInChildren<Hitbox>();

                hitbox.status = status;
                hitbox.move = activeMove;
                hitbox.attack = this;
            }
        }
        else
        if (activeHitbox == null)
        {
            if (activeMove.attackPrefab == null) return;
            activeHitbox = Instantiate(activeMove.attackPrefab, transform.position, transform.rotation, hitboxContainer);

            AttackContainer attackContainer = activeHitbox.GetComponentInChildren<AttackContainer>();

            attackContainer.status = status;
            attackContainer.attack = this;
            attackContainer.move = activeMove;
        }

    }

    public void RecoveryFrames()
    {
        newAttack = false;
        status.GoToState(Status.State.Recovery);
        if (activeMove != null)
            if (activeMove.resetVelocityDuringRecovery)
                status.rb.velocity = Vector3.zero;

        if (activeHitbox != null && activeMove.type != Move.MoveType.Special)
        {
            if (Application.isEditor)
                DestroyImmediate(activeHitbox);
            else
            {
                Destroy(activeHitbox);
            }
        }
        else activeHitbox = null;
    }

    public void AttackProperties(Move move)
    {
        status.minusFrames = -(move.startupFrames + move.activeFrames + move.recoveryFrames);
        status.SetBlockState(move.collissionState);
        activeMove = move;
        attackID = move.animationID;
        attackString = false;
        canGatling = false;
        gameFrames = 0;

        if (move.invincible) {
            status.invincible = true;
        }

        if (move.noClip) {
            status.DisableHurtboxes();
        }

        if (activeHitbox != null) Destroy(activeHitbox);

        Vector3 desiredDirection = movement.strafeTarget.position - transform.position;
        Quaternion desiredRotation = Quaternion.Euler(0, Vector3.SignedAngle(Vector3.forward, new Vector3(desiredDirection.x, 0, desiredDirection.z), Vector3.up), 0);
        transform.rotation = desiredRotation;

        Startup();
        status.GoToState(Status.State.Startup);

        AttackMomentum();
        landCancel = activeMove.landCancel;

        ResetFrames();

        startupEvent?.Invoke();
        attacking = true;
        newAttack = true;
        movement.isMoving = false;
    }

    public void Attack(Move move)
    {
        if (usedMoves.Contains(move)) return;
        usedMoves.Add(move);

        AttackProperties(move);
    }


    public void AttackMomentum()
    {
    }

    public void AnimMomemtun()
    {
    }

    void Startup()
    {
        status.GoToState(Status.State.Startup);
    }

    public void Active()
    {
        if (status.currentState != Status.State.Startup) return;
        activeEvent?.Invoke();

        status.GoToState(Status.State.Active);

        if (sfx != null)
            sfx.AttackSFX(attackID);
        containerScript.ActivateHitbox(0);
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

    public void AttackLink()
    {
        attackString = true;
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
            containerScript.DeactivateHitboxes();
            containerScript.DeactivateParticles();
            attacking = false;
            landCancel = false;
            recoveryEvent?.Invoke();

        }
    }
}
