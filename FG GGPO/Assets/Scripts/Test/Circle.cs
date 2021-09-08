using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Circle : MonoBehaviour
{
    public float val;
    public float speed;


    public Transform target;
    public float angle = 0;
    public float distance;
    public float measuredDistance;

    float radius = 5;
    Rigidbody rb;
    public Vector3 pos;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        //angle += speed * Time.deltaTime; //if you want to switch direction, use -= instead of +=

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        
        if (Keyboard.current.spaceKey.isPressed)
        {
            rb.velocity = (TravelRight(speed));
            Debug.DrawLine(transform.position, pos);
        }
        else rb.velocity = Vector3.zero;
    }


    Vector3 TravelRight(float f)
    {

        measuredDistance = Vector3.Distance(target.position, transform.position);
        pos = transform.position;


        transform.LookAt(target);

        angle = Vector3.SignedAngle(transform.right, Vector3.forward, Vector3.up);
        angle += val * distance;
        pos.x = Mathf.Cos(angle * Mathf.Deg2Rad) * distance + target.position.x;
        pos.z = Mathf.Sin(angle * Mathf.Deg2Rad) * distance + target.position.z;
        return (pos - transform.position).normalized * speed;
    }

    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(pos, 1);
    }
}
