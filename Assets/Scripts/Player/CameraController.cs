using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyInputSystem;

[System.Serializable]
public class HeadBobSettings
{
    public float XBobAmplitude;
    public float XBobFrequency;
    [Space]
    [Space]
    [Space]
    public float YBobAmplitude;
    public float YBobFrequency;
    public float BobSpeed;
}

public class CameraController : MonoBehaviour
{
    [Header("Configurations")]
    public bool Lock;
    public PlayerMovement player;

    public float MouseSensitivity = 100f;
    public float MinRotationClamp;
    public float MaxRotationClamp;

    [Space]
    [Space]
    [Space]
    [Header("Transforms")]
    public Transform CameraParent;
    public Transform PlayerBody;
    public Transform PlayerOrientation;
    public Transform CameraHead;
    public Transform CameraFollow;
    [Space]
    [Space]
    [Space]
    public Transform RightSteer;
    public Transform LeftSteer;
    [Space]
    [Space]
    [Space]
    [Header("Game Feel")]
    public HeadBobSettings headBobSettings;
    public HeadBobSettings Run_headBobSettings;
    public float MinFov;
    public float MaxFov;
    public float FOVSpeed;
    [Space]
    [Header("Weapon Sway")]
    public Transform ApplySway;
    public float SwayThreshold = 5;
    public float SwayLerp = 5;
    public Vector3 SwayLeft = new Vector3(0, 0.1f, 0);
    public Vector3 SwayRight = new Vector3(0, -0.1f, 0);

    public Vector3 SwayUp = new Vector3(0, 0.1f, 0);
    public Vector3 SwayDown = new Vector3(0, -0.1f, 0);
    public Vector3 SwayNormal;

