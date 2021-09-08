using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Hitbox
{
    public float velocity;
    Rigidbody rb;
    bool hit;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        rb.velocity = transform.forward * velocity;
    }

    public override void DoDamage(Status other, float dmgMod, float poiseMod)
    {
        if (!hit)
            base.DoDamage(other, dmgMod, poiseMod);
        hit = true;
   
        Destroy(gameObject);
    }
}
