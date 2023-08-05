using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using MyInputSystem;

public class PlayerMovementNew : MonoBehaviour
{

    [Header("Configurations")]
    public bool Lock;
    public CameraController cam;
    public Transform orientation;
    public float SpeedMultiplier;
    public float WalkSpeed;
    public float RunningSpeed;
    public float MaxSpeed_Walk;
    public float MaxSpeed_Run;
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
    [Header("Counter Movement")]
    public float Threshold;
    public float counterMovement;
    [Space]
    [Space]
    [Space]
    [Header("Game Feel")]
    public Transform cameraMovementEffect;
    public float CameraTiltAmount;
    public float TiltSpeed;
    public GameObject JumpParticles;
    [Space]
    [Space]
    public float InputBufferMaxTime;
    float LastJumpPress;

    [Header("====FootSteps=====")]
    public float MinTimeBetweenSteps;
    public float MaxTimeBetweenSteps;
    private float currentTimeBetweenSteps;
    private bool PlayingFootSteps;
    [Header("====Audio=====")]
    public AudioClip JumpSounds;
    public AudioClip FootStepSounds;
    public AudioMixer SoundEffectsMixer;

    public static PlayerMovementNew instance;

    #region Private

    float x, z;
    Vector3 moveDir;
    Vector3 slideDir;
    bool jumping;
    float speedMultiplier = 1;
    bool WallRun;
    bool wasGrounded;
    [HideInInspector]
    public bool Sliding;
    [HideInInspector]
    public bool Running;

    Rigidbody rb;

    Vector3 lastPos;


    #endregion

    #region Additive Functions

    public Rigidbody GetRigidbody() { return rb; }

