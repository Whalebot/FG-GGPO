using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowcaseSpin : MonoBehaviour
{

    public bool spin;
    public float turnSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        if (spin)
        {
            transform.Rotate(0, turnSpeed, 0, Space.World);
        }
    }
}
