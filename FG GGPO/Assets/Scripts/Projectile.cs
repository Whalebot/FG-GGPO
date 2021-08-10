using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Hitbox
{
    public float velocity;
    Rigidbody rb;

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
        base.DoDamage(other, dmgMod, poiseMod);
        Destroy(gameObject);
    }
}
