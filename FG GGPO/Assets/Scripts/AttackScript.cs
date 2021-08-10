using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackScript : MonoBehaviour
{
    private Status status;
    public HitboxContainer containerScript;
    public Moveset moveset;


    Movement movement;

    public Move activeMove;
    public bool canGatling;
    CharacterSFX sfx;
    public Transform hitboxContainer;

    public delegate void AttackEvent();
    public AttackEvent startupEvent;
    public AttackEvent activeEvent;
    public AttackEvent recoveryEvent;
    public AttackEvent parryEvent;
    public AttackEvent blockEvent;

    [HeaderAttribute("Attack attributes")]
    public int attackID;
    [HideInInspector] public bool canAttack;
    [HideInInspector] public bool attacking;
    public bool attackString;
    public bool holdAttack;
    [HideInInspector] public bool landCancel;
    bool newAttack;
    [HideInInspector] public int combo;
    [HideInInspector] public bool fullCancel;
    [HideInInspector] public bool iFrames;
    public bool startupRotation;
    int lastAttackID;
    int momentumCount;
    public bool block;

    // Start is called before the first frame update
    void Start()
    {
        containerScript = GetComponentInChildren<HitboxContainer>();
        status = GetComponent<Status>();
        movement = GetComponent<Movement>();
        sfx = GetComponentInChildren<CharacterSFX>();
        movement.jumpEvent += Idle;

        status.neutralEvent += ResetCombo;
        status.hurtEvent += HitstunEvent;
        status.deathEvent += HitstunEvent;
    }

    private void FixedUpdate()
    {

    }

    public void ExecuteFrame()
    {

    }

    public void AttackProperties(Move move)
    {
        momentumCount = 0;
        activeMove = move;
        attackID = move.animationID;
        attackString = false;
        canGatling = false;

        GameObject tempGO;
        if (!move.verticalRotation)
        {
            tempGO = Instantiate(activeMove.attackPrefab, transform.position, transform.rotation, hitboxContainer);
        }
        else { tempGO = Instantiate(activeMove.attackPrefab, transform.position, hitboxContainer.rotation, hitboxContainer); }

        Vector3 desiredDirection = movement.strafeTarget.position - transform.position;
        Quaternion desiredRotation = Quaternion.Euler(0, Vector3.SignedAngle(Vector3.forward, new Vector3(desiredDirection.x, 0, desiredDirection.z), Vector3.up), 0);
        transform.rotation = desiredRotation;

        AttackContainer attackContainer = tempGO.GetComponentInChildren<AttackContainer>();

        attackContainer.status = status;
        attackContainer.attack = this;
        attackContainer.move = move;

        if (movement.strafeTarget != null) attackContainer.target = movement.strafeTarget;
        Startup();
        status.GoToState(Status.State.Startup);

        AttackMomentum();

        fullCancel = activeMove.fullCancelable;
        holdAttack = activeMove.holdAttack;

        iFrames = activeMove.iFrames;


        ResetFrames();

        startupEvent?.Invoke();
        attacking = true;
        newAttack = true;
        movement.isMoving = false;
    }

    public void Attack(Move move)
    {
        AttackProperties(move);
    }


    public void AttackMomentum()
    {
        if (activeMove.overrideVelocity)
            status.rb.velocity = Vector3.zero;
        status.rb.AddForce(transform.right * activeMove.Momentum.x + transform.up * activeMove.Momentum.y + transform.forward * activeMove.Momentum.z, ForceMode.VelocityChange);
    }

    public void AnimMomemtun()
    {
        if (lastAttackID == attackID) { }

        if (activeMove.overrideVelocity)
            status.rb.velocity = Vector3.zero;
        if (activeMove.momentumArray.Length > momentumCount)
        {
            status.rb.AddForce(transform.right * activeMove.momentumArray[momentumCount].x + transform.up * activeMove.momentumArray[momentumCount].y + transform.forward * activeMove.momentumArray[momentumCount].z, ForceMode.VelocityChange);
        }
        momentumCount++;

    }

    void Startup()
    {
        status.GoToState(Status.State.Startup);
        startupRotation = true;
    }

    public void StartRotation()
    {
        startupRotation = true;
    }


    public void StopRotation()
    {
        startupRotation = false;
    }

    public void Active()
    {
        startupRotation = false;
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

    public void ParticleEnd()
    {
        //if (weaponParticles != null) weaponParticles.DeactivateParticles();
        containerScript.DeactivateParticles();
    }

    public void Recovery()
    {
        status.GoToState(Status.State.Recovery);

        if (activeMove != null)
            if (activeMove.resetVelocityDuringRecovery)
                status.rb.velocity = Vector3.zero;

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
        fullCancel = false;
        holdAttack = false;
        newAttack = false;

        combo = 0;

        containerScript.InterruptAttack();
        containerScript.DeactivateParticles();
    }

    public void Idle()
    {
        if (!newAttack)
        {
            attackString = false;
            fullCancel = false;

            combo = 0;
            status.GoToState(Status.State.Neutral);
            containerScript.DeactivateParticles();
            attacking = false;
            recoveryEvent?.Invoke();

        }
    }
}
