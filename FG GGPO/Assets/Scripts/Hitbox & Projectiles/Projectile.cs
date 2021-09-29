using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Hitbox
{
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

    public virtual void Movement() {
        rb.velocity = transform.forward * velocity;
    }

    new void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        Projectile proj = other.GetComponentInParent<Projectile>();
        if (proj != null && destroyOnProjectileClash) {
            print("Projectile clash");
            Destroy(gameObject);
        }

        Hitbox hitbox = other.GetComponent<Hitbox>();
        if (hitbox != null && destroyOnHitboxClash) {
            print("Hitbox clash");
            Destroy(gameObject);
        }
    }

    public override void DoDamage(Status other, float dmgMod)
    {
        if (!hit)
            base.DoDamage(other, dmgMod);
        hit = true;
   
        Destroy(gameObject);
    }
}
