using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterCamera : MonoBehaviour
{
    public Transform target;
    public CameraController p1;
    public CameraController p2;
    // Start is called before the first frame update
    void Start()
    {
        GameHandler.Instance.advanceGameState += ExecuteFrame;
    }

    public void ExecuteFrame()
    {

        transform.position = target.position;
        if (!CameraManager.Instance.isRightCamera)
            transform.rotation = Quaternion.LookRotation(Vector3.Cross(p2.target.position - p1.target.position, Vector3.up));
        else
            transform.rotation = Quaternion.LookRotation(Vector3.Cross(p2.target.position - p1.target.position, Vector3.down));
    }
}
