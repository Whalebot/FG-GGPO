using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameState 
{
    public int p1Health;
    public int p2Health;

    public GameState(Vector3 v1, Vector3 v2, Quaternion q1, Quaternion q2) {
        p1Position = v1;
        p2Position = v2;
        p1Rotation = q1;
        p2Rotation = q2;

    }
    public Vector3 p1Position;
    public Vector3 p2Position;

    public Quaternion p1Rotation;
    public Quaternion p2Rotation;

    public Vector3 p1Velocity;
    public Vector3 p2Velocity;
}
