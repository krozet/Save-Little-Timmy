using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{

    public Transform CrazyJoe;
    private Vector3 cameraOffset;

    [Range(0.01f, 1.0f)]
    public float SmoothFactor = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        cameraOffset = transform.position - CrazyJoe.position;
    }

    // LateUpdate is called after Update
    void LateUpdate()
    {
        Vector3 newPos = CrazyJoe.position + cameraOffset;

        transform.position = Vector3.Slerp(transform.position, newPos, SmoothFactor);
    }
}
