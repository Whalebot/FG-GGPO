using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public Transform lookTarget;
    public float lerpValue;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, target.position, lerpValue);
        transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(lookTarget.position - transform.position), lerpValue); 
    }

    [Button]
    private void SetPosition()
    {
        transform.position = target.position;
        transform.rotation = Quaternion.LookRotation(lookTarget.position - transform.position);
    }
}
