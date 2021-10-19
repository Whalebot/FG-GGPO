using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageScript : MonoBehaviour
{
    public static StageScript Instance { get; private set; }

    public Transform[] roundStartPosition;
    public Transform[] wall1;
    public Transform[] wall2;
    public Transform[] midScreenCloseRange;
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
