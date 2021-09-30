using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();
        ps.Pause();
        VFXManager.Instance.AddParticle(ps);
    }
}
