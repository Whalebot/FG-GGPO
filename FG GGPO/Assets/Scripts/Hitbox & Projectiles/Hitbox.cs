using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public float baseDamage = 1;
    [HideInInspector] public int totalDamage;
    [HideInInspector] public int hitboxID;
    [HideInInspector] public AttackScript attack;
    [HideInInspector] public Move move;
    [HideInInspector] public Status status;
    [SerializeField] bool canClash = true;
    Vector3 knockbackDirection;
    Vector3 aVector;
    public Transform body;
    [SerializeField] public List<Status> enemyList;
    MeshRenderer mr;
    protected bool returnWallPushback;
    protected Transform colPos;
    [SerializeField] protected bool hitOnce;

    private void Awake()
    {
        // print(" Hitbox active");
        mr = GetComponent<MeshRenderer>();

        if (body == null) body = transform.parent;
        if (body == null) body = transform;
        if (GameHandler.Instance.showHitboxes)
        {
            mr.enabled = true;
        }
        else
        {
            mr.enabled = false;
        }
        enemyList = new List<Status>();

    }
    private void Start()
    {

    }

    public void OnTriggerEnter(Collider other)
    {
        if (hitOnce) return;
        Status enemyStatus = other.GetComponentInParent<Status>();
        Hitbox hitbox = other.GetComponent<Hitbox>();
        colPos = other.gameObject.transform;
       // if (!attack.attacking) return;
        if (hitbox != null && canClash)
        {
            if (hitbox.GetType() == typeof(Projectile)) return;
            if (CheckInvul(enemyStatus) && hitbox.CheckInvul(status))
            {
                hitOnce = true;

                Clash(enemyStatus);
                return;
            }
        }
        else if (enemyStatus != null)
        {
            print("Pog");
            if (status == enemyStatus) return;

            if (!enemyList.Contains(enemyStatus))
            {
                canClash = false;
                if (!CheckInvul(enemyStatus)) return;

                enemyList.Add(enemyStatus);
                DoDamage(enemyStatus, 1);
                return;
            }
        }


    }

    public bool CheckInvul(Status enemyStatus)
    {
        if (enemyStatus.invincible) return false;
        else if (enemyStatus.linearInvul && !move.attacks[hitboxID].homing) return false;
        switch (move.attacks[hitboxID].bodyProperty)
        {
            case BodyProperty.Foot:
                if (enemyStatus.footInvul) return false;
                break;
            case BodyProperty.Body:
                if (enemyStatus.bodyInvul) return false;
                break;
            case BodyProperty.Head:
                if (enemyStatus.headInvul) return false;
                break;
            case BodyProperty.Air:
                if (enemyStatus.airInvul) return false;
                break;
            default:
                break;
        }

        return true;
    }

    void OnDisable()
    {
        enemyList.Clear();
    }
    void OnEnable()
    {
        enemyList.Clear();

    }
    public virtual void DoDamage(Status other, float dmgMod)
    {

        CheckAttack(other, move.attacks[hitboxID]);
    }

    public virtual void CheckAttack(Status other, Attack tempAttack)
    {
        hitOnce = true;
        returnWallPushback = move.attacks[hitboxID].attackType != AttackType.Projectile;
        knockbackDirection = transform.forward;
        knockbackDirection.y = 0;
        knockbackDirection = knockbackDirection.normalized;



        status.cancelMinusFrames = (move.totalMoveDuration - (tempAttack.gatlingFrames + tempAttack.startupFrame));
        other.cancelMinusFrames = -(move.totalMoveDuration - (tempAttack.gatlingFrames + tempAttack.startupFrame));

        //Check for block
        if (other.blocking)
        {
            //Check if blocked wrong height
            if (tempAttack.attackHeight == AttackHeight.Low && other.blockState == BlockState.Standing || tempAttack.attackHeight == AttackHeight.Overhead && other.blockState == BlockState.Crouching)
            {
                if (other.counterhitState)
                    ExecuteCounterHit(tempAttack.groundCounterhitProperty, other);
                else
                    ExecuteHit(tempAttack.groundHitProperty, other);
                return;
            }
            if (other.groundState == GroundState.Grounded)
            {
                ExecuteBlock(tempAttack.groundBlockProperty, other);
            }
            //Check for airborne
            else if (other.groundState == GroundState.Airborne)
                ExecuteBlock(tempAttack.airBlockProperty, other);
        }
        else
        {
            if (other.groundState == GroundState.Grounded)
            {
                if (other.counterhitState)
                    ExecuteCounterHit(tempAttack.groundCounterhitProperty, other);
                else
                    ExecuteHit(tempAttack.groundHitProperty, other);
            }
            //Check for airborne or knockdown state
            else if (other.groundState == GroundState.Airborne || other.groundState == GroundState.Knockdown)
            {
                if (other.counterhitState)
                    ExecuteCounterHit(tempAttack.airCounterhitProperty, other);
                else
                    ExecuteHit(tempAttack.airHitProperty, other);
            }
        }
    }

    void ExecuteBlock(HitProperty hit, Status other)
    {
        attack.gatling = move.gatlingMoves.Count > 0 && move.gatlingCancelOnBlock;
        attack.specialCancel = move.specialCancelOnBlock;
        attack.jumpCancel = move.jumpCancelOnBlock;
        status.Meter += hit.meterGain;
        other.Meter += hit.meterGain / 2;


        //Enemy Hitstop
        other.newMove = true;
        other.hitstopCounter = hit.hitstop;

        if (move.noHitstopOnSelf)
        {
            status.minusFrames = -(move.totalMoveDuration - attack.attackFrames);
        }
        else
        {
            status.minusFrames = -(move.totalMoveDuration - attack.attackFrames + hit.hitstop);

            //Own hitstop
            status.Hitstop();
            status.newMove = true;
            status.hitstopCounter = hit.hitstop;
        }

        //Block FX
        if (move.blockFX != null)
            Instantiate(move.blockFX, colPos.position, colPos.rotation);
        else Instantiate(VFXManager.Instance.defaultBlockVFX, colPos.position, colPos.rotation);
        if (move.blockSFX != null)
            Instantiate(move.blockSFX, colPos.position, colPos.rotation);
        else Instantiate(VFXManager.Instance.defaultBlockSFX, colPos.position, colPos.rotation);


        //Calculate direction
        aVector = knockbackDirection * hit.pushback.z + Vector3.Cross(Vector3.up, knockbackDirection) * hit.pushback.x + Vector3.up * hit.pushback.y;
        other.TakeBlock(hit.damage, aVector, hit.stun + hit.hitstop, knockbackDirection, returnWallPushback);
    }



    void ExecuteHit(HitProperty hit, Status other)
    {
        attack.gatling = move.gatlingMoves.Count > 0 && move.gatlingCancelOnHit;
        attack.specialCancel = move.specialCancelOnHit;
        attack.jumpCancel = move.jumpCancelOnHit;
        status.Meter += hit.meterGain;
        other.Meter += hit.meterGain / 2;
        other.burstGauge += hit.meterGain * 60;
        //Enemy Hitstop
        other.newMove = true;
        other.hitstopCounter = hit.hitstop;

        if (move.noHitstopOnSelf)
        {
            status.minusFrames = -(move.totalMoveDuration - attack.attackFrames);
        }
        else
        {
            status.minusFrames = -(move.totalMoveDuration - attack.attackFrames + hit.hitstop);


            //Own hitstop
            status.Hitstop();
            status.newMove = true;
            status.hitstopCounter = hit.hitstop;
        }

        //Hit FX
        if (move.hitFX != null)
            Instantiate(move.hitFX, colPos.position, colPos.rotation);
        else
            Instantiate(VFXManager.Instance.defaultHitVFX, colPos.position, colPos.rotation);

        if (move.hitSFX != null)
            Instantiate(move.hitSFX, colPos.position, colPos.rotation);
        else
            Instantiate(VFXManager.Instance.defaultHitSFX, colPos.position, colPos.rotation);

        //Calculate direction
        aVector = knockbackDirection * hit.pushback.z + Vector3.Cross(Vector3.up, knockbackDirection) * hit.pushback.x + Vector3.up * hit.pushback.y;

        other.TakeHit(hit.damage, aVector, hit.stun + hit.hitstop, hit.proration, knockbackDirection, hit.hitState, hit.hitID, returnWallPushback);
    }

    void ExecuteCounterHit(HitProperty hit, Status other)
    {
        attack.gatling = move.gatlingMoves.Count > 0 && move.gatlingCancelOnHit;
        attack.specialCancel = move.specialCancelOnHit;
        attack.jumpCancel = move.jumpCancelOnHit;
        status.Meter += hit.meterGain;
        other.Meter += hit.meterGain / 2;
        other.burstGauge += hit.meterGain * 60;
        //Enemy Hitstop
        other.newMove = true;
        other.hitstopCounter = hit.hitstop;

        if (move.noHitstopOnSelf)
        {
            status.minusFrames = -(move.totalMoveDuration - attack.attackFrames);
        }
        else
        {
            status.minusFrames = -(move.totalMoveDuration - attack.attackFrames + hit.hitstop);

            //Own hitstop
            status.Hitstop();
            status.newMove = true;
            status.hitstopCounter = hit.hitstop;

            CameraManager.Instance.CounterhitCamera(hit.hitstop);
        }

        //Hit FX
        if (move.counterhitFX != null)
            Instantiate(move.counterhitFX, colPos.position, colPos.rotation);
        else
            Instantiate(VFXManager.Instance.counterHitVFX, colPos.position, colPos.rotation);

        if (move.counterhitSFX != null)
            Instantiate(move.counterhitSFX, colPos.position, colPos.rotation);
        else
            Instantiate(VFXManager.Instance.counterHitSFX, colPos.position, colPos.rotation);

        //Calculate direction
        aVector = knockbackDirection * hit.pushback.z + Vector3.Cross(Vector3.up, knockbackDirection) * hit.pushback.x + Vector3.up * hit.pushback.y;

        other.TakeHit(hit.damage, aVector, hit.stun + hit.hitstop, hit.proration, knockbackDirection, hit.hitState, hit.hitID, returnWallPushback);
        status.counterhitEvent?.Invoke();
    }
    void Clash(Status enemyStatus)
    {
        print("Clash");
        canClash = false;
        Collider col = GetComponent<Collider>();
        col.enabled = false;
        status.Hitstop();
        status.newMove = true;
        status.hitstopCounter = 25;

        //Hit FX
        if (move.hitFX != null)
            Instantiate(move.hitFX, colPos.position, colPos.rotation);
        else
            Instantiate(VFXManager.Instance.defaultHitVFX, colPos.position, colPos.rotation);

        if (move.hitSFX != null)
            Instantiate(move.hitSFX, colPos.position, colPos.rotation);
        else
            Instantiate(VFXManager.Instance.defaultHitSFX, colPos.position, colPos.rotation);


        attack.newAttack = false;
        attack.Idle();
    }
}
