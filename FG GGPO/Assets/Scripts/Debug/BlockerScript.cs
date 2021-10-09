using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockerScript : MonoBehaviour
{
    public Collider[] playerColliders;
    public Collider playerBlockerCollider;
    // Start is called before the first frame update
    void Start()
    {
        foreach (var item in playerColliders)
        {
            Physics.IgnoreCollision(item, playerBlockerCollider, true);
        }
      

    }

    private void OnCollissionExit(Collision collision)
    {
        print("Test");
        Vector3 vel = collision.collider.attachedRigidbody.velocity;
        vel.x = 0;
        vel.z = 0;
        collision.collider.attachedRigidbody.velocity = vel;
        playerBlockerCollider.attachedRigidbody.velocity = vel;
    }
}
