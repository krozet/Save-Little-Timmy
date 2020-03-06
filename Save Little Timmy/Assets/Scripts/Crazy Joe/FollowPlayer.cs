using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{

    public Transform crazyJoe;
    private Vector3 cameraOffset;
    private Vector3 startingCameraOffset;

    [Range(0.01f, 1.0f)]
    public float SmoothFactor = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        startingCameraOffset = new Vector3(4.0f, 8.6f, -5.7f);
        transform.position = crazyJoe.position + startingCameraOffset;
        
        cameraOffset = transform.position - crazyJoe.position;
    }

    // LateUpdate is called after Update
    void LateUpdate()
    {
        Vector3 newPos = crazyJoe.position + cameraOffset;

        transform.position = Vector3.Slerp(transform.position, newPos, SmoothFactor);
    }
}
