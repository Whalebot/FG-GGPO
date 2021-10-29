using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class CharacterAnimator : MonoBehaviour
{
    private Status status;
    private Animator anim;
    private Movement movement;
    public bool hitstop;
    [SerializeField] private AttackScript attack;
    private float runSpeed;
    //  private Character character;

    public int frame;

    float x, y;
    float zeroFloat = 0f;
    [SerializeField]
    float maxSpeed;
    [SerializeField]
    private float deaccelerateSpeed;
    float tempDirection = 0F;
    public List<AnimationData> animationData;
    public float strafeSmooth;

    public Move move;
    public bool isPaused;
    public GameObject graphics;
    private void Awake()
    {

    }


    // Start is called before the first frame update
    void Start()
    {
        animationData = new List<AnimationData>();


        //   status = GetComponentInParent<Status>();
        anim = GetComponent<Animator>();
        movement = GetComponentInParent<Movement>();
        attack = GetComponentInParent<AttackScript>();
        status = GetComponentInParent<Status>();
        GameHandler.Instance.rollbackEvent += RollbackAnimation;
        GameHandler.Instance.advanceGameState += ExecuteFrame;


        status.hitstunEvent += HitStun;
        //status.hitstunEvent += HitStop;
        status.knockdownEvent += Knockdown;
        status.wakeupEvent += WakeUp;
        status.blockEvent += Block;
        status.takeAnimationEvent += LockedAnimation;
        status.throwBreakEvent += ThrowBreak;

       

        if (GameHandler.Instance.IsPlayer1(transform.parent))
        {
            GameHandler.Instance.p1IntroEvent += Intro;
            GameHandler.Instance.p1WinEvent += Win;
            GameHandler.Instance.hideP1Event += HideGraphics;
            GameHandler.Instance.displayP1Event += DisplayGraphics;
        }
        else
        {
            GameHandler.Instance.p2IntroEvent += Intro;
            GameHandler.Instance.p2WinEvent += Win;
            GameHandler.Instance.hideP2Event += HideGraphics;
            GameHandler.Instance.displayP2Event += DisplayGraphics;
        }

        if (movement != null)
        {
            movement.jumpEvent += Jump;
        }
        if (attack != null)
        {
            attack.startupEvent += StartAttack;
            attack.recoveryEvent += AttackRecovery;
        }
    }

    public void HideGraphics() {
        graphics.SetActive(false);
    }
    public void DisplayGraphics() {
        graphics.SetActive(true);
    }

    public void Intro()
    {
        anim.SetTrigger("Intro");
    }

    public void Win()
    {
        print("Winnering");
        anim.SetTrigger("Win");
    }

    public void ThrowBreak()
    {
        anim.SetTrigger("ThrowBreak");
    }

    public void HitStop()
    {
        StartCoroutine(HitstopStart());
    }

    IEnumerator HitstopStart()
    {
        hitstop = false;
        yield return new WaitForFixedUpdate();
        // yield return new WaitForFixedUpdate();
        hitstop = true;
    }
    void ExecuteFrame()
    {
        if (!GameHandler.Instance.runNormally) anim.enabled = true;
        anim.SetBool("Cutscene", GameHandler.cutscene);
        if (GameHandler.Instance.superFlash)
        {
            if (attack.superCounter > 0)
            {
                anim.speed = 0.5F;
            }
            else
            {
                anim.enabled = false;
                StartCoroutine(PauseAnimation());
                return;
            }
        }
        else
        {
            anim.speed = 1;
        }


        frame = Mathf.RoundToInt(anim.GetCurrentAnimatorStateInfo(0).normalizedTime * anim.GetCurrentAnimatorStateInfo(0).length / (1f / 60f));

        SaveAnimationData();
        if (status.hitstopCounter > 0 && !hitstop) { HitStop(); }
        else if (status.hitstopCounter <= 0)
        {
            StopCoroutine(HitstopStart());
            hitstop = false;
        }
        StatusAnimation();
        BlockAnimation();
        //if (status.hitstopCounter > 0)
        //    anim.speed = 1 / status.hitstopCounter;
        //else
        //    anim.speed = 1;
        anim.enabled = !hitstop;
        MovementAnimation();
        if (!GameHandler.Instance.runNormally) StartCoroutine(PauseAnimation());
    }

    IEnumerator PauseAnimation()
    {
        yield return new WaitForFixedUpdate();
        anim.enabled = false;
    }

    private void OnValidate()
    {
        EditorAnimation();
    }
    public void EditorAnimation()
    {

        //anim = GetComponent<Animator>();
        //frame = Mathf.Clamp(frame, 0, (int)(anim.GetCurrentAnimatorStateInfo(0).length / (1f / 60f)));
        //if (move != null)
        //{
        //    anim.Play("Base Layer.Attacking." + move.name, 0, (float)frame / (anim.GetCurrentAnimatorStateInfo(0).length / (1f / 60f)));
        //    anim.Update(Time.fixedDeltaTime);
        //}


        //if (move != null)
        //{
        //    if (frame >= move.startupFrames + move.activeFrames + move.recoveryFrames)
        //    {
        //        if (Application.isEditor && attack.activeHitbox != null)
        //            DestroyImmediate(attack.activeHitbox);
        //    }
        //    else if (frame >= move.startupFrames + move.activeFrames)
        //    {

        //        if (move.attackPrefab == null) return;
        //        if (attack.activeHitbox == null)
        //        {
        //            attack.activeHitbox = Instantiate(move.attackPrefab, transform.position, transform.rotation, attack.hitboxContainer);

        //            AttackContainer attackContainer = attack.activeHitbox.GetComponentInChildren<AttackContainer>();
        //            if (attackContainer != null)
        //            {
        //                attackContainer.status = status;
        //                attackContainer.attack = attack;
        //                attackContainer.move = move;
        //            }
        //        }
        //    }
        //    else if (frame < move.startupFrames)
        //    {
        //        if (Application.isEditor && attack.activeHitbox != null)
        //            DestroyImmediate(attack.activeHitbox);
        //    }
        //}

    }

    void RollbackAnimation(int i)
    {
        anim.SetBool("Attacking", animationData[animationData.Count - i].attacking);
        anim.Play(animationData[animationData.Count - i].hash, 0, animationData[animationData.Count - i].frame / 60f);
        // anim.PlayInFixedTime("Attacking.5A",0, 0.1F);
        animationData.RemoveRange(i, animationData.Count - i);
    }


    public void SaveAnimationData()
    {
        AnimationData data = new AnimationData(anim.GetCurrentAnimatorStateInfo(0).fullPathHash, (int)(anim.GetCurrentAnimatorStateInfo(0).normalizedTime * anim.GetCurrentAnimatorStateInfo(0).length / (1f / 60f)));
        data.attacking = anim.GetBool("Attacking");
        animationData.Add(data);
    }

    void BackDash()
    {
        anim.SetTrigger("BackDash");
    }

    void BlockAnimation()
    {
        anim.SetBool("Blocking", status.BlockStun > 0);
    }

    void Block()
    {
        anim.SetTrigger("Block");
    }

    public void ParryAnimation()
    {
        anim.SetTrigger("Parry");
    }

    void StatusAnimation()
    {
        anim.SetBool("Dead", status.isDead);
        anim.SetBool("Hitstun", status.inHitStun);
    }

    void HitStun()
    {
        anim.SetBool("Ground", movement.ground && status.groundState == GroundState.Grounded);

        if (status.pushbackVector.y < 0)
            anim.SetInteger("Falling", -1);
        else if (Mathf.Abs(status.pushbackVector.y) < 0.1F) anim.SetInteger("Falling", 0);
        else anim.SetInteger("Falling", 1);

        //anim.SetBool("Knockdown", false);
        anim.SetFloat("HitX", status.knockbackDirection.x);
        anim.SetFloat("HitY", status.knockbackDirection.y);
        anim.SetTrigger("Hit");

    }

    public void LockedAnimation(int id)
    {
        anim.SetBool("Ground", movement.ground && status.groundState == GroundState.Grounded);
        anim.SetInteger("HitID", id);
        if (status.pushbackVector.y < 0)
            anim.SetInteger("Falling", -1);
        else if (Mathf.Abs(status.pushbackVector.y) < 0.1F) anim.SetInteger("Falling", 0);
        else anim.SetInteger("Falling", 1);

        //anim.SetBool("Knockdown", false);
        anim.SetFloat("HitX", status.knockbackDirection.x);
        anim.SetFloat("HitY", status.knockbackDirection.y);
        anim.SetTrigger("Hit");

    }

    void Knockdown()
    {
        //print("Knockdown bool");
        anim.SetBool("Knockdown", true);
        anim.SetTrigger("Hit");

    }
    void WakeUp()
    {
        anim.SetBool("Knockdown", false);
    }

    void MovementAnimation()
    {
        if (status.hitstopCounter > 0) return;

        if (movement == null) return;
        RunSpeed();
        tempDirection = Mathf.Sign(movement.deltaAngle);


        anim.SetBool("Walking", movement.isMoving);
        anim.SetBool("Crouch", status.blockState == BlockState.Crouching);
        anim.SetBool("Run", movement.sprinting);
        x = Mathf.Lerp(x, movement.RelativeToForward().normalized.x, strafeSmooth);
        y = Mathf.Lerp(y, movement.RelativeToForward().normalized.z, strafeSmooth);

        anim.SetBool("Ground", movement.ground && status.groundState != GroundState.Airborne);

        anim.SetFloat("Horizontal", x);
        anim.SetFloat("Vertical", y);

        if (movement.ground)
        {
            if (movement.rb.velocity.y > 0)
                anim.SetInteger("Falling", 1);
            else if (movement.rb.velocity.y == 0)
                anim.SetInteger("Falling", 0);
            else anim.SetInteger("Falling", -1);
        }
        else
        {
            if (movement.rb.velocity.y > 4)
                anim.SetInteger("Falling", 1);
            else if (movement.rb.velocity.y == 0)
                anim.SetInteger("Falling", 0);
            else anim.SetInteger("Falling", -1);
        }


    }

    void Release() { anim.SetTrigger("Release"); }

    void StartAttack()
    {

        anim.SetTrigger("Attack");
        anim.SetBool("Attacking", true);
        anim.SetInteger("AttackID", attack.attackID);

    }

    void AttackRecovery()
    {
        anim.SetBool("Attacking", false);
    }

    private void RunSpeed()
    {
        if (!movement.isMoving) runSpeed = Mathf.Lerp(runSpeed, 0, deaccelerateSpeed);
        else if (movement.sprinting) runSpeed = Mathf.Lerp(runSpeed, 1, deaccelerateSpeed);
        else if (movement.isMoving) runSpeed = Mathf.Lerp(runSpeed, 0.25F, deaccelerateSpeed);
    }

    void Jump()
    {
        anim.SetTrigger("Jump");
    }
}
[System.Serializable]
public class AnimationData
{
    public AnimationData(int h, int f)
    {
        hash = h;
        frame = f;

    }

    public int hash;
    public int frame;
    public bool attacking;
}