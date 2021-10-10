using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetCollisionVelocity : MonoBehaviour
{
    private void OnCollisionExit(Collision collision)
    {


        if (collision.rigidbody != null)
        {
            Status status = collision.gameObject.GetComponent<Status>();
            AttackScript attack = collision.gameObject.GetComponent<AttackScript>();
            if (status.HitStun <= 0 && !attack.inMomentum)
            {
                Vector3 vel = collision.collider.attachedRigidbody.velocity;
                vel.x = 0;
                vel.z = 0;
                collision.rigidbody.velocity = vel;
            }
        }
    }
}
