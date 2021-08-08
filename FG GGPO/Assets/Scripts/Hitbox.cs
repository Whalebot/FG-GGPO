using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public float baseDamage = 1;
    public float baseKnockback = 1;
    public int totalDamage;
    public AttackContainer container;
    Move move;
    Status status;
    public GameObject projectile;
    Vector3 knockbackDirection;
    Vector3 aVector;
    public Transform body;
    [SerializeField] List<Status> enemyList;
    MeshRenderer mr;
    Transform colPos;
    private void Start()
    {

    }

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
    private void OnTriggerEnter(Collider other)
    {
        if (!other.transform.IsChildOf(body))
        {
            Status enemyStatus = other.GetComponentInParent<Status>();
            if (enemyStatus != null)
                if (!enemyList.Contains(enemyStatus))
                {
                    colPos = other.gameObject.transform;
                    if (enemyStatus != null)
                    {
                        move = container.move;
                        status = container.status;


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
    void DoDamage(Status other, float dmgMod, float poiseMod)
    {
        container.attack.canGatling = true;
        totalDamage = (int)(dmgMod * (baseDamage * container.move.damage));
        int damageDealt = totalDamage;
        knockbackDirection = (new Vector3(other.transform.position.x, 0, other.transform.position.z) - new Vector3(body.position.x, 0, body.position.z)).normalized;
        aVector = baseKnockback * knockbackDirection * move.knockback;


        GameObject GO;
        if (other.blocking)
        {
            if (move.attackHeight == Move.AttackHeight.Low && other.blockState == Status.BlockState.Standing || move.attackHeight == Move.AttackHeight.Overhead && other.blockState == Status.BlockState.Crouching)
            {
                GO = Instantiate(move.hitFX, colPos.position, colPos.rotation);
                if (move.groundHitProperty == Move.HitProperty.Knockdown)
                    other.TakeKnockdown(damageDealt, aVector, (int)(move.hitStun), knockbackDirection, move.slowMotionDuration);
                else
                    other.TakeHit(damageDealt, aVector, (int)(move.hitStun), knockbackDirection, move.slowMotionDuration);
                return;
            }
            GO = Instantiate(move.blockFX, colPos.position, colPos.rotation);
            other.TakeBlock(damageDealt, aVector, (int)(move.hitStun), knockbackDirection, move.slowMotionDuration);
        }
        else
        {
            GO = Instantiate(move.hitFX, colPos.position, colPos.rotation);
            if (move.groundHitProperty == Move.HitProperty.Knockdown)
                other.TakeKnockdown(damageDealt, aVector, (int)(move.hitStun), knockbackDirection, move.slowMotionDuration);
            else
                other.TakeHit(damageDealt, aVector, (int)(move.hitStun), knockbackDirection, move.slowMotionDuration);
        }
    }
}
