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
    Vector3 knockbackDirection;
    Vector3 aVector;
    public Transform body;
   [SerializeField] List<Status> enemyList;
    MeshRenderer mr;
    Transform colPos;
    bool hitOnce;

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
        // if (!other.transform.IsChildOf(body))
        {
            if (hitOnce) return;
            Status enemyStatus = other.GetComponentInParent<Status>();

            if (enemyStatus != null)
            {
                if (status == enemyStatus) return;

                if (!enemyList.Contains(enemyStatus))
                {
                    colPos = other.gameObject.transform;
                    if (enemyStatus.invincible) return;

                    enemyList.Add(enemyStatus);
                    //Hurtbox hurtbox = other.GetComponent<Hurtbox>();
                    //if (hurtbox != null)
                    //{
                    //    DoDamage(enemyStatus, hurtbox.damageMultiplier);
                    //}
                    //else
                    DoDamage(enemyStatus, 1);
                }
            }
        }
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
        hitOnce = true;

        attack.canGatling = move.canGatling;
        knockbackDirection = (new Vector3(other.transform.position.x, 0, other.transform.position.z) - new Vector3(body.position.x, 0, body.position.z)).normalized;
        // knockbackDirection = (status.transform.forward).normalized;
        CheckAttack(other, move.attacks[hitboxID]);
    }

    void CheckAttack(Status other, Attack attack)
    {
        //Check for block
        if (other.blocking)
        {
            //Check if blocked wrong height
            if (attack.attackHeight == AttackHeight.Low && other.blockState == BlockState.Standing || attack.attackHeight == AttackHeight.Overhead && other.blockState == BlockState.Crouching)
            {
                if (other.counterhitState)
                    ExecuteCounterHit(attack.groundCounterhitProperty, other);
                else
                    ExecuteHit(attack.groundHitProperty, other);
                return;
            }
            if (other.groundState == GroundState.Grounded)
            {
                ExecuteBlock(attack.groundBlockProperty, other);
            }
            //Check for airborne
            else if (other.groundState == GroundState.Airborne)
                ExecuteBlock(attack.airBlockProperty, other);
        }
        else
        {
            if (other.groundState == GroundState.Grounded)
            {
                if (other.counterhitState)
                    ExecuteCounterHit(attack.groundCounterhitProperty, other);
                else
                    ExecuteHit(attack.groundHitProperty, other);
            }
            //Check for airborne or knockdown state
            else if (other.groundState == GroundState.Airborne || other.groundState == GroundState.Knockdown)
            {
                if (other.counterhitState)
                    ExecuteCounterHit(attack.airCounterhitProperty, other);
                else
                    ExecuteHit(attack.airHitProperty, other);
            }
        }
    }

    void ExecuteBlock(HitProperty hit, Status other)
    {
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
        other.TakeBlock(hit.damage, aVector, hit.stun + hit.hitstop, knockbackDirection);
    }

    void ExecuteHit(HitProperty hit, Status other)
    {
        attack.jumpCancel = move.jumpCancelOnHit;
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

        other.TakeHit(hit.damage, aVector, hit.stun + hit.hitstop, knockbackDirection, hit.hitState);
    }

    void ExecuteCounterHit(HitProperty hit, Status other)
    {
        attack.jumpCancel = move.jumpCancelOnHit;
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

        other.TakeHit(hit.damage, aVector, hit.stun + hit.hitstop, knockbackDirection, hit.hitState);
        status.counterhitEvent?.Invoke();
    }
}
