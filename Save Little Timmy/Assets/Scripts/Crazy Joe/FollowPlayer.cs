using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{

    public Transform crazyJoe;
    private Vector3 startingCameraOffset;
    private Vector3 cameraOffset;

    [Header("Speed which Camera follows Crazy Joe")]
    [Range(0.01f, 1.0f)]
    public float SmoothFactor = 0.5f;
    [Header("Distance from Crazy Joe")]
    public Vector3 AdjustableCameraOffset;
    [Header("Rotation")]
    public float roll;
    public float pitch;
    public float yaw;


    // Start is called before the first frame update
    void Start()
    {
        AdjustableCameraOffset = Vector3.zero;
        startingCameraOffset = new Vector3(4.0f, 8.6f, -5.7f);
        transform.position = crazyJoe.position + startingCameraOffset;
        
        cameraOffset = transform.position - crazyJoe.position;

        roll = transform.rotation.z;
        pitch = transform.rotation.y;
        yaw = transform.rotation.x;
    }

    // LateUpdate is called after Update
    void LateUpdate()
    {
        Vector3 newPos = crazyJoe.position + cameraOffset + AdjustableCameraOffset;

        transform.position = Vector3.Slerp(transform.position, newPos, SmoothFactor);
        //Quaternion rotation = Quaternion.Euler(roll + transform.rotation.x, pitch + transform.rotation.y, yaw + transform.rotation.z);
        //Quaternion rotation = Quaternion.Euler(roll, pitch, yaw);
        //transform.rotation = Quaternion.Slerp(transform.rotation, rotation, SmoothFactor);
    }
}
