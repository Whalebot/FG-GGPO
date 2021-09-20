using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance
    {
        get; private set;
    }
    public GameObject defaultHitVFX;
    public GameObject defaultBlockVFX;
    public GameObject defaultHitSFX;
    public GameObject defaultBlockSFX;

    public GameObject recoveryFX;
    public GameObject wakeupFX;

    public List<ParticleSystem> particles;
    public int frame;
    public float time;
    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    [Button]
    public void AdvanceParticles() {

        frame++;
        foreach (var item in particles)
        {
            // time = item.time;
            item.Play();
            time = frame * Time.fixedDeltaTime;
            item.Simulate(time, true, true, true);
        }
    }
    [Button]
    public void RevertParticles()
    {
        frame--;
        foreach (var item in particles)
        {
            // time = item.time;
            item.Play();
            time = frame * Time.fixedDeltaTime;
            item.Simulate(time, true, true, true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var item in particles)
        {
       //     time = item.time;
        }
    }
}
