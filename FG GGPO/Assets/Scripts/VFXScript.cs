using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        VFXManager.Instance.AddParticle(GetComponent<ParticleSystem>());
    }
}
