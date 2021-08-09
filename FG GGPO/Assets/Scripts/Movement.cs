using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;
public class Movement : MonoBehaviour
{
    [HideInInspector] public Rigidbody rb;
    public bool isMoving = false;
    public bool strafe;
    public Transform strafeTarget;

    public bool crouching;
    public bool holdBack;
    [HeaderAttribute("Movement attributes")]
    [TabGroup("Movement")] public bool forwardOnly = true;
    [TabGroup("Movement")] public float walkSpeed = 3;
    [TabGroup("Movement")] public float backWalkSpeed = 2;

    [HideInInspector] public bool run;

    [TabGroup("Movement")] public float currentVel;
    public float actualVelocity;
    [TabGroup("Movement")] public float smoothAcceleration = 0.5f;
    [TabGroup("Movement")] public float smoothDeacceleration = 0.5f;
    [TabGroup("Movement")] public float walkThreshold;

    [TabGroup("Rotation")]
    [HeaderAttribute("Rotation attributes")]
    [TabGroup("Rotation")] public bool smoothRotation = true;
    [TabGroup("Rotation")] public float rotationDamp = 8;
    [TabGroup("Rotation")] public float sharpRotationDamp = 16;
    [TabGroup("Rotation")] public float deltaAngle;

    [HeaderAttribute("Jump attributes")]
    [FoldoutGroup("Jump")] public bool ground;
    [FoldoutGroup("Jump")] public float rayLength;
    [FoldoutGroup("Jump")] public float stepAngle;
    RaycastHit hit, hit2;
    [FoldoutGroup("Jump")] public float offset;
    [FoldoutGroup("Jump")] public LayerMask groundMask;
    [FoldoutGroup("Jump")] public float jumpHeight;
    [FoldoutGroup("Jump")] public float fallMultiplier;
    [FoldoutGroup("Jump")] public int minimumJumpTime = 2;
    [FoldoutGroup("Jump")] int jumpCounter;
    [FoldoutGroup("Jump")] public float airRotation = 4;

    [HeaderAttribute("Sprint attributes")]
    public bool sprinting;
    public float sprintSpeed = 12;

    public delegate void MovementEvent();
    public MovementEvent jumpEvent;
    public MovementEvent LandEvent;
    public MovementEvent strafeSet;
    public MovementEvent strafeBreak;

    [HideInInspector] public float zeroFloat;
    public Vector3 direction;
    public Vector3 storedDirection;

    [FoldoutGroup("Assign components")] Status status;
    [FoldoutGroup("Assign components")] public Collider hurtbox;
    [FoldoutGroup("Assign components")] public Collider col;
    [FoldoutGroup("Assign components")] public PhysicMaterial groundMat;
    [FoldoutGroup("Assign components")] public PhysicMaterial airMat;
    bool check;
    bool check2;

    // Start is called before the first frame update
    void Start()
    {
        status = GetComponent<Status>();
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }


    public void SetStrafeTarget(Transform t)
    {
        strafeTarget = t;
        strafe = true;
        strafeSet?.Invoke();
    }

    public void ResetStrafe()
    {
        strafeBreak?.Invoke();
        strafeTarget = null;
        strafe = false;
    }

    void DisableMovement()
    {
        rb.velocity = Vector3.zero;
        direction = Vector3.zero;
        rb.isKinematic = true;
        hurtbox.gameObject.SetActive(false);
        return;
    }

    private void FixedUpdate()
    {
        if (GameHandler.isPaused)
        {
            isMoving = false;
            return;
        }
        if (status.currentState == Status.State.Neutral)
        {

            MovementProperties();
            Rotation();
            PlayerMovement();
            if (!GameHandler.Instance.disableBlock)
                status.blocking = 90 < Vector3.Angle(strafeTarget.position - transform.position, direction);
        }

        if (rb.velocity.y < 0) rb.velocity += Physics.gravity * fallMultiplier;

        if (direction != Vector3.zero)
        {
            isMoving = true;
        }
        else { isMoving = false; }

        GroundDetection();
    }

    public Vector3 RelativeToForward()
    {
        float angle = Vector3.SignedAngle(transform.forward, direction, Vector3.up);
        Vector3 temp = Quaternion.Euler(0, angle, 0) * Vector3.forward;
        if (!isMoving) temp = Vector3.zero;
        return temp;
    }


    public void RotateInPlace(Vector3 dir)
    {
        deltaAngle = Vector3.SignedAngle(transform.forward, dir, Vector3.up);
        Quaternion desiredRotation = Quaternion.Euler(0, Vector3.SignedAngle(Vector3.forward, new Vector3(dir.x, 0, dir.z), Vector3.up), 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * rotationDamp);
    }



