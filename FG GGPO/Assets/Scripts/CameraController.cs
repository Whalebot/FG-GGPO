using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public Transform lookTarget;
    public float lerpValue;
    public float rotationLerp;
    public bool ignoreY;
    public bool averageHeight;
    public bool frontCamera;
    public Transform main;
    public Vector3 offset;
    Vector3 yVector;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!ignoreY)
        {
            transform.position = Vector3.Lerp(transform.position, target.position, lerpValue);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookTarget.position - transform.position), rotationLerp);
        }
        else if (averageHeight)
        {
            yVector = target.position;
            yVector = yVector + main.forward * offset.z;
            yVector.y = (lookTarget.position.y + target.position.y) / 2;

            transform.position = Vector3.Lerp(transform.position, yVector, lerpValue);
            transform.rotation =
              //   Quaternion.LookRotation(lookTarget.position - transform.position);
            Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookTarget.position - transform.position), rotationLerp);
        }
        else
        {
            Vector3 ignoreY = target.position;
            ignoreY.y = 0;
            transform.position = Vector3.Lerp(transform.position, ignoreY, lerpValue);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookTarget.position - transform.position), rotationLerp);
        }
    }

    [Button]
    private void SetPosition()
    {
        transform.position = target.position;
        transform.rotation = Quaternion.LookRotation(lookTarget.position - transform.position);
    }

    private void OnDrawGizmos()
    {
        if (frontCamera)
            Gizmos.color = Color.blue;
        else Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 1);
        Gizmos.DrawSphere(yVector, 0.5F);
    }
}
