using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;
public class Movement : MonoBehaviour
{
    [HideInInspector] public Rigidbody rb;
    public bool isMoving = false;
    public Transform strafeTarget;

    public bool crouching;
    public bool holdBack;
    [HeaderAttribute("Movement attributes")]
    [TabGroup("Movement")] public bool forwardOnly = true;
    [TabGroup("Movement")] public float walkSpeed = 3;
    [TabGroup("Movement")] public float sideWalkSpeed = 3;
    [TabGroup("Movement")] public float backWalkSpeed = 2;
    [TabGroup("Movement")]
    [HeaderAttribute("Sprint attributes")]
    public bool sprinting;
    [TabGroup("Movement")] public float sprintSpeed = 8;
    [TabGroup("Movement")] public int runMomentumDuration;
    [TabGroup("Movement")] public int runMomentumCounter;
    [TabGroup("Movement")] public Vector3 runDirection;


    [TabGroup("Movement")] public float currentVel;
    public float actualVelocity;
    [TabGroup("Rotation")]
    [HeaderAttribute("Rotation attributes")]
    [TabGroup("Rotation")] public bool smoothRotation = true;
    [TabGroup("Rotation")] public float rotationDamp = 8;
    [TabGroup("Rotation")] public float sharpRotationDamp = 16;
    [TabGroup("Rotation")] public float deltaAngle;


    [TabGroup("Jump")] [HeaderAttribute("Jump attributes")] public int multiJumps;
    [TabGroup("Jump")] public float jumpVelocity;
    [TabGroup("Jump")] public int performedJumps;
    [TabGroup("Jump")] public bool ground;
    [TabGroup("Jump")] public float rayLength;
    RaycastHit hit;
    [TabGroup("Jump")] public float offset;
    [TabGroup("Jump")] public LayerMask groundMask;
    [TabGroup("Jump")] public float[] jumpHeight;
    [TabGroup("Jump")] public float fallMultiplier;
    [TabGroup("Jump")] public int minimumJumpTime = 2;
    [TabGroup("Jump")] int jumpCounter;



    public delegate void MovementEvent();
    public MovementEvent jumpEvent;
    public MovementEvent landEvent;

    [HideInInspector] public float zeroFloat;
    public Vector3 direction;
    public Vector3 storedDirection;

    [FoldoutGroup("Assign components")] Status status;
    [FoldoutGroup("Assign components")] public Collider hurtbox;
    [FoldoutGroup("Assign components")] public Collider col;
    [FoldoutGroup("Assign components")] public PhysicMaterial groundMat;
    [FoldoutGroup("Assign components")] public PhysicMaterial airMat;
    bool check;
    Vector3 pos;
    // Start is called before the first frame update
    void Start()
    {
        strafeTarget = GameHandler.Instance.ReturnPlayer(transform);
        status = GetComponent<Status>();
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
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
        //CalculateRight(1);
        if (GameHandler.isPaused)
        {
            isMoving = false;
            return;
        }

        ExecuteFrame();
    }
    public Vector3 CalculateRight(float f)
    {


        Vector3 targetNoY = strafeTarget.position;
        targetNoY.y = transform.position.y;
        float distance = Vector3.Distance(targetNoY, transform.position);

        transform.LookAt(targetNoY);
        float angle = Vector3.SignedAngle(transform.right, Vector3.forward, Vector3.up);

        angle += Mathf.Sign(f) * distance;


        pos.x = Mathf.Cos(angle * Mathf.Deg2Rad) * distance + targetNoY.x;
        pos.z = Mathf.Sin(angle * Mathf.Deg2Rad) * distance + targetNoY.z;
        Debug.DrawLine(transform.position, pos, Color.red);

        return (pos - transform.position).normalized * Mathf.Abs(f);
    }


