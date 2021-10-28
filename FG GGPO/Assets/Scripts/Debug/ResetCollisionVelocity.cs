using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetCollisionVelocity : MonoBehaviour
{
    private void OnCollisionExit(Collision collision)
    {


        //if (collision.rigidbody != null)
        //{
        //    Status status = collision.gameObject.GetComponent<Status>();
        //    AttackScript attack = collision.gameObject.GetComponent<AttackScript>();
        //  //  print("1");
        //    if (status.HitStun <= 0 && !attack.inMomentum && status.currentState != Status.State.Startup)
        //    {
        //     //   print("2");
        //        Vector3 vel = collision.collider.attachedRigidbody.velocity;
        //        vel.x = 0;
        //        vel.z = 0;
        //        collision.rigidbody.velocity = vel;
        //    }
        //}
    }

    private void OnCollisionEnter(Collision collision)
    {

        ////print(collision.gameObject);
        //if (collision.rigidbody != null)
        //{
        //    Status status = collision.gameObject.GetComponent<Status>();
        //    AttackScript attack = collision.gameObject.GetComponent<AttackScript>();
        // //   print("1");
        //    if (status.HitStun <= 0 && !attack.inMomentum && status.currentState != Status.State.Startup)
        //    {
        //   //     print("2");
        //        Vector3 vel = collision.collider.attachedRigidbody.velocity;
        //        vel.x = 0;
        //        vel.z = 0;
        //        collision.rigidbody.velocity = vel;
        //    }
        //}
    }
}
