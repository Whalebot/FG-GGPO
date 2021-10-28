using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Sirenix.OdinInspector;

public class Status : MonoBehaviour
{


    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public Vector2 knockbackDirection;
    [HideInInspector] public bool isDead;
    public bool forcedCounterhit;
    public bool autoBlock;
    public bool blocking;

    [FoldoutGroup("Frames")] public int hitstunValue;
    [FoldoutGroup("Frames")] public int blockstunValue;
    [FoldoutGroup("Frames")] public bool inBlockStun;
    [FoldoutGroup("Frames")] public bool inHitStun;
    [FoldoutGroup("Frames")] public int minusFrames;
    [FoldoutGroup("Frames")] public int cancelMinusFrames;

    [FoldoutGroup("Frames")] public int hitstopCounter;
    [FoldoutGroup("Frames")] public Vector3 pushbackVector;
    [FoldoutGroup("Frames")] public int knockdownValue;
    [FoldoutGroup("Frames")] public int throwBreakWindow;
    [FoldoutGroup("Frames")] public int throwBreakCounter;
    public delegate void StatusEvent();
    public StatusEvent healthEvent;
    public StatusEvent hurtEvent;
    public StatusEvent deathEvent;
    public StatusEvent blockEvent;
    public StatusEvent frameDataEvent;
    public StatusEvent hitEvent;
    public StatusEvent punishEvent;
    public StatusEvent invincibleEvent;
    public StatusEvent reversalEvent;
    public StatusEvent recoveryEvent;
    public StatusEvent comboReset;

    public delegate void TransitionEvent();
    public TransitionEvent neutralEvent;
    public TransitionEvent animationEvent;
    public TransitionEvent blockstunEvent;
    public TransitionEvent hitstunEvent;
    public TransitionEvent counterhitEvent;
    public TransitionEvent knockdownEvent;
    public TransitionEvent wakeupEvent;
    public TransitionEvent throwEvent;
    public TransitionEvent throwBreakEvent;


    public delegate void AnimationEvent(int animationID);
    public AnimationEvent takeAnimationEvent;
    [HideInInspector] public bool newMove;
    [FoldoutGroup("Variables")] public int groundPushbackDuration;
    [HideInInspector] public int pushbackCounter;

    CharacterSFX characterSFX;
    Movement mov;
    [FoldoutGroup("Variables")] public int wakeupRecovery;
    int wakeupValue;


    [FoldoutGroup("State")] public State currentState;
    [FoldoutGroup("State")] public GroundState groundState;
    [FoldoutGroup("State")] public BlockState blockState;
    public enum State { Neutral, Startup, Active, Recovery, Hitstun, Blockstun, Knockdown, Wakeup, LockedAnimation }
    [FoldoutGroup("State")] public bool counterhitState;
    [FoldoutGroup("State")] public bool crossupState;
    [FoldoutGroup("State")] public bool invincible;
    [FoldoutGroup("State")] public bool projectileInvul;
    [FoldoutGroup("State")] public bool linearInvul;
    [FoldoutGroup("State")] public bool airInvul;
    [FoldoutGroup("State")] public bool headInvul;
    [FoldoutGroup("State")] public bool bodyInvul;
    [FoldoutGroup("State")] public bool footInvul;

    public int maxHealth;
    public int health;
    public int maxMeter;
    public int meter;
    public int burstGauge;


    public int comboCounter;
    public int lastAttackDamage;
    public int comboDamage;
    public float proration;
    public Vector3 pushVelocity;

