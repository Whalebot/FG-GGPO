using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockerScript : MonoBehaviour
{
    public Collider playerCollider;
    public Collider playerBlockerCollider;
    // Start is called before the first frame update
    void Start()
    {
        Physics.IgnoreCollision(playerCollider, playerBlockerCollider, true);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