    public void ExecuteFrame()
    {



        if (ground && runMomentumCounter == 0 || ground && sprinting)
            storedDirection = direction.normalized;
        if (status.currentState == Status.State.Neutral)
        {
            MovementProperties();
            Rotation();
            PlayerMovement();
        }

        if (rb.velocity.y < 0 && status.groundState == GroundState.Airborne) rb.velocity += Physics.gravity * fallMultiplier;

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
        if (ground && isMoving)
        {
            if (strafeTarget == null) return;
            Vector3 desiredDirection = strafeTarget.position - transform.position;
            Quaternion desiredRotation = Quaternion.Euler(0, Vector3.SignedAngle(Vector3.forward, new Vector3(desiredDirection.x, 0, desiredDirection.z), Vector3.up), 0);
            deltaAngle = Vector3.SignedAngle(transform.forward, direction, Vector3.up);

            if (Mathf.Abs(deltaAngle) < 90)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.fixedDeltaTime * rotationDamp);

            }
            else
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.fixedDeltaTime * sharpRotationDamp);
            }

            return;
        }
    }

    public void ResetRun()
    {
        runMomentumCounter = 0;
    }

    public virtual void MovementProperties()
    {
        if (!GameHandler.Instance.disableBlock)
        {
            holdBack = 90 < Vector3.Angle(strafeTarget.position - transform.position, direction);
        }

        if (ground)
        {
            if (!isMoving || crouching)
            {
                currentVel = 0;
                actualVelocity = currentVel;
            }
            else if (isMoving)
            {
                if (95 < Vector3.Angle(strafeTarget.position - transform.position, direction))
                {
                    currentVel = backWalkSpeed;
                }
                else if (50 < Vector3.Angle(strafeTarget.position - transform.position, direction))
                {
                    currentVel = sideWalkSpeed;
                }
                else if (sprinting)
                {
                    runMomentumCounter = runMomentumDuration;
                    runDirection = direction.normalized;
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
        float f = jumpVelocity;

        return f;
    }

    public void Jump()
    {
        if (performedJumps > multiJumps) return;
        jumpEvent?.Invoke();
        storedDirection = direction.normalized * jumpVelocity;
        jumpCounter = minimumJumpTime;
        col.material = airMat;
        ground = false;
        status.groundState = GroundState.Airborne;

        if (status.currentState == Status.State.Active || status.currentState == Status.State.Recovery)
        {
            status.minusFrames = 0;
            status.frameDataEvent?.Invoke();
        }

        Vector3 temp = direction.normalized;
        actualVelocity = Speed();
        rb.velocity = new Vector3(temp.x * Speed(), jumpHeight[performedJumps], temp.z * Speed()) + runDirection * walkSpeed;
        performedJumps++;
    }

    public bool GroundDetection()
    {
        check = Physics.Raycast(transform.position + Vector3.up * 0.1F, Vector3.down, out hit, rayLength, groundMask);

        if (jumpCounter > 0)
        {

            jumpCounter--;

            return false;
        }

        if (!ground)
        {
            if (check && rb.velocity.y < 0) rb.velocity = new Vector3(-transform.forward.x, rb.velocity.y, -transform.forward.z);

            status.DisableCollider();
        }

        if (!ground && transform.position.y < 0.1F)
        {
            if (status.currentState == Status.State.Active || status.currentState == Status.State.Recovery)
            {
                status.minusFrames = 0;
                status.frameDataEvent?.Invoke();
            }
            landEvent?.Invoke();
            performedJumps = 0;
            status.GoToGroundState(GroundState.Grounded);
            ground = true;
            runMomentumCounter = 0;
            status.EnableCollider();
        }
        else if (transform.position.y > 0.1F)
        {
            ground = false;
        }


        if (ground) col.material = groundMat;
        else col.material = airMat;

        return ground;
    }

    public void PlayerMovement()
    {
        if (ground)
        {
            if (runMomentumCounter > 0 && !sprinting)
            {
                //Run momentum + normal momentum
                rb.velocity = new Vector3((storedDirection.normalized * actualVelocity).x, rb.velocity.y, (storedDirection.normalized * actualVelocity).z) + runDirection * walkSpeed / (runMomentumDuration / runMomentumCounter);
                runMomentumCounter--;
            }
            else
                rb.velocity = new Vector3((storedDirection.normalized * actualVelocity).x, rb.velocity.y, (storedDirection.normalized * actualVelocity).z);
            //rb.velocity = CalculateRight(activeMove.m[i].momentum.x) + transform.up * rb.velocity.y + transform.forward * ;
        }
        else
        {
            if (runMomentumCounter > 0)
                rb.velocity = new Vector3((storedDirection.normalized * actualVelocity).x, rb.velocity.y, (storedDirection.normalized * actualVelocity).z) + runDirection * backWalkSpeed;
            else
                rb.velocity = new Vector3(storedDirection.x, rb.velocity.y, storedDirection.z);
            print(rb.velocity);
        }
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
