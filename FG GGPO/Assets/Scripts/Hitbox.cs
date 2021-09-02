using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public float baseDamage = 1;
    public float baseKnockback = 1;
    public int totalDamage;
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


    private void Awake()
    {

        mr = GetComponent<MeshRenderer>();
        move = container.move;
        status = container.status;

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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.transform.IsChildOf(body))
        {
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
                        DoDamage(enemyStatus, hurtbox.damageMultiplier, hurtbox.poiseMultiplier);
                    }
                    else
                        DoDamage(enemyStatus, 1, 1);
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
    public virtual void DoDamage(Status other, float dmgMod, float poiseMod)
    {
        if (container != null)
        {
            attack = container.attack;

        }
        attack.canGatling = move.canGatling;
        {
            status.minusFrames = -(move.startupFrames + move.activeFrames + move.recoveryFrames - attack.gameFrames);
            //status.hit
        }

        totalDamage = (int)(dmgMod * (baseDamage * move.damage));
        int damageDealt = totalDamage;
        knockbackDirection = (new Vector3(other.transform.position.x, 0, other.transform.position.z) - new Vector3(body.position.x, 0, body.position.z)).normalized;

        GameObject GO;
        if (other.blocking)
        {
            if (move.attackHeight == AttackHeight.Low && other.blockState == BlockState.Standing || move.attackHeight == AttackHeight.Overhead && other.blockState == BlockState.Crouching)
            {
                aVector = baseKnockback * knockbackDirection * move.hitPushback.z + baseKnockback * Vector3.Cross(Vector3.up, knockbackDirection) * move.hitPushback.x + baseKnockback * Vector3.up * move.hitPushback.y;

                //CameraManager.Instance.ShakeCamera(move.shakeMagnitude, move.shakeDuration);
                GO = Instantiate(move.hitFX, colPos.position, colPos.rotation);
                if (move.groundHitProperty.hitState == HitState.Knockdown)
                    other.TakeKnockdown(damageDealt, aVector, (int)(move.hitStun), knockbackDirection, 0);
                else
                    other.TakeHit(damageDealt, aVector, (int)(move.hitStun), knockbackDirection, 0);
                return;
            }
            aVector = baseKnockback * knockbackDirection * move.blockPushback.z + baseKnockback * Vector3.Cross(Vector3.up, knockbackDirection) * move.blockPushback.x + baseKnockback * Vector3.up * move.blockPushback.y;


            GO = Instantiate(move.blockFX, colPos.position, colPos.rotation);
            other.TakeBlock(damageDealt, aVector, (int)(move.blockStun), knockbackDirection, 0);
        }
        else
        {

            //CameraManager.Instance.ShakeCamera(move.shakeMagnitude, move.shakeDuration);
            GO = Instantiate(move.hitFX, colPos.position, colPos.rotation);
            if (other.groundState == GroundState.Grounded)
            {
                aVector = baseKnockback * knockbackDirection * move.hitPushback.z + baseKnockback * Vector3.Cross(Vector3.up, knockbackDirection) * move.hitPushback.x + baseKnockback * Vector3.up * move.hitPushback.y;
                if (move.groundHitProperty.hitState == HitState.Launch)
                {
                    other.groundState = GroundState.Airborne;
                }

                if (move.groundHitProperty.hitState == HitState.Knockdown)
                    other.TakeKnockdown(damageDealt, aVector, (int)(move.hitStun), knockbackDirection, 0);
                else
                    other.TakeHit(damageDealt, aVector, (int)(move.hitStun), knockbackDirection,0);
            }
            else if (other.groundState == GroundState.Airborne || other.groundState == GroundState.Knockdown)
            {
                aVector = baseKnockback * knockbackDirection * move.airHitPushback.z + baseKnockback * Vector3.Cross(Vector3.up, knockbackDirection) * move.airHitPushback.x + baseKnockback * Vector3.up * move.airHitPushback.y;

                if (move.groundHitProperty.hitState == HitState.Knockdown)
                    other.TakeKnockdown(damageDealt, aVector, (int)(move.hitStun), knockbackDirection,0);
                else
                    other.TakeHit(damageDealt, aVector, (int)(move.hitStun), knockbackDirection, 0);
            }

        }
    }
}
