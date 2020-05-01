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
    public float SmoothFactor = 0f;
    [Header("Distance from Crazy Joe")]
    public Vector3 AdjustableCameraOffset;
    [Header("Rotation")]
    public Quaternion rotation;


    // Start is called before the first frame update
    void Start()
    {
        AdjustableCameraOffset = Vector3.zero;
        //startingCameraOffset = new Vector3(4.0f, 8.6f, -5.7f);
        startingCameraOffset = new Vector3(0f, 1.8f, 0f);
        transform.position = crazyJoe.position + startingCameraOffset;
        
        cameraOffset = transform.position - crazyJoe.position;
        transform.rotation = crazyJoe.rotation;
    }

    // LateUpdate is called after Update
    void LateUpdate()
    {
        Vector3 newPos = crazyJoe.position + cameraOffset + AdjustableCameraOffset;
        rotation = new Quaternion(transform.rotation.x, crazyJoe.rotation.y, transform.rotation.z, transform.rotation.w);

        transform.position = Vector3.Slerp(transform.position, newPos, SmoothFactor);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, SmoothFactor);
    }
}