    public virtual void Rotation()
    {
        if (strafe)
        {
            if (strafeTarget == null) return;
            Vector3 desiredDirection = strafeTarget.position - transform.position;
            Quaternion desiredRotation = Quaternion.Euler(0, Vector3.SignedAngle(Vector3.forward, new Vector3(desiredDirection.x, 0, desiredDirection.z), Vector3.up), 0);
            deltaAngle = Vector3.SignedAngle(transform.forward, direction, Vector3.up);

            if (Mathf.Abs(deltaAngle) < 90)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * rotationDamp);

            }
            else
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * sharpRotationDamp);
            }

            return;
        }

        if (direction != Vector3.zero)
        {
            //Desired rotation, updated every (fixed) frame
            Quaternion desiredRotation = Quaternion.Euler(0, Vector3.SignedAngle(Vector3.forward, new Vector3(direction.x, 0, direction.z), Vector3.up), 0);
            deltaAngle = Vector3.SignedAngle(transform.forward, direction, Vector3.up);
            if (ground)
            {
                if (Mathf.Abs(deltaAngle) < 90)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * rotationDamp);

                }
                else
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * sharpRotationDamp);
                }
            }
            else transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * airRotation);
        }
    }



    public virtual void MovementProperties()
    {
        if (!GameHandler.Instance.disableBlock)
            holdBack = 90 < Vector3.Angle(strafeTarget.position - transform.position, direction);

        if (ground)
        {
            if (!isMoving || crouching)
            {
                currentVel = 0;
                actualVelocity = currentVel;
            }
            else if (isMoving)
            {
                if (90 < Vector3.Angle(strafeTarget.position - transform.position, direction))
                {
                    currentVel = backWalkSpeed;
                }
                else if (sprinting)
                {
                    currentVel = sprintSpeed;
                }
                else
                {
                    currentVel = walkSpeed;
                }
                actualVelocity = currentVel;
            }
        }
        else actualVelocity = Speed();
    }

    float Speed()
    {
        float f = 0;

        if (90 < Vector3.Angle(strafeTarget.position - transform.position, direction))
        {
            f = backWalkSpeed;
        }
        else if(sprinting)
        {
            f = sprintSpeed;
        }
        else f = walkSpeed;

        return f;
    }

    public void Jump()
    {
        jumpCounter = minimumJumpTime;
        col.material = airMat;
        ground = false;
        status.groundState = Status.GroundState.Airborne;
        jumpEvent?.Invoke();
        Vector3 temp = direction.normalized;
        rb.velocity = new Vector3(temp.x * Speed(), jumpHeight, temp.z * Speed());
        // rb.velocity = new Vector3(rb.velocity.x, jumpHeight, rb.velocity.z);
    }

    public void JumpFX()
    {
        col.material = airMat;
        ground = false;

        Vector3 temp = direction.normalized;
        rb.velocity = new Vector3(temp.x * Speed(), rb.velocity.y, temp.z * Speed());

        rb.velocity = new Vector3(rb.velocity.x, jumpHeight, rb.velocity.z);
    }

    public bool GroundDetection()
    {
        check = Physics.Raycast(transform.position + Vector3.up * 0.1F, Vector3.down, out hit, rayLength, groundMask);
        check2 = Physics.Raycast(transform.position + Vector3.up * 0.1F + transform.forward * offset, Vector3.down, out hit2, rayLength, groundMask);

        if (jumpCounter > 0)
        {

            jumpCounter--;

            return false;
        }

        if (check)
        {

            Debug.DrawRay(transform.position + Vector3.up * 0.1F, Vector3.down * rayLength, Color.yellow);
            Debug.DrawRay(hit.point, hit.normal * 2f, Color.blue);
            float angle = Vector3.Angle(hit2.normal, Vector3.up);
            //Debug.Log("angle " + angle);

            if (angle > stepAngle) check = false;
        }
        if (check2)
        {
            Debug.DrawRay(transform.position + Vector3.up * 0.1F + transform.forward * offset, Vector3.down * rayLength, Color.yellow);
            Debug.DrawRay(hit2.point, hit2.normal * 2f, Color.blue);

            float angle2 = Vector3.Angle(hit2.normal, Vector3.up);
            //Debug.Log("angle 2 " + angle2);

            if (angle2 > stepAngle) check2 = false;
        }



        if (!ground && check || !ground && check2)
        {
            LandEvent?.Invoke();

        }
        ground = check || check2;

        if (ground) col.material = groundMat;
        else col.material = airMat;

        return ground;
    }

    public void PlayerMovement()
    {
        //if (!ground)
        //{
        //    Vector3 temp = rb.velocity;
        //    temp.y = 0;
        //    rb.velocity = transform.forward * temp.magnitude + rb.velocity.y * Vector3.up;


        //    // rb.AddForce(new Vector3(transform.forward.x * actualVelocity * Time.deltaTime, 0, transform.forward.z * actualVelocity * Time.deltaTime), ForceMode.VelocityChange);
        //}
        //else if (forwardOnly || sprinting)
        //    rb.velocity = new Vector3(transform.forward.x * actualVelocity, rb.velocity.y, transform.forward.z * actualVelocity);

        //else
        //{

        if (ground)
            storedDirection = direction.normalized;

        rb.velocity = new Vector3((storedDirection.normalized * actualVelocity).x, rb.velocity.y, (storedDirection.normalized * actualVelocity).z);
    }

    public Vector3 RemoveAxis(Vector3 vec, Vector3 removedAxis)
    {
        Vector3 n = removedAxis;
        Vector3 dir = vec;

        float d = Vector3.Dot(dir, n);


        return n * d;
    }

    public Vector3 RemoveYAxis(Vector3 vec)
    {
        Vector3 n = Vector3.down;

        Vector3 dir = vec;
        float d = Vector3.Dot(dir, n);
        dir -= n * d;
        return dir;
    }
}
