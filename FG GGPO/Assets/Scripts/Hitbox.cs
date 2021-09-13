using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public float baseDamage = 1;
    public float baseKnockback = 1;
    public int totalDamage;
    public int hitboxID;
    public AttackContainer container;
    public AttackScript attack;
    public Move move;
    public Status status;
    public GameObject projectile;
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
        if (container != null)
        {
            move = container.move;
            status = container.status;
        }

        if (body == null) body = transform.parent;

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
        if (container != null)
        {
            attack = container.attack;

        }
       // print(attack.gameFrames + " Hitbox active");
    }

    public void OnTriggerEnter(Collider other)
    {
       // if (!other.transform.IsChildOf(body))
        {if (hitOnce) return;
            Status enemyStatus = other.GetComponentInParent<Status>();
        
            if (enemyStatus != null)
            {
                if (status == enemyStatus) return;

                if (!enemyList.Contains(enemyStatus))
                {
                    colPos = other.gameObject.transform;
                    if (enemyStatus.invincible) return;
                    if (move == null)
                    {
                        move = container.move;
                        status = container.status;
                    }
                    enemyList.Add(enemyStatus);
                    Hurtbox hurtbox = other.GetComponent<Hurtbox>();
                    if (hurtbox != null)
                    {
                        DoDamage(enemyStatus, hurtbox.damageMultiplier);
                    }
                    else
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
        if (container != null)
        {
            attack = container.attack;

        }
        attack.canGatling = move.canGatling;
        knockbackDirection = (new Vector3(other.transform.position.x, 0, other.transform.position.z) - new Vector3(body.position.x, 0, body.position.z)).normalized;

        CheckAttack(other, move.attacks[hitboxID]);
    }

    void CheckAttack(Status other, Attack attack) {
        //Check for block
        if (other.blocking)
        {
            //Check if blocked wrong height
            if (attack.attackHeight == AttackHeight.Low && other.blockState == BlockState.Standing || attack.attackHeight == AttackHeight.Overhead && other.blockState == BlockState.Crouching)
            {

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
            if (other.counterhitState) { }

            if (other.groundState == GroundState.Grounded)
            {
                ExecuteHit(attack.groundHitProperty, other);
            }
            //Check for airborne or knockdown state
            else if (other.groundState == GroundState.Airborne || other.groundState == GroundState.Knockdown)
            {
                ExecuteHit(attack.airHitProperty, other);
            }
        }
    }

    void ExecuteBlock(HitProperty hit, Status other)
    {
        status.Meter += hit.meterGain;
        other.Meter += hit.meterGain / 2;
        status.minusFrames = -(move.totalMoveDuration - attack.gameFrames + hit.hitstop);
        other.newMove = true;
        other.hitstopCounter = hit.hitstop;
        //Own hitstop
        status.Hitstop();
        status.newMove = true;
        status.hitstopCounter = hit.hitstop;
        //Block FX
        if(move.blockFX != null)
         Instantiate(move.blockFX, colPos.position, colPos.rotation);
        if (move.blockSFX != null)
            Instantiate(move.blockSFX, colPos.position, colPos.rotation);
        //Calculate direction
        aVector = baseKnockback * knockbackDirection * hit.pushback.z + baseKnockback * Vector3.Cross(Vector3.up, knockbackDirection) * hit.pushback.x + baseKnockback * Vector3.up * hit.pushback.y;
        other.TakeBlock(hit.damage, aVector, hit.stun + hit.hitstop, knockbackDirection);
    }

    void ExecuteHit(HitProperty hit, Status other)
    {
        status.Meter += hit.meterGain;
        other.Meter += hit.meterGain/2;

        status.minusFrames = -(move.totalMoveDuration - attack.gameFrames + hit.hitstop);
        other.newMove = true;
        other.hitstopCounter = hit.hitstop;
        //Own hitstop
        status.Hitstop();
        status.newMove = true;
        status.hitstopCounter = hit.hitstop;

        //Hit FX
        if (move.hitFX != null)
            Instantiate(move.hitFX, colPos.position, colPos.rotation);
        if (move.hitSFX != null)
            Instantiate(move.hitSFX, colPos.position, colPos.rotation);
        //Calculate direction
        aVector = baseKnockback * knockbackDirection * hit.pushback.z + baseKnockback * Vector3.Cross(Vector3.up, knockbackDirection) * hit.pushback.x + baseKnockback * Vector3.up * hit.pushback.y;
        if (hit.hitState == HitState.Knockdown)
            other.TakeKnockdown(hit.damage, aVector, hit.stun + hit.hitstop, knockbackDirection);
        else
            other.TakeHit(hit.damage, aVector, hit.stun + hit.hitstop, knockbackDirection);

        if (hit.hitState == HitState.Launch)
        {
            other.groundState = GroundState.Airborne;
        }
    }
}
