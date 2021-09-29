using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayProjectile : Projectile
{
    public int delay;
    Transform target;
    Collider col;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    private void Start()
    {
        target = GameHandler.Instance.ReturnPlayer(status.transform);
        col.enabled = false;
    }


    public override void Movement()
    {
        if (delay > 0)
        {
            delay--;
            if (delay == 0)
            {
                transform.LookAt(target);
                col.enabled = true;
            }
        }
        if (delay <= 0)
            rb.velocity = transform.forward * velocity;
    }
}
