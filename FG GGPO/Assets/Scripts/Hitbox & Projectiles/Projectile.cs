using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Hitbox
{
    public bool destroyOnBlock;
    public bool destroyOnHit;
    bool isDestroying;
    bool delayDestroy;
    public int life;
    public float velocity;
    public bool destroyOnProjectileClash = true;
    public bool destroyOnHitboxClash = true;

    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public bool hit;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();


    }

    private void Start()
    {

    }


    private void FixedUpdate()
    {
        Movement();
    }

    public void DestroyProjectile()
    {
        if (!delayDestroy)
            GameHandler.Instance.advanceGameState += FramePassed;
        delayDestroy = true;
        hitOnce = true;
        //Hit FX
        if (move.hitFX != null)
            Instantiate(move.hitFX, colPos.position, colPos.rotation);
        else
            Instantiate(VFXManager.Instance.defaultProjectileVFX, transform.position, transform.rotation);

        if (move.hitSFX != null)
            Instantiate(move.hitSFX, colPos.position, colPos.rotation);
        else
            Instantiate(VFXManager.Instance.defaultProjectileSFX, transform.position, transform.rotation);
    }

    void FramePassed()
    {
        if (isDestroying)
            Destroy(gameObject);
        isDestroying = true;

    }

    private void OnEnable()
    {
        if (destroyOnHit)
            status.hitEvent += DestroyProjectile;
        if (destroyOnBlock)
            status.blockEvent += DestroyProjectile;
    }

    private void OnDestroy()
    {
        if (destroyOnHit)
            status.hitEvent -= DestroyProjectile;
        if (destroyOnBlock)
            status.blockEvent -= DestroyProjectile;

        GameHandler.Instance.advanceGameState -= FramePassed;
    }

    public virtual void Movement()
    {
        rb.velocity = transform.forward * velocity;
    }

    new void OnTriggerEnter(Collider other)
    {
        if (hitOnce) return;

        Projectile proj = other.GetComponentInParent<Projectile>();
        if (proj != null && destroyOnProjectileClash)
        {
            life--;

            DestroyProjectile();
        }

        Hitbox hitbox = other.GetComponent<Hitbox>();
        if (hitbox != null && destroyOnHitboxClash)
        {
            life--;
            DestroyProjectile();
        }


        Status enemyStatus = other.GetComponentInParent<Status>();
        colPos = other.gameObject.transform;
        if (enemyStatus != null && hitbox == null)
        {
            if (status == enemyStatus) return;

            if (!enemyList.Contains(enemyStatus))
            {
                if (enemyStatus.invincible) return;
                else if (enemyStatus.projectileInvul) return;
                else if (enemyStatus.linearInvul && !move.attacks[hitboxID].homing) return;

                enemyList.Add(enemyStatus);
                DoDamage(enemyStatus, 1);
            }
        }

    }

    public override void DoDamage(Status other, float dmgMod)
    {
        if (!hit)
            base.DoDamage(other, dmgMod);
        hit = true;
        DestroyProjectile();
    }
}
