using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXScript : MonoBehaviour
{
    [HideInInspector] public int ID = 0;
    // Start is called before the first frame update
    void Start()
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();
        ps.Pause();
        VFXManager.Instance.AddParticle(ps, ID);
    }
}