    #region Private
    private Vector3 extraOffset;
    float xRotation = 0f;
    float yRotation;
    float zRotation;
    float landT = 1;
    Camera cam;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!Lock)
            Tilt();

        if(!Lock)
            LookAround();

        

        CameraHead.position = CameraFollow.position;/* + CalCulateHeadBobOffset(WalkingTime);*/
        CameraHead.localRotation = CameraFollow.localRotation;


        if (!player.Sliding)
        {
            //var vel = new Vector3(player.WalkSpeed, 0, player.WalkSpeed);
            if (player.GetMoveDirUnModified() == Vector3.zero)
            {
                var fov = Mathf.Lerp(MinFov, MaxFov, 0.0f);
                cam.fieldOfView = Mathf.MoveTowards(cam.fieldOfView, fov, FOVSpeed * Time.deltaTime);
            }
            else if(!player.Running && player.GetMoveDirUnModified() != Vector3.zero)
            {
                var fov = Mathf.Lerp(MinFov, MaxFov, 0.5f);
                cam.fieldOfView = Mathf.MoveTowards(cam.fieldOfView, fov, FOVSpeed * Time.deltaTime);
            }
            else
            {
                var fov = Mathf.Lerp(MinFov, MaxFov, 0.7f);
                cam.fieldOfView = Mathf.MoveTowards(cam.fieldOfView, fov, FOVSpeed * Time.deltaTime);
            }

        }
        else
        {
            var fov = Mathf.Lerp(MinFov, MaxFov, 0.8f);
            cam.fieldOfView = Mathf.MoveTowards(cam.fieldOfView, fov, FOVSpeed * Time.deltaTime);
        }

        //float multiplier = 1;
        //Debug.Log((player.GetRigidbody().velocity.y / player.MaxYSpeed));
        //extraOffset.y = (player.GetRigidbody().velocity.y / player.MaxSpeed) * multiplier;
        //cameraOffset = Vector3.Lerp(cameraOffset, Vector3.zero, CameraOffsetSpeed * Time.deltaTime);

        //extraOffset = Vector3.MoveTowards(extraOffset, Vector3.zero, CameraOffsetSpeed * Time.deltaTime);
        //transform.parent.position = Vector3.MoveTowards(transform.parent.position, CameraHead.position + extraOffset, HeadBoobSmooth * Time.deltaTime);

        CameraParent.position = CameraHead.transform.position + extraOffset;

        if ((CameraParent.position - CameraHead.position).magnitude <= 0.001f)
        {
            CameraParent.position = CameraHead.position;

        }
    }


    //public void AddExtraOffset(Vector3 offset)
    //{
    //    extraOffset += offset;
    //}

    void LookAround()
    {

        //float x = Input.x;
        //float y = mov.y;

        var controller_axis = GamePadInput.GetRightAxis();

        float x = Mathf.Clamp(Input.GetAxisRaw("Mouse X") + controller_axis.x, -1.0f, 1.0f);
        float y = Mathf.Clamp(Input.GetAxisRaw("Mouse Y") + controller_axis.y, -1.0f, 1.0f);

        xRotation -= y * MouseSensitivity * Time.deltaTime;
        yRotation += x * MouseSensitivity * Time.deltaTime;

        xRotation = Mathf.Clamp(xRotation, MinRotationClamp, MaxRotationClamp);

        //if (player.GetMoveDirUnModified().x != 0)
        //   zRotation = Quaternion.RotateTowards()
        //else
        //   zRotation = Mathf.MoveTowards(transform.parent.localRotation.eulerAngles.z, 0, player.TiltSpeed * Time.deltaTime);

        Vector3 v = transform.rotation.eulerAngles;

        transform.localRotation = Quaternion.Euler(xRotation, yRotation, v.z);
        //if (player.GetMoveDirUnModified().x < 0)
        //    transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.Euler(transform.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y, player.CameraTiltAmount * player.GetMoveDirUnModified().x), player.TiltSpeed * Time.deltaTime);
        //else if (player.GetMoveDirUnModified().x > 0)
        //    transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.Euler(transform.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y, player.CameraTiltAmount * player.GetMoveDirUnModified().x), player.TiltSpeed * Time.deltaTime);
        //else
        //    transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.Euler(transform.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y, 0), player.TiltSpeed * Time.deltaTime);

        //PlayerBody.rotation = Quaternion.Euler(0, yRotation, 0);
        PlayerOrientation.rotation = Quaternion.Euler(0, yRotation, 0);

        Vector3 finalSway = Vector3.zero;

        if(-x >= SwayThreshold)
        {
            finalSway += SwayLeft;
        }
        else if(-x <= -SwayThreshold)
        {
            finalSway += SwayRight;
        }
        else
        {
            finalSway += SwayNormal;
        }

        if (-y >= SwayThreshold)
        {
            finalSway += SwayDown;
        }
        else if (-y <= -SwayThreshold)
        {
            finalSway += SwayUp;
        }
        else
        {
            finalSway += SwayNormal;
        }

        ApplySway.localRotation = Quaternion.Lerp(ApplySway.localRotation, Quaternion.Euler(finalSway), SwayLerp * Time.deltaTime);

        //Orientation.rotation = Quaternion.Euler(0, yRotation, 0);

        //UpdateSway();
    }

    public void HeadBob(float t)
    {
        if (!player.Running)
        {
            extraOffset.x = Mathf.Cos(t * headBobSettings.XBobFrequency) * headBobSettings.XBobAmplitude;
            extraOffset.y = Mathf.Sin(t * headBobSettings.YBobFrequency) * headBobSettings.YBobAmplitude;
        }
        else
        {
            extraOffset.x = Mathf.Cos(t * Run_headBobSettings.XBobFrequency) * Run_headBobSettings.XBobAmplitude;
            extraOffset.y = Mathf.Sin(t * Run_headBobSettings.YBobFrequency) * Run_headBobSettings.YBobAmplitude;
        }

    }

    public void Tilt()
    {
        float rotZ = -Input.GetAxis("Horizontal") * player.CameraTiltAmount;

        //if (player.WallRunning)
        //{
        //    rotZ = player.WallOppositeDir.z * player.CameraTiltAmount_WallRun * player.transform.forward.x;
        //}

        Quaternion finalRot = Quaternion.Euler(xRotation, yRotation, rotZ);
        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, finalRot, player.TiltSpeed * Time.deltaTime);
    }

    public static float Spring(float from, float to, float time)
    {
        time = Mathf.Clamp01(time);
        time = (Mathf.Sin(time * Mathf.PI * (.2f + 2.5f * time * time * time)) * Mathf.Pow(1f - time, 2.2f) + time) * (1f + (1.2f * (1f - time)));
        return from + (to - from) * time;
    }

    public static Vector3 Spring(Vector3 from, Vector3 to, float time)
    {
        return new Vector3(Spring(from.x, to.x, time), Spring(from.y, to.y, time), Spring(from.z, to.z, time));
    }
}
