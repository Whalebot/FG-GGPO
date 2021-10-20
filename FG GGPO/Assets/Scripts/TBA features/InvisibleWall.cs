using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleWall : MonoBehaviour
{
    public float threshold;
    public float distance;
    // Start is called before the first frame update
    void Start()
    {
        GameHandler.Instance.advanceGameState += ExecuteFrame;
    }

    // Update is called once per frame
    void ExecuteFrame()
    {
        distance = Vector3.Distance(GameHandler.Instance.p1Transform.position, GameHandler.Instance.p2Transform.position);

        if (distance < threshold)
        {
            transform.position = (GameHandler.Instance.p1Transform.position + GameHandler.Instance.p2Transform.position) / 2;

        }

        Vector3 v = GameHandler.Instance.p2Transform.position - GameHandler.Instance.p1Transform.position;
        v.y = transform.position.y;
        transform.rotation = Quaternion.LookRotation(v, Vector3.up);
    }
}
