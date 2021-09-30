using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayProjectile : Projectile
{
    public int delay;
    public bool ignoreY;
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
                Vector3 aimTarget = target.position;
                if (ignoreY) aimTarget.y = transform.position.y;
                else
                    aimTarget.y += 0.75F;
                transform.LookAt(aimTarget);
                col.enabled = true;
            }
        }
        if (delay <= 0)
            rb.velocity = transform.forward * velocity;
    }
}
