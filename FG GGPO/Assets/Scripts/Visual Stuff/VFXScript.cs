﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXScript : MonoBehaviour
{
    [HideInInspector] public int ID = 0;
    // Start is called before the first frame update
    void Start()
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();
        //if (!GameHandler.Instance.runNormally) ps.Pause();
        VFXManager.Instance.AddParticle(ps, ID);
    }

    private void OnDestroy()
    {
        //ParticleSystem ps = GetComponent<ParticleSystem>();
        //if (ps != null)
        //    VFXManager.Instance.DeleteParticle(ps);
    }
}