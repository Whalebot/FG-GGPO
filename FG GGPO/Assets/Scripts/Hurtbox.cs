using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    public MeshRenderer mr;
    public float damageMultiplier = 1;
    Status status;
    private void Start()
    {
        status = GetComponentInParent<Status>();
        //if(destroyOnDeath)

    }

    private void OnDisable()
    {
    }

    private void OnValidate()
    {
        mr = GetComponent<MeshRenderer>();
        mr.enabled = GameHandler.staticHurtboxes;
    }

    public void AutoDestroy()
    {
        Destroy(gameObject);
    }
}
