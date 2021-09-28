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


    public delegate void TransitionEvent();
    public TransitionEvent neutralEvent;
    public TransitionEvent animationEvent;
    public TransitionEvent blockstunEvent;
    public TransitionEvent hitstunEvent;
    public TransitionEvent counterhitEvent;
    public TransitionEvent knockdownEvent;
    public TransitionEvent wakeupEvent;
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

    public int maxHealth;
    public int health;
    public int maxMeter;
    public int meter;


    public int comboCounter;
    public int lastAttackDamage;
    public int comboDamage;
    public float proration;

    [FoldoutGroup("Components")] public GameObject defaultHurtbox;
    [FoldoutGroup("Components")] public GameObject standingCollider;
    [FoldoutGroup("Components")] public GameObject crouchingCollider;
    [FoldoutGroup("Components")] public GameObject jumpingCollider;
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

    [Button]
    public void ResetStatus()
    {
        health = maxHealth;
        meter = 0;

        hitstunValue = 0;
        blockstunValue = 0;

        invincible = false;
        isDead = false;

        groundState = GroundState.Grounded;
        currentState = State.Neutral;
    }

    void Land()
    {
        //  if(groundState ==)
    }

    void ActivateCollider()
    {

        standingCollider.SetActive(blockState == BlockState.None && groundState == GroundState.Grounded || blockState == BlockState.Standing && groundState == GroundState.Grounded);
        crouchingCollider.SetActive(blockState == BlockState.Crouching || groundState == GroundState.Knockdown);

        jumpingCollider.SetActive(groundState == GroundState.Airborne);
    }

    public void AirHurtboxes()
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
    }
    public void DisableCollider()
    {
        standingCollider.layer = LayerMask.NameToLayer("Disabled");
        crouchingCollider.layer = LayerMask.NameToLayer("Disabled");
        jumpingCollider.layer = LayerMask.NameToLayer("Disabled");
    }
    public void EnableHurtboxes()
    {
        defaultHurtbox.layer = LayerMask.NameToLayer("Hurtbox");
    }
    public void DisableHurtboxes()
    {
        defaultHurtbox.layer = LayerMask.NameToLayer("Disabled");
    }

    void FixedUpdate()
    {
        //ExecuteFrame();
    }

    void ExecuteFrame()
    {
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
        StateMachine();
    }

    public void SetBlockState(BlockState state)
    {
        blockState = state;
        switch (state)
        {
            case BlockState.None: break;
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
        if (hitstunValue > 0)
        {
            hitstunValue--;

            if (groundState != GroundState.Airborne)
            {
                pushbackCounter--;
                if (pushbackCounter <= 0)
                {
                    print("pog");
                    rb.velocity = Vector3.zero;
                }
            }
        }
        if (hitstunValue <= 0 && inHitStun)
        {
            if (groundState == GroundState.Knockdown)
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
        wakeupEvent?.Invoke();
        Instantiate(VFXManager.Instance.recoveryFX, transform.position + Vector3.up * 0.5F, Quaternion.identity);

        GoToGroundState(GroundState.Airborne);
        GoToState(State.Wakeup);
        hitstunValue = 0;
        comboCounter = 0;
        inHitStun = false;
    }

    void KnockdownRecovery()
    {
        wakeupEvent?.Invoke();
        Instantiate(VFXManager.Instance.recoveryFX, transform.position + Vector3.up * 0.5F, Quaternion.identity);
        GoToGroundState(GroundState.Grounded);
        GoToState(State.Wakeup);
        hitstunValue = 0;
        comboCounter = 0;
        inHitStun = false;
    }

    void GroundRecovery()
    {

        Instantiate(VFXManager.Instance.recoveryFX, transform.position + Vector3.up * 0.5F, Quaternion.identity);
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


    //public void ReplaceStats(Stats stat1, Stats stat2)
    //{
    //    //Get stat definition and replace 1 with 2
    //    Stats def1 = stat1;
    //    Stats def2 = stat2;

    //    FieldInfo[] defInfo1 = def1.GetType().GetFields();
    //    FieldInfo[] defInfo2 = def2.GetType().GetFields();

    //    for (int i = 0; i < defInfo1.Length; i++)
    //    {
    //        object obj = def1;
    //        object obj2 = def2;
    //        // Debug.Log("fi name " + defInfo1[i].Name + " val " + defInfo1[i].GetValue(obj));
    //        defInfo1[i].SetValue(obj, defInfo2[i].GetValue(obj2));
    //    }
    //}

    void StateMachine()
    {
        switch (currentState)
        {
            case State.Neutral:
                if (forcedCounterhit) counterhitState = true;
                if (autoBlock) blocking = true;
                else
                    blocking = mov.holdBack;
                break;
            case State.Hitstun:
                blocking = false;
                SetBlockState(BlockState.None);
                ResolveHitstun();
                minusFrames = -HitStun;
                break;
            case State.Blockstun:
                if (autoBlock) blocking = true;
                else
                    blocking = mov.holdBack;
                ResolveBlockstun();

                minusFrames = -BlockStun;
                break;
            case State.Knockdown:
                blocking = false;
                SetBlockState(BlockState.None);
                // ResolveKnockdown();
                ResolveHitstun();
                minusFrames = -HitStun;
                break;
            case State.Wakeup:
                wakeupValue--;
                invincible = true;
                if (wakeupValue <= 0)
                {
                    GoToState(State.Neutral);
                    Instantiate(VFXManager.Instance.wakeupFX, transform.position + Vector3.up * 0.5F, Quaternion.identity);
                }
                break;
            default: break;
        }
    }
    public void GoToGroundState(GroundState s)
    {
        switch (s)
        {

            default:
                if (HitStun > 0 && s == GroundState.Grounded && groundState == GroundState.Airborne)
                {
                    groundState = GroundState.Knockdown;
                    break;
                }
                else groundState = s;

                break;
        }
        ActivateCollider();
    }

    public void GoToState(State transitionState)
    {
        currentState = transitionState;
        switch (transitionState)
        {
            case State.Neutral:
                invincible = false;
                EnableHurtboxes();
                neutralEvent?.Invoke(); break;
            case State.Startup:
                blocking = false;
                break;
            case State.Active:
                blocking = false;
                break;
            case State.Recovery:
                crossupState = false;
                blocking = false;
                invincible = false;
                EnableHurtboxes();
                break;
            case State.Hitstun:
                inHitStun = true;
                EnableHurtboxes();
                minusFrames = -HitStun;
                hitEvent?.Invoke();
                hitstunEvent?.Invoke();
                break;
            case State.Blockstun:
                EnableHurtboxes();
                inBlockStun = true;
                minusFrames = -BlockStun;
                hitEvent?.Invoke();
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
                DisableHurtboxes();
                break;
            default: break;
        }

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
            if (comboCounter > 0)
                hitstunValue = (int)(value * proration);
            else hitstunValue = value;
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

    public void TakeHit(int damage, Vector3 kb, int stunVal, float p, Vector3 dir, HitState hitState, int animationID)
    {
        float angle = Mathf.Abs(Vector3.SignedAngle(transform.forward, dir, Vector3.up));
        if (hitState == HitState.Launch || kb.y > 1)
        {
            GoToGroundState(GroundState.Airborne);
        }
        else if (hitState == HitState.Knockdown)
        {
            GoToGroundState(GroundState.Knockdown);
        }
        else
        {
            if (groundState != GroundState.Grounded)
            {
                GoToGroundState(GroundState.Airborne);
            }
            else GoToGroundState(GroundState.Grounded);
        }

        TakePushback(kb);

        int val = 0;
        if (comboCounter > 0)
            val = (int)(damage * proration * ComboSystem.Instance.comboDamageBaseProration);
        else
        {
            comboDamage = 0;
            val = damage;
            proration = 1;
        }

        lastAttackDamage = val;
        comboDamage += val;
        Health -= val;

        HitStun = stunVal;
        proration *= p;

        if (groundState == GroundState.Knockdown)
            GoToState(State.Knockdown);
        else
            GoToState(State.Hitstun);
        takeAnimationEvent?.Invoke(animationID);
        hurtEvent?.Invoke();
    }


    public void TakeBlock(int damage, Vector3 kb, int stunVal, Vector3 dir)
    {
        Health -= damage;
        float angle = Mathf.Abs(Vector3.SignedAngle(transform.forward, dir, Vector3.up));
        TakePushback(kb);
        BlockStun = stunVal;
        blockEvent?.Invoke();
    }

    public void TakePushback(Vector3 direction)
    {
        float temp = Vector3.SignedAngle(new Vector3(direction.x, 0, direction.z), transform.forward, Vector3.up);
        Vector3 tempVector = (Quaternion.Euler(0, temp, 0) * new Vector3(direction.x, 0, direction.z)).normalized;
        knockbackDirection = new Vector2(tempVector.x, tempVector.z);


        rb.velocity = Vector3.zero;
        pushbackVector = direction;

    }

    public void TakeThrow(int animationID)
    {
        takeAnimationEvent?.Invoke(animationID);
        GoToState(State.LockedAnimation);
    }

    public void ThrowBreak() {
        print("Throw Break");
        throwBreakEvent?.Invoke();
        KnockdownRecovery();
        
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

