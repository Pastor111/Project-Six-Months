using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public PlayerMovement player;

    public float MouseSensitivity = 100f;
    public float MinRotationClamp;
    public float MaxRotationClamp;

    [Space]
    [Space]
    [Space]
    [Header("Transforms")]
    public Transform PlayerBody;
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
    public float MinFov;
    public float MaxFov;
    public float FOVSpeed;


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
        Tilt();

        LookAround();

        

        CameraHead.position = CameraFollow.position;/* + CalCulateHeadBobOffset(WalkingTime);*/
        CameraHead.localRotation = CameraFollow.localRotation;


        var vel = new Vector3(player.GetRigidbody().velocity.x, 0, player.GetRigidbody().velocity.z);
        var fov = Mathf.Lerp(MinFov, MaxFov, vel.magnitude / player.MaxMoveSpeed);
        cam.fieldOfView = Mathf.MoveTowards(cam.fieldOfView, fov, FOVSpeed * Time.deltaTime);
        //float multiplier = 1;
        //Debug.Log((player.GetRigidbody().velocity.y / player.MaxYSpeed));
        //extraOffset.y = (player.GetRigidbody().velocity.y / player.MaxSpeed) * multiplier;
        //cameraOffset = Vector3.Lerp(cameraOffset, Vector3.zero, CameraOffsetSpeed * Time.deltaTime);

        //extraOffset = Vector3.MoveTowards(extraOffset, Vector3.zero, CameraOffsetSpeed * Time.deltaTime);
        //transform.parent.position = Vector3.MoveTowards(transform.parent.position, CameraHead.position + extraOffset, HeadBoobSmooth * Time.deltaTime);

        transform.parent.position = CameraHead.transform.position + extraOffset;

        if ((transform.parent.position - CameraHead.position).magnitude <= 0.001f)
        {
            transform.parent.position = CameraHead.position;

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

        float x = Input.GetAxisRaw("Mouse X");
        float y = Input.GetAxisRaw("Mouse Y");

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

        PlayerBody.rotation = Quaternion.Euler(0, yRotation, 0);
        //Orientation.rotation = Quaternion.Euler(0, yRotation, 0);

        //UpdateSway();
    }

    public void HeadBob(float t)
    {
        extraOffset.x = Mathf.Cos(t * headBobSettings.XBobFrequency) * headBobSettings.XBobAmplitude;
        extraOffset.y = Mathf.Sin(t * headBobSettings.YBobFrequency) * headBobSettings.YBobAmplitude;
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