    [FoldoutGroup("Components")] public GameObject standingHurtbox;
    [FoldoutGroup("Components")] public GameObject crouchingHurtbox;
    [FoldoutGroup("Components")] public GameObject groundedHurtbox;
    [FoldoutGroup("Components")] public GameObject standingCollider;
    [FoldoutGroup("Components")] public GameObject crouchingCollider;
    [FoldoutGroup("Components")] public GameObject jumpingCollider;
    [FoldoutGroup("Components")] public GameObject standingBlocker;
    [FoldoutGroup("Components")] public GameObject crouchingBlocker;
    [FoldoutGroup("Components")] public GameObject jumpingBlocker;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mov = GetComponent<Movement>();
        characterSFX = GetComponentInChildren<CharacterSFX>();

    }

    private void Start()
    {
        GameHandler.Instance.advanceGameState += ExecuteFrame;
        mov.landEvent += Land;
        ResetStatus();
    }

    public int Health
    {
        get
        {
            return health;
        }
        set
        {
            if (isDead)
                if (health == value) return;

            health = Mathf.Clamp(value, 0, maxHealth);

            healthEvent?.Invoke();
            if (health <= 0 && !isDead)
            {
                Death();
            }
        }
    }

    public int Meter
    {
        get
        {
            return meter;
        }
        set
        {

            meter = Mathf.Clamp(value, 0, maxMeter);
        }
    }

    public int HitStun
    {
        get { return hitstunValue; }
        set
        {
            if (value <= 0) return;
            hitstunValue = value;
            comboCounter++;
            //GoToState(State.Hitstun);
            counterhitState = false;
        }
    }

    public int BlockStun
    {
        get { return blockstunValue; }
        set
        {
            blockstunValue = value;
            GoToState(State.Blockstun);
        }
    }

    public int BurstGauge
    {
        get { return burstGauge; }
        set
        {
            burstGauge = Mathf.Clamp(value, 0, 6000);

        }
    }

    [Button]
    public void ResetStatus()
    {
        wakeupEvent?.Invoke();
        health = maxHealth;
        meter = 0;

        comboCounter = 0;
        hitstunValue = 0;
        blockstunValue = 0;
        inHitStun = false;
        GroundRecovery();
        ResetInvincibilities();
        isDead = false;

        groundState = GroundState.Grounded;
        currentState = State.Neutral;
    }

    public void ResetInvincibilities()
    {
        invincible = false;
        airInvul = false;
        bodyInvul = false;
        footInvul = false;
        headInvul = false;
        linearInvul = false;
        projectileInvul = false;
    }

    void Land()
    {
        if (currentState == State.Hitstun || currentState == State.Knockdown)
        {
            GoToGroundState(GroundState.Knockdown);
        }
        else
        {
            GoToGroundState(GroundState.Grounded);
        }
    }

    void ActivateCollider()
    {
        standingCollider.SetActive(false);
        jumpingCollider.SetActive(false);
        crouchingCollider.SetActive(false);

        standingHurtbox.SetActive(false);
        crouchingHurtbox.SetActive(false);
        groundedHurtbox.SetActive(false);

        switch (groundState)
        {
            case GroundState.Grounded:
                if (currentState == State.Wakeup) crouchingCollider.SetActive(true);
                else
                {
                    if (blockState == BlockState.Standing)
                    {
                        standingCollider.SetActive(true);
                        standingHurtbox.SetActive(true);
                    }
                    else
                    {
                        crouchingCollider.SetActive(true);
                        crouchingHurtbox.SetActive(true);
                    }
                }
                break;
            case GroundState.Airborne:
                jumpingCollider.SetActive(true);
                standingHurtbox.SetActive(true);
                break;
            case GroundState.Knockdown:
                crouchingCollider.SetActive(true);
                groundedHurtbox.SetActive(true);
                break;
            default:
                break;
        }



    }

    public void AirCollider()
    {
        standingCollider.layer = LayerMask.NameToLayer("AirCollision");
        crouchingCollider.layer = LayerMask.NameToLayer("AirCollision");
        jumpingCollider.layer = LayerMask.NameToLayer("AirCollision");
    }
    public void EnableCollider()
    {
        standingCollider.layer = LayerMask.NameToLayer("Collision");
        crouchingCollider.layer = LayerMask.NameToLayer("Collision");
        jumpingCollider.layer = LayerMask.NameToLayer("Collision");
        standingBlocker.layer = LayerMask.NameToLayer("Blocker");
        crouchingBlocker.layer = LayerMask.NameToLayer("Blocker");
        jumpingBlocker.layer = LayerMask.NameToLayer("Blocker");
    }
    public void DisableCollider()
    {
        standingCollider.layer = LayerMask.NameToLayer("Disabled");
        crouchingCollider.layer = LayerMask.NameToLayer("Disabled");
        jumpingCollider.layer = LayerMask.NameToLayer("Disabled");
        standingBlocker.layer = LayerMask.NameToLayer("Disabled");
        crouchingBlocker.layer = LayerMask.NameToLayer("Disabled");
        jumpingBlocker.layer = LayerMask.NameToLayer("Disabled");
    }
    public void EnableHurtboxes()
    {
        standingHurtbox.layer = LayerMask.NameToLayer("Hurtbox");
        crouchingHurtbox.layer = LayerMask.NameToLayer("Hurtbox");
        groundedHurtbox.layer = LayerMask.NameToLayer("Hurtbox");
    }
    public void DisableHurtboxes()
    {
        standingHurtbox.layer = LayerMask.NameToLayer("Disabled");
        crouchingHurtbox.layer = LayerMask.NameToLayer("Disabled");
        groundedHurtbox.layer = LayerMask.NameToLayer("Disabled");
    }

    void ExecuteFrame()
    {
        if (GameHandler.Instance.superFlash)
        {
            return;
        }

        if (newMove)
        {
            hitstopCounter--;
            if (hitstopCounter > 1) rb.velocity = Vector3.zero;
            else
            if (hitstopCounter <= 0)
            {
                newMove = false;
                ApplyPushback();
            }
        }

        BurstGauge++;

        StateMachine();
    }

    public void SetBlockState(BlockState state)
    {
        blockState = state;
        switch (state)
        {
            case BlockState.Crouching: break;
            case BlockState.Standing: break;
            case BlockState.Airborne: break;
            default: break;
        }
        ActivateCollider();
    }

    void ResolveBlockstun()
    {
        if (blockstunValue > 0)
        {

            blockstunValue--;
        }
        if (blockstunValue <= 0 && inBlockStun)
        {
            GoToState(State.Neutral);
            blockstunValue = 0;
            inBlockStun = false;
        }
    }

    void ResolveHitstun()
    {
        if (rb.velocity.y > 1) GoToGroundState(GroundState.Airborne);

        if (hitstunValue > 0)
        {
            hitstunValue--;

            if (groundState != GroundState.Airborne)
            {
                pushbackCounter--;
                if (pushbackCounter <= 0)
                {
                    rb.velocity = Vector3.zero;
                }
            }
        }
        if (hitstunValue <= 0 && inHitStun)
        {
            recoveryEvent?.Invoke();
            if (groundState == GroundState.Knockdown || currentState == State.Knockdown)
                KnockdownRecovery();
            else if (groundState == GroundState.Airborne)
            {
                if (!mov.ground)
                    AirRecovery();
                else KnockdownRecovery();
            }
            else GroundRecovery();
        }
    }



    void AirRecovery()
    {
        //  print("air recovery");
        Instantiate(VFXManager.Instance.recoveryFX, transform.position + VFXManager.Instance.recoveryFX.transform.localPosition, Quaternion.identity);
        GoToGroundState(GroundState.Airborne);
        GoToState(State.Wakeup);
        //GoToState(State.Wakeup);
        hitstunValue = 0;
        comboCounter = 0;
        inHitStun = false;
    }

    void KnockdownRecovery()
    {
        // print("kd recovery");
        // Instantiate(VFXManager.Instance.recoveryFX, transform.position + VFXManager.Instance.recoveryFX.transform.localPosition, Quaternion.identity);
        GoToGroundState(GroundState.Grounded);
        GoToState(State.Wakeup);
        hitstunValue = 0;
        comboCounter = 0;
        inHitStun = false;
    }

    void GroundRecovery()
    {
        //   print("ground recovery");
        //Instantiate(VFXManager.Instance.recoveryFX, transform.position + VFXManager.Instance.recoveryFX.transform.localPosition, Quaternion.identity);
        GoToGroundState(GroundState.Grounded);
        GoToState(State.Neutral);
        hitstunValue = 0;
        comboCounter = 0;
        inHitStun = false;
    }

    void ResolveKnockdown()
    {
        if (knockdownValue > 0)
        {
            knockdownValue--;
        }
        else if (knockdownValue <= 0)
        {
            KnockdownRecovery();
        }
    }



    void StateMachine()
    {
        if (currentState == State.Neutral && HitStun > 0)
        {
            print("Budget solution");
            GoToState(State.Hitstun);
        }


        switch (currentState)
        {
            case State.Neutral:
                counterhitState = forcedCounterhit;
                if (autoBlock) blocking = true;
                // PushVelocity();
                break;
            case State.Hitstun:
                blocking = false;

                ResolveHitstun();
                //  PushVelocity();
                minusFrames = -HitStun;
                break;
            case State.Blockstun:
                if (autoBlock) blocking = true;
                ResolveBlockstun();
                minusFrames = -BlockStun;
                break;
            case State.Knockdown:
                blocking = false;
                // ResolveKnockdown();
                ResolveHitstun();
                //   PushVelocity();
                minusFrames = -HitStun;
                break;
            case State.Wakeup:
                wakeupValue--;
                //invincible = true;
                if (wakeupValue <= 0)
                {
                    wakeupEvent?.Invoke();
                    GoToState(State.Neutral);

                }
                break;
            case State.LockedAnimation:
                throwBreakCounter--;

                break;
            default: break;
        }
    }
    public void GoToGroundState(GroundState s)
    {
        groundState = s;
        ActivateCollider();
    }

    public void GoToState(State transitionState)
    {
        if (currentState == State.Wakeup && transitionState != State.Hitstun)
        {
            wakeupEvent?.Invoke();
            Instantiate(VFXManager.Instance.wakeupFX, transform.position + VFXManager.Instance.wakeupFX.transform.localPosition, Quaternion.identity);
        }
        //   if (currentState == State.LockedAnimation && transitionState == State.Neutral) return;
        currentState = transitionState;

        switch (transitionState)
        {
            case State.Neutral:
                invincible = false;
                linearInvul = false;
                wakeupEvent?.Invoke();
                EnableHurtboxes();
                neutralEvent?.Invoke(); break;
            case State.Startup:
                blocking = false;
                minusFrames++;
                break;
            case State.Active:
                blocking = false;
                minusFrames++;
                break;
            case State.Recovery:
                crossupState = false;
                blocking = false;
                minusFrames++;
                //invincible = false;
                EnableHurtboxes();
                break;
            case State.Hitstun:
                inHitStun = true;
                linearInvul = false;

                EnableHurtboxes();
                minusFrames = -HitStun;
                hitEvent?.Invoke();
                hitstunEvent?.Invoke();
                break;
            case State.Blockstun:
                EnableHurtboxes();
                inBlockStun = true;
                minusFrames = -BlockStun;
                blockstunEvent?.Invoke(); break;
            case State.Knockdown:

                knockdownValue = HitStun + 50;
                EnableHurtboxes();
                inHitStun = true;
                minusFrames = -HitStun;
                hitEvent?.Invoke();
                knockdownEvent?.Invoke();
                //frameDataEvent?.Invoke();

                break;
            case State.Wakeup:
                wakeupValue = wakeupRecovery;

                break;
            case State.LockedAnimation:
                blocking = false;
                //DisableHurtboxes();
                break;
            default: break;
        }

    }

    public void TakeHit(HitProperty hit, Vector3 dir, HitState hitState, bool returnWallPushback)
    {
        float angle = Mathf.Abs(Vector3.SignedAngle(transform.forward, dir, Vector3.up));
        Vector3 push = dir * hit.pushback.z + Vector3.Cross(Vector3.up, dir) * hit.pushback.x + Vector3.up * hit.pushback.y;

        ResetInvincibilities();

        if (currentState == State.Recovery)
        {
            punishEvent?.Invoke();
        }

        if (hitState == HitState.Launch || push.y > 1)
        {
            GoToGroundState(GroundState.Airborne);
            SetBlockState(BlockState.Airborne);
        }
        else if (hitState == HitState.Knockdown)
        {
            GoToGroundState(GroundState.Knockdown);
        }
        else
        {
            if (groundState != GroundState.Grounded || currentState == State.Knockdown || currentState == State.Wakeup)
            {
                GoToGroundState(GroundState.Airborne);
            }
            else GoToGroundState(GroundState.Grounded);
        }

        mov.storedDirection = Vector3.zero;
        TakePushback(push, returnWallPushback);


        int val = 0;
        int stunVal = 0;
        if (comboCounter > 0)
        {
            val = (int)(hit.damage * proration * ComboSystem.Instance.comboDamageBaseProration);
            if (val < hit.minimumDamage) val = hit.minimumDamage;
            stunVal = (int)(hit.stun * proration);
            if (stunVal < hit.minimumStun) stunVal = hit.minimumStun;
        }
        else
        {
            comboDamage = 0;
            val = hit.damage;
            stunVal = hit.stun;
            proration = 1;
        }

        lastAttackDamage = val;
        comboDamage += val;
        Health -= val;
        HitStun = stunVal + hit.hitstop;

        proration *= hit.proration;
        mov.runMomentumCounter = 0;

        if (groundState == GroundState.Knockdown)
            GoToState(State.Knockdown);
        else
            GoToState(State.Hitstun);
        takeAnimationEvent?.Invoke(hit.hitID);
        hurtEvent?.Invoke();
    }

    public void TakeBlock(int damage, Vector3 kb, int stunVal, Vector3 dir, bool returnWallPushback)
    {
        ResetInvincibilities();
        mov.runMomentumCounter = 0;
        mov.storedDirection = Vector3.zero;

        Health -= damage;
        float angle = Mathf.Abs(Vector3.SignedAngle(transform.forward, dir, Vector3.up));
        TakePushback(kb, returnWallPushback);
        BlockStun = stunVal;
        blockEvent?.Invoke();
    }

    public void TakePushback(Vector3 direction, bool returnWallPushback)
    {
        float temp = Vector3.SignedAngle(new Vector3(direction.x, 0, direction.z), transform.forward, Vector3.up);
        Vector3 tempVector = (Quaternion.Euler(0, temp, 0) * new Vector3(direction.x, 0, direction.z)).normalized;
        knockbackDirection = new Vector2(tempVector.x, tempVector.z);


        rb.velocity = Vector3.zero;
        pushbackVector = direction;
        Vector3 tempV = pushbackVector;
        tempV.y = 0;
        float mag = tempV.magnitude;
        if (returnWallPushback)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, tempV, out hit, mag / 4, mov.wallMask))
            {
                Status enemyStatus = GameHandler.Instance.ReturnPlayer(transform).GetComponent<Status>();
                enemyStatus.newMove = true;
                if (enemyStatus.groundState == GroundState.Grounded)
                    enemyStatus.pushbackVector += (-tempV.normalized * (mag * 3 / 4 - hit.distance));
            }
        }
    }

    public void TakeThrow(int animationID)
    {
        ResetInvincibilities();
        if (currentState == State.Recovery)
        {
            punishEvent?.Invoke();
        }
        mov.runMomentumCounter = 0;
        mov.storedDirection = Vector3.zero;
        rb.velocity = Vector3.zero;

        throwEvent?.Invoke();
        takeAnimationEvent?.Invoke(animationID);
        throwBreakCounter = throwBreakWindow;
        GoToState(State.LockedAnimation);
    }

    public void ThrowBreak()
    {
        print("Throw Break");
        throwBreakEvent?.Invoke();
    }

    public void Hitstop()
    {
        pushbackVector = rb.velocity;
        rb.velocity = Vector3.zero;
    }

    public void ApplyPushback()
    {
        rb.velocity = pushbackVector;
        pushbackCounter = groundPushbackDuration;
    }

    public void Death()
    {
        isDead = true;
        deathEvent?.Invoke();
    }
}

