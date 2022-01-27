using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockerScript : MonoBehaviour
{
    public Collider[] playerColliders;
    public Collider playerBlockerCollider;
    public Rigidbody rb;
    public Movement mov;
    public Movement enemyMov;
    public Vector3 ownVelocity;
    public Vector3 enemyVelocity;
    public Vector3 pushVelocity;

    public MeshRenderer mr;
    // Start is called before the first frame update
    void Awake()
    {
        mr = GetComponent<MeshRenderer>();
        foreach (var item in playerColliders)
        {
            Physics.IgnoreCollision(item, playerBlockerCollider, true);
        }
     
    }

    private void Start()
    {
        enemyMov = GameHandler.Instance.ReturnPlayer(mov.transform).gameObject.GetComponent<Movement>();
        GameHandler.Instance.advanceGameState += ExecuteFrame;
        mr.enabled = GameHandler.Instance.showColliders;
    }

    void ExecuteFrame() {
        mr.enabled = GameHandler.Instance.showColliders;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody != null && enemyMov != null)
        {
            enemyVelocity = new Vector3((enemyMov.storedDirection.normalized * enemyMov.actualVelocity).x, rb.velocity.y, (enemyMov.storedDirection.normalized * enemyMov.actualVelocity).z);
            ownVelocity = new Vector3((mov.storedDirection.normalized * mov.actualVelocity).x, rb.velocity.y, (mov.storedDirection.normalized * mov.actualVelocity).z);
            if (enemyVelocity.magnitude < ownVelocity.magnitude)
            {
             
                ownVelocity.y = 0;
                pushVelocity = ownVelocity * 0.5F; 
                collision.gameObject.GetComponent<Status>().pushVelocity = pushVelocity;
            }
     
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
