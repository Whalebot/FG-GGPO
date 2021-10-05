using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Hitbox
{
    public GameObject explosionVFX;
    public GameObject explosionSFX;

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
        body = transform;

    }

    private void Start()
    {
        if (destroyOnHit)
            status.hitEvent += DestroyProjectile;
        if (destroyOnBlock)
            status.blockEvent += DestroyProjectile;
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
        //if (explosionVFX != null)
        //    Instantiate(explosionVFX, transform.position, transform.rotation);
        //else
        //    Instantiate(VFXManager.Instance.defaultProjectileVFX, transform.position, transform.rotation);

        //if (explosionSFX != null)
        //    Instantiate(explosionSFX, transform.position, transform.rotation);
        //else
        //    Instantiate(VFXManager.Instance.defaultProjectileSFX, transform.position, transform.rotation);
        //Hit FX
        if (explosionVFX != null)
            Instantiate(explosionVFX, transform.position, transform.rotation);
        else
            Instantiate(VFXManager.Instance.defaultProjectileVFX, transform.position, transform.rotation);

        if (explosionSFX != null)
            Instantiate(explosionSFX, transform.position, transform.rotation);
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
        colPos = other.gameObject.transform;
        Projectile proj = other.GetComponentInParent<Projectile>();
        if (proj != null && destroyOnProjectileClash && proj.status != status)
        {
            life--;

            DestroyProjectile();
        }

        Hitbox hitbox = other.GetComponent<Hitbox>();
        if (hitbox != null && destroyOnHitboxClash && hitbox.status != status)
        {
            life--;
            DestroyProjectile();
        }


        Status enemyStatus = other.GetComponentInParent<Status>();

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