    public Vector3 GetGroundPosition()
    {
        Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 100f);
        return hit.point;
    }

    public Vector3 GetMoveDirUnModified()
    {
        return new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
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

        if (!Lock)
            DoMovement();


        if (Grounded && !wasGrounded)
        {
            OnEnterGround();
        }

        if (!Grounded && wasGrounded)
        {
            OnExitGround();
        }

        if ((x != 0 || z != 0) && Grounded && !Sliding)
        {
            cam.HeadBob(walkTime);
            walkTime += Time.deltaTime * cam.headBobSettings.BobSpeed;
        }
        else
        {
            //walkTime = 0
        }

        if (Sliding)
        {
            //Instantiate(JumpParticles, transform.position, Quaternion.identity);
        }



        if (transform.position.y >= 20 && lastPos.y < 20)
        {
            LevelGenerator.generator.SetPlayerFloor(1);
            MiniMapManager.GetMiniMap().SetMap(1);
        }
        else
        {
            LevelGenerator.generator.SetPlayerFloor(0);
            MiniMapManager.GetMiniMap().SetMap(0);
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
        AudioManager.PlaySound2D(JumpSounds, false, 1, 0, 1, SoundEffectsMixer);
        Instantiate(JumpParticles, transform.position - new Vector3(0, PlayerHeight, 0), Quaternion.identity);
    }

    void OnExitGround()
    {
        AudioManager.PlaySound2D(JumpSounds, false, 1, 0, 1, SoundEffectsMixer);
        Instantiate(JumpParticles, transform.position - new Vector3(0, PlayerHeight, 0), Quaternion.identity);
        //cam.AddExtraOffset(new Vector3(0, JumpingOffset, 0));
    }

    public Vector3 GetMovementVector()
    {
        return rb.velocity;
    }

    private void FixedUpdate()
    {

        rb.AddForce(Vector3.down * ExtraGravity * Time.deltaTime * 10);


        if (jumping)
        {
            rb.AddForce(Vector3.up * JumpForce);
            jumping = false;
            LastJumpPress = 0.0f;
        }

        if (Grounded)
            speedMultiplier = GroundSpeedMultiplier;
        else
            speedMultiplier = AirSpeedMultiplier;



        Vector2 mag = FindVelRelativeToLook();
        float xMag = mag.x, yMag = mag.y;

        //Counteract sliding and sloppy movement
        CounterMovement(x, z, mag);


        //moveDir = transform.forward * z + transform.right * x;




        if (!Sliding)
        {
            if (!Running)
            {
                if (x > 0 && xMag > MaxSpeed_Walk) x = 0;
                if (x < 0 && xMag < -MaxSpeed_Walk) x = 0;
                if (z > 0 && yMag > MaxSpeed_Walk) z = 0;
                if (z < 0 && yMag < -MaxSpeed_Walk) z = 0;


                //rb.AddForce(moveDir * WalkSpeed * SpeedMultiplier);



                rb.AddForce(orientation.transform.forward * z * WalkSpeed * Time.deltaTime);
                rb.AddForce(orientation.transform.right * x * WalkSpeed * Time.deltaTime);
            }
            //rb.velocity = new Vector3(moveDir.x * WalkSpeed, rb.velocity.y, moveDir.z * WalkSpeed);
            else
            {
                if (x > 0 && xMag > MaxSpeed_Run) x = 0;
                if (x < 0 && xMag < -MaxSpeed_Run) x = 0;
                if (z > 0 && yMag > MaxSpeed_Run) z = 0;
                if (z < 0 && yMag < -MaxSpeed_Run) z = 0;

                rb.AddForce(orientation.transform.forward * z * RunningSpeed * Time.deltaTime);
                rb.AddForce(orientation.transform.right * x * RunningSpeed * Time.deltaTime);

                //rb.AddForce(moveDir * RunningSpeed * SpeedMultiplier);
            }

            //rb.velocity = new Vector3(moveDir.x * RunningSpeed, rb.velocity.y, moveDir.z * RunningSpeed);
        }
        else
        {
            //rb.AddForce(slideDir * SlideForce * Time.deltaTime);
            //rb.AddForce(orientation.transform.right * x * WalkSpeed * Time.deltaTime);
            rb.velocity = new Vector3(slideDir.x * SlideForce, rb.velocity.y, slideDir.z * SlideForce);
        }

        //if (rb.velocity.magnitude >= MaxSpeed)
        //    rb.velocity = Vector3.ClampMagnitude(rb.velocity, MaxSpeed);
    }

    public void CounterMovement(float x, float y, Vector2 mag)
    {
        if (!Grounded || jumping) return;

        //Slow down sliding
        //if (Sliding)
        //{
        //    rb.AddForce(WalkSpeed * Time.deltaTime * -rb.velocity.normalized * slideCounterMovement);
        //    return;
        //}

        //Counter movement
        if (Math.Abs(mag.x) > Threshold && Math.Abs(x) < 0.05f || (mag.x < -Threshold && x > 0) || (mag.x > Threshold && x < 0))
        {
            rb.AddForce(WalkSpeed * orientation.transform.right * Time.deltaTime * -mag.x * counterMovement);
        }
        if (Math.Abs(mag.y) > Threshold && Math.Abs(y) < 0.05f || (mag.y < -Threshold && y > 0) || (mag.y > Threshold && y < 0))
        {
            rb.AddForce(WalkSpeed * orientation.transform.forward * Time.deltaTime * -mag.y * counterMovement);
        }
    }

    /// <summary>
    /// Find the velocity relative to where the player is looking
    /// Useful for vectors calculations regarding movement and limiting movement
    /// </summary>
    /// <returns></returns>
    public Vector2 FindVelRelativeToLook()
    {
        float lookAngle = transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        float magnitue = rb.velocity.magnitude;
        float yMag = magnitue * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitue * Mathf.Cos(v * Mathf.Deg2Rad);

        return new Vector2(xMag, yMag);
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
        var k_z = KeyBoardInput.GetAxis(KeyCode.W, KeyCode.S);
        var k_x = KeyBoardInput.GetAxis(KeyCode.D, KeyCode.A);

        var controller_axis = GamePadInput.GetHorizontalAndVerticalAxis(GamepadNumber.Gamepad01);

        //x = Input.GetAxisRaw("Horizontal");
        //z = Input.GetAxisRaw("Vertical");

        x = Math.Clamp(k_x + controller_axis.x, -1f, 1.0f);
        z = Math.Clamp(k_z + controller_axis.y, -1f, 1.0f);


        //Running = Input.GetKey(KeyCode.LeftShift) || GamePadInput.IsControllerButtonHeld(GamePadButton.RB, GamepadNumber.Gamepad01);

        if (moveDir.magnitude >= 1)
            moveDir = Vector3.ClampMagnitude(moveDir, 1.0f);

        if (LastJumpPress != 0.0f)
        {
            if ((Time.time - LastJumpPress) <= InputBufferMaxTime)
            {
                if (Grounded)
                    jumping = true;
            }
            else
            {
                LastJumpPress = 0.0f;
            }

        }

        if (Input.GetKeyDown(KeyCode.Space) || GamePadInput.IsControllerButtonPressed(GamePadButton.A))
        {
            if (Grounded)
                jumping = true;
            else
                LastJumpPress = Time.time;

        }

        if ((Input.GetKey(KeyCode.LeftControl) || GamePadInput.IsControllerButtonHeld(GamePadButton.B, GamepadNumber.Gamepad01)) && Grounded)
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
        AudioManager.PlaySound2D(FootStepSounds, false, 0.5f, 0.0f, UnityEngine.Random.Range(0.9f, 1.1f), SoundEffectsMixer);
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
