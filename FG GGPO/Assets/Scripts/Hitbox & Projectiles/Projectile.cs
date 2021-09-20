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

    private void Start()
    {

    }


    private void FixedUpdate()
    {
        rb.velocity = transform.forward * velocity;
    }

    new void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        Projectile proj = other.GetComponentInParent<Projectile>();
        if (proj != null) {
            print("Projectile clash");
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
