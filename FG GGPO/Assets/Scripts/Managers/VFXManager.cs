using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
