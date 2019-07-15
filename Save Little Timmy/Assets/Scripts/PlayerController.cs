using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 2;
    public bool debug = false;

    private Vector3 moveInput;
    private Vector3 moveVelocity;

    private Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = FindObjectOfType<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        // Consider Slerping the start and stop
        //Player movement
        moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        moveVelocity = moveInput * moveSpeed;
        transform.position += moveVelocity * Time.deltaTime;

        //Player rotation towards mouse
        Ray cameraRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayLength;

        if(groundPlane.Raycast(cameraRay, out rayLength)) {
            Vector3 pointToLookAt = cameraRay.GetPoint(rayLength);
            transform.LookAt(new Vector3(pointToLookAt.x, transform.position.y, pointToLookAt.z));

            if(debug) {
                Debug.DrawLine(cameraRay.origin, pointToLookAt, Color.red);
            }
        }
    }
}
