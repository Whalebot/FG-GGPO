using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    public bool destroyOnDeath;
    public float damageMultiplier;
    public float poiseMultiplier;
    Status status;
    private void Start()
    {
        status = GetComponentInParent<Status>();
        //if(destroyOnDeath)
        status.deathEvent += AutoDestroy;

    }

    private void OnDisable()
    {
        status.deathEvent -= AutoDestroy;
    }

    public void AutoDestroy() {
        Destroy(gameObject);
    }
}
