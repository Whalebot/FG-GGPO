using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public Transform lookTarget;
    public float lerpValue;
    public float tempLerp;
    public float tempLerp2;
    public float rotationLerp;
    public bool frontCamera;
    public Transform main;
    public Vector3 offset;
    Vector3 yVector;
    // Start is called before the first frame update
    void Start()
    {
        GameHandler.Instance.advanceGameState += ExecuteFrame;
    }

    // Update is called once per frame
    void ExecuteFrame()
    {


        yVector = target.position;
        yVector = yVector + main.forward * offset.z;
        yVector.y = (lookTarget.position.y + target.position.y) / 2;
        tempLerp = (CameraManager.Instance.cameraAngle / 90) * lerpValue;        
        tempLerp2 = (CameraManager.Instance.cameraAngle / 90) * rotationLerp;
        transform.position = Vector3.Lerp(transform.position, yVector, tempLerp);

        if (CameraManager.Instance.updateCameras && !CameraManager.Instance.groundCrossup)
        {
            Vector3 v1 = lookTarget.position;
            v1.y = (lookTarget.position.y + target.position.y) / 2;

            if (v1 - yVector != Vector3.zero)
                transform.rotation =
                Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(v1 - yVector), tempLerp2);
        }
    }

    [Button]
    private void SetPosition()
    {
        transform.position = target.position;
        transform.rotation = Quaternion.LookRotation(lookTarget.position - transform.position);
    }
}
