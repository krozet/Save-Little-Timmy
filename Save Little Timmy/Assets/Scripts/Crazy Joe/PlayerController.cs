using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 2;
    public bool debug = false;

    private Vector3 moveInput;
    private Vector3 moveVelocity;
    private Vector3 direction;
    private Camera mainCamera;
    private CrazyJoe crazyJoe;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = FindObjectOfType<Camera>();
        crazyJoe = GetComponent<CrazyJoe>();
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
        SetRotationTowardsMouse();

        // Left Click to piss
        if (Input.GetMouseButton(0)) {
            crazyJoe.IsPissing(true);
        }

        // Release Left Click to stop pissing
        if (Input.GetMouseButtonUp(0)) {
            crazyJoe.IsPissing(false);
        }

        // For testing purposes
        // Refil Piss Meter by 1000
        if (Input.GetKeyUp(KeyCode.Space)) {
            crazyJoe.RefillPissMeter(100);
        }

        // For testing purposes
        // Refil Health by 100
        if (Input.GetKeyUp(KeyCode.H)) {
            crazyJoe.HealHP(100);
        }
    }

    private void SetRotationTowardsMouse() {
        Ray cameraRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayLength;

        if (groundPlane.Raycast(cameraRay, out rayLength)) {
            Vector3 pointToLookAt = cameraRay.GetPoint(rayLength);
            direction = new Vector3(pointToLookAt.x, transform.position.y, pointToLookAt.z);
            transform.LookAt(direction);

            if (debug) {
                Debug.DrawLine(cameraRay.origin, pointToLookAt, Color.red);
            }
        }
    }

    public Vector3 GetLookAtDirection() {
        return direction;
    }
}
