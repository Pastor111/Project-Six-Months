using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [Header("Configurations")]
    public CameraController cam;
    public float WalkSpeed;
    public float RunningSpeed;
    public float MaxSpeed;
    public float MaxMoveSpeed;
    public float Acceleration;
    public float JumpForce;
    float acc;
    [Space]
    public float GroundSpeedMultiplier;
    public float AirSpeedMultiplier;
    public float PlayerHeight;
    public float GroundDetectRadius;
    public LayerMask GroundLayer;
    [Space]
    [Space]
    [Space]
    [Header("Slide")]
    public float SlideForce;
    [Space]
    [Space]
    [Space]
    [Header("Gravity")]
    public float ExtraGravity;
    public float MaxGroundDistance;
    [Space]
    [Space]
    [Space]
    [Header("Game Feel")]
    public Transform cameraMovementEffect;
    public float CameraTiltAmount;
    public float TiltSpeed;

    [Header("====FootSteps=====")]
    public float MinTimeBetweenSteps;
    public float MaxTimeBetweenSteps;
    private float currentTimeBetweenSteps;
    private bool PlayingFootSteps;
    [Header("====Audio=====")]
    public AudioClip JumpSounds;
    public AudioClip FootStepSounds;

    public static PlayerMovement instance;

    #region Private

    float x, z;
    Vector3 moveDir;
    Vector3 slideDir;
    bool jumping;
    float speedMultiplier = 1;
    bool WallRun;
    bool wasGrounded;
    bool Sliding;
    [HideInInspector]
    public bool Running;

    Rigidbody rb;

    Vector3 lastPos;


    #endregion

    #region Additive Functions

    public Rigidbody GetRigidbody() { return rb; }

    public Vector3 GetMoveDirUnModified()
    {
        return new Vector3(x, 0, z);
    }

    float walkTime = 0;
    bool Grounded
    {
        get
        {
            var coll = Physics.OverlapSphere(transform.position - new Vector3(0, PlayerHeight, 0), GroundDetectRadius, GroundLayer);
            if (coll.Length > 0)
            {
                return true;
            }

            return false;
        }
    }

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        instance = this;

        
        SetPositionInstant(transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        DoMovement();


        if(Grounded && !wasGrounded)
        {
            OnEnterGround();
        }

        if(!Grounded && !wasGrounded)
        {
            OnExitGround();
        }

        if((x != 0 || z != 0) && Grounded && !Sliding)
        {
            cam.HeadBob(walkTime);
            walkTime += Time.deltaTime * cam.headBobSettings.BobSpeed;
        }
        else
        {
            //walkTime = 0
        }




        //if (Grounded)
        //    speedMultiplier = Mathf.Lerp(speedMultiplier, GroundSpeedMultiplier, 5 * Time.deltaTime);
        //else
        //    speedMultiplier = Mathf.Lerp(speedMultiplier, AirSpeedMultiplier, 5 * Time.deltaTime);

        wasGrounded = Grounded;
    }

    public void FixPosition()
    {
        if (Physics.Linecast(transform.position, lastPos, out RaycastHit hit))
        {
            if (hit.transform == transform)
                return;

            transform.position = lastPos;
        }
        else
        {
            lastPos = transform.position;
        }

    }

    void OnEnterGround()
    {
        AudioManager.PlaySound2D(JumpSounds);
    }

    void OnExitGround()
    {
        //AudioManager.PlaySound2D(JumpSounds);
        //cam.AddExtraOffset(new Vector3(0, JumpingOffset, 0));
    }

    private void FixedUpdate()
    {
        if (jumping)
        {
            rb.AddForce(Vector3.up * JumpForce);
            jumping = false;
        }

        if (Grounded)
            speedMultiplier = GroundSpeedMultiplier;
        else
            speedMultiplier = AirSpeedMultiplier;


        if (!Sliding)
        {
            if (!Running)
                rb.velocity = new Vector3(moveDir.x * WalkSpeed, rb.velocity.y, moveDir.z * WalkSpeed);
            else
                rb.velocity = new Vector3(moveDir.x * RunningSpeed, rb.velocity.y, moveDir.z * RunningSpeed);
        }
        else
        {
            rb.velocity = new Vector3(slideDir.x * SlideForce, rb.velocity.y, slideDir.z * SlideForce);
        }

         rb.AddForce(Vector3.down * ExtraGravity);

        if (rb.velocity.magnitude >= MaxSpeed)
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, MaxSpeed);
    }

    public bool VectorsAreSimilar(Vector3 a, Vector3 b)
    {
        Vector3 forward = a;
        Vector3 toOther = b;

        if (Vector3.Dot(forward, toOther) < 0)
        {
            return false;
        }

        return true;

    }

    public void SetPositionInstant(Vector3 pos)
    {
        transform.position = pos;
        lastPos = pos;
    }



    void DoMovement()
    {

        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");

        moveDir = transform.forward * z + transform.right * x;

        Running = Input.GetKey(KeyCode.LeftShift);

        if (moveDir.magnitude >= 1)
            moveDir = Vector3.ClampMagnitude(moveDir, 1.0f);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Grounded)
                jumping = true;

        }

        if (Input.GetKey(KeyCode.LeftControl) && Grounded)
        {

            if (!Sliding)
            {
                slideDir = transform.forward * z + transform.right * x;
            }

            Sliding = true;

            transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }
        else
        {
            Sliding = false;

            transform.localScale = new Vector3(1, 1, 1);
        }


        if (Grounded && !Sliding && (x != 0 || z != 0))
        {
            var vel = new Vector3(GetRigidbody().velocity.x, 0, GetRigidbody().velocity.z);
            var normalizedSpeed = vel.magnitude / MaxMoveSpeed;
            currentTimeBetweenSteps = Mathf.Lerp(MinTimeBetweenSteps, MaxTimeBetweenSteps, normalizedSpeed);

            if (!PlayingFootSteps)
                StartCoroutine(PlayFootSteps(currentTimeBetweenSteps));

        }

        ControlDrag();

    }

    IEnumerator PlayFootSteps(float t)
    {
        PlayingFootSteps = true;
        yield return new WaitForSeconds(t);
        AudioManager.PlaySound2D(FootStepSounds, false, 0.5f, 0.0f, UnityEngine.Random.Range(0.9f, 1.1f));
        PlayingFootSteps = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position - new Vector3(0, PlayerHeight, 0), GroundDetectRadius);
    }

    void ControlDrag()
    {
        //if (Grounded)
        //    rb.drag = rbGroundDrag;
        //else
        //    rb.drag = rbAirDrag;
    }
}
