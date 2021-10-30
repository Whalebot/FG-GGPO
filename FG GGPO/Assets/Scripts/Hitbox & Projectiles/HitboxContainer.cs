using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HitboxContainer : MonoBehaviour
{
    public AttackContainer[] containers;
    public GameObject[] particles;
    // Start is called before the first frame update
    void Start()
    {
        DeactivateHitboxes();
        containers = GetComponentsInChildren<AttackContainer>();
    }

    void GetAllHitboxes() {
        containers = GetComponentsInChildren<AttackContainer>();
    }

    //private void Update()
    //{
    //    containers = GetComponentsInChildren<AttackContainer>();
    //    print(containers);
    //}

    public void ActivateHitbox(int i)
    {
        GetAllHitboxes();
        
        if (containers.Length > 0)
        {
            foreach (AttackContainer container in containers)
            {
                container.ActivateHitbox();
            }
        }
    }

    public void DeactivateHitboxes()
    {
        GetAllHitboxes();
        foreach (AttackContainer container in containers)
        {
            container.DeactivateHitbox();
        }
    }

    public void InterruptAttack()
    {
        GetAllHitboxes();
        foreach (AttackContainer container in containers)
        {
            container.InterruptAttack();
        }
    }

    public void ActivateParticle(int i)
    {

        DeactivateParticles();
        if (particles.Length > 0)
            if (i < particles.Length)
            {
                ParticleSystem[] ps = particles[i].GetComponentsInChildren<ParticleSystem>();

                foreach (ParticleSystem thisPS in ps)
                {
                   thisPS.Play();
                }

            }
            else
            {
                particles[0].SetActive(true);

                ParticleSystem[] ps = particles[0].GetComponentsInChildren<ParticleSystem>();

                foreach (ParticleSystem thisPS in ps)
                {
                    thisPS.Play();
                }
            }
    }


    public void DeactivateParticles()
    {

        foreach (GameObject particle in particles)
        {
            for (int i = 0; i < particles.Length; i++)
            {
                ParticleSystem[] ps = particles[i].GetComponentsInChildren<ParticleSystem>();

                foreach (ParticleSystem thisPS in ps)
                {
                    thisPS.Stop();
                }
            }
        }
    }
}
