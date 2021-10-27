using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockerScript : MonoBehaviour
{
    public Collider[] playerColliders;
    public Collider playerBlockerCollider;
    public Rigidbody rb;
    public Movement mov;
    public Vector3 ownVelocity;
    public Vector3 pushVelocity;
    // Start is called before the first frame update
    void Awake()
    {
        foreach (var item in playerColliders)
        {
            Physics.IgnoreCollision(item, playerBlockerCollider, true);
        }
      

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody != null)
        {
            ownVelocity = new Vector3((mov.storedDirection.normalized * mov.actualVelocity).x, rb.velocity.y, (mov.storedDirection.normalized * mov.actualVelocity).z);
            ownVelocity.y = 0;
            pushVelocity = ownVelocity * 0.5F;
            collision.gameObject.GetComponent<Status>().pushVelocity = pushVelocity;
        }
    }
    //private void OnCollisionExit(Collision collision)
    //{
    //    if (collision.rigidbody != null)
    //    {
    //        pushVelocity = Vector3.zero;
    //        collision.gameObject.GetComponent<Status>().pushVelocity = pushVelocity;
    //    }
    //}
}
