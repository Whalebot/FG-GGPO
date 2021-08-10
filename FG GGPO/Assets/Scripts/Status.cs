using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Sirenix.OdinInspector;

public class Status : MonoBehaviour
{
    [HideInInspector] public int hitstunValue;
    [HideInInspector] public int blockstunValue;
    [HideInInspector] public bool inBlockStun;
    [HideInInspector] public bool inHitStun;
    public int knockdownValue;

    public bool autoDeath;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public Vector2 knockbackDirection;
    [HideInInspector] public bool isDead;
    public bool blocking;

    public delegate void StatusEvent();
    public StatusEvent healthEvent;
    public StatusEvent hurtEvent;
    public StatusEvent deathEvent;
    public StatusEvent blockEvent;

    public delegate void TransitionEvent();
    public TransitionEvent neutralEvent;
    public TransitionEvent animationEvent;
    public TransitionEvent blockstunEvent;
    public TransitionEvent hitstunEvent;
    public TransitionEvent knockdownEvent;
    public TransitionEvent wakeupEvent;

    CharacterSFX characterSFX;
    Movement mov;
    public int knockdownRecovery;

    public enum GroundState { Grounded, Airborne, Knockdown }
    public GroundState groundState;
    public enum BlockState { None, Standing, Crouching, Jumping }
    public BlockState blockState;
    public enum State { Neutral, Startup, Active, Recovery, Hitstun, Blockstun, Knockdown }
    [SerializeField] public State currentState;
    public int maxHealth;
    public int health;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mov = GetComponent<Movement>();
        characterSFX = GetComponentInChildren<CharacterSFX>();
        currentState = State.Neutral;
        ApplyCharacter();
    }

    private void Start()
    {
    }

    void FixedUpdate()
    {

        StateMachine();
    }

    void SetBlockState()
    {
        blocking = mov.holdBack; 

    }

    void ResolveBlockstun()
    {
        if (blockstunValue > 0)
        {

            blockstunValue--;
        }
        else if (blockstunValue <= 0 && inBlockStun)
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

        }
        else if (hitstunValue <= 0 && inHitStun)
        {
            if (groundState == GroundState.Knockdown)
                AirRecovery();

            else if (groundState == GroundState.Airborne)
                AirRecovery();
            else GroundRecovery();



        }
    }

    void AirRecovery()
    {
        wakeupEvent?.Invoke();
        Instantiate(VFXManager.Instance.recoveryFX, transform.position + Vector3.up * 0.5F, Quaternion.identity);
        groundState = GroundState.Grounded;
        GoToState(State.Neutral);
        hitstunValue = 0;
        inHitStun = false;
    }

    void GroundRecovery() {
        Instantiate(VFXManager.Instance.recoveryFX, transform.position + Vector3.up * 0.5F, Quaternion.identity);
        groundState = GroundState.Grounded;
        GoToState(State.Neutral);
        hitstunValue = 0;
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

            GoToState(State.Neutral);
            knockdownValue = 50;
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
                SetBlockState();
                break;
            case State.Hitstun:
                blocking = false;
                blockState = BlockState.None;
                ResolveHitstun();
                break;
            case State.Blockstun:
                SetBlockState();
                ResolveBlockstun();
                break;
            case State.Knockdown:
                blocking = false;
                blockState = BlockState.None;
                ResolveKnockdown();
                break;
            default: break;
        }
    }


    public void GoToState(State transitionState)
    {
        print(transitionState);
        switch (transitionState)
        {
            case State.Neutral:
                currentState = State.Neutral;
                neutralEvent?.Invoke(); break;
            case State.Startup:
                currentState = State.Startup;
                break;
            case State.Active:
                currentState = State.Active;
                break;
            case State.Recovery:
                currentState = State.Recovery;
                break;
            case State.Hitstun:
                currentState = State.Hitstun;
                inHitStun = true;
        

                hitstunEvent?.Invoke();
                break;
            case State.Blockstun:
                currentState = State.Blockstun;
                inBlockStun = true;
                blockstunEvent?.Invoke(); break;
            case State.Knockdown:
                currentState = State.Knockdown;

                knockdownEvent?.Invoke(); break;
            default: break;
        }
    }

    public void ApplyCharacter()
    {
        //if (character == null) return;
        //ReplaceStats(rawStats, character.stats);
        //ReplaceStats(baseStats, character.stats);
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

    public int HitStun
    {
        get { return hitstunValue; }
        set
        {
            if (value <= 0) return;

            hitstunValue = value;
            GoToState(State.Hitstun);
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

    public void TakeHit(int damage, Vector3 kb, int stunVal, Vector3 dir, float slowDur)
    {
        float angle = Mathf.Abs(Vector3.SignedAngle(transform.forward, dir, Vector3.up));
        if (groundState != GroundState.Grounded)
        {
            groundState = GroundState.Airborne;
        }

        TakePushback(kb);
        HitStun = stunVal;
        hurtEvent?.Invoke();
        Health -= damage;
    }
    public void TakeKnockdown(int damage, Vector3 kb, int stunVal, Vector3 dir, float slowDur)
    {
        GoToState(State.Hitstun);
        groundState = GroundState.Knockdown;
        HitStun = stunVal;
        knockdownEvent?.Invoke();
        hurtEvent?.Invoke();
        Health -= damage;
    }

    public void TakeBlock(int damage, Vector3 kb, int stunVal, Vector3 dir, float slowDur)
    {
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
            rb.AddForce(direction, ForceMode.VelocityChange);

    }

    public void Death()
    {
        isDead = true;
        deathEvent?.Invoke();
    }



}

