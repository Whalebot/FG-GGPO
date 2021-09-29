using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayProjectile : Projectile
{
    public int delay;
    Transform target;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

    }

    private void Start()
    {
        target = GameHandler.Instance.ReturnPlayer(status.transform);
    }


    private void FixedUpdate()
    {

    }

    public override void Movement()
    {
        if (delay > 0)
        {
            delay--;
            if (delay == 0) transform.LookAt(target);
        }
        if (delay <= 0)
            rb.velocity = transform.forward * velocity;
    }

    new void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }

    public override void DoDamage(Status other, float dmgMod)
    {
        if (!hit)
            base.DoDamage(other, dmgMod);
        hit = true;

        Destroy(gameObject);
    }
}
