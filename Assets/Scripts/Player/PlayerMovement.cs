using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [Header("Configurations")]
    public CameraController cam;
    public float WalkSpeed;
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

    #region Private

    float x, z;
    Vector3 moveDir;
    bool jumping;
    float speedMultiplier = 1;
    bool WallRun;
    bool wasGrounded;

    Rigidbody rb;

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

        if((x != 0 || z != 0) && Grounded)
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

    void OnEnterGround()
    {
        
    }

    void OnExitGround()
    {
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



         rb.velocity = new Vector3(moveDir.x * WalkSpeed, rb.velocity.y, moveDir.z * WalkSpeed);
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



    void DoMovement()
    {

        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");

        moveDir = transform.forward * z + transform.right * x;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Grounded)
                jumping = true;

        }

        ControlDrag();

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
