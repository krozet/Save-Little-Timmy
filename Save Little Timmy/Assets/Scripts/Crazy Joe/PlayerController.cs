using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 2;
    private float currentSpeed = 0f;
    private float speedSmoothVelocity = 0f;
    private float speedSmoothTime = 0.1f;
    private float rotationSpeed = 0.1f;
    private float gravity = 3f;
    public bool debug = false;

    private Vector3 moveInput;
    private Vector3 moveVelocity;
    private Vector3 direction;
    private Vector3 mousePosition;
    private Camera mainCamera;
    private CrazyJoe crazyJoe;
    private Transform penis;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = FindObjectOfType<Camera>();
        crazyJoe = GetComponent<CrazyJoe>();
        penis = crazyJoe.GetComponentInChildren<Penis>().transform;
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Consider Slerping the start and stop
        //Player movement
        /*moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        moveVelocity = moveInput * moveSpeed;
        transform.position += moveVelocity * Time.deltaTime;*/

        Move();

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

        // For testing purposes
        // Fire animation
        if (Input.GetKeyUp(KeyCode.F)) {
            animator.SetTrigger("fire");
        }
    }

    private void Move() {
        Vector3 movementInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        Vector3 forward = mainCamera.transform.forward;
        Vector3 right = mainCamera.transform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        Vector3 desiredMoveDirection = (forward * movementInput.z + right * movementInput.x).normalized;

        float targetSpeed = moveSpeed * movementInput.magnitude;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);

        transform.position += desiredMoveDirection * currentSpeed * Time.deltaTime;
        SetDirectionalAnimations();
        animator.SetFloat("velocity", currentSpeed);
    }

    private void SetDirectionalAnimations() {
        switch(Input.GetAxisRaw("Horizontal")) {
            case -1:
                if (!animator.GetBool("right")) {
                    animator.SetBool("left", true);
                }
                break;
            case 1:
                if (!animator.GetBool("left")) {
                    animator.SetBool("right", true);
                }
                break;
            default:
                animator.SetBool("right", false);
                animator.SetBool("left", false);
                break;
        }

        switch (Input.GetAxisRaw("Vertical")) {
            case -1:
                if (!animator.GetBool("forward")) {
                    animator.SetBool("backward", true);
                }
                break;
            case 1:
                if (!animator.GetBool("backward")) {
                    animator.SetBool("forward", true);
                }
                break;
            default:
                animator.SetBool("backward", false);
                animator.SetBool("forward", false);
                break;
        }
    }

    private void SetRotationTowardsMouse() {
        Ray cameraRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane crazyJoePlace = new Plane(Vector3.up, crazyJoe.transform.position);
        RaycastHit hit;
        float rayLength;

        // Searches for an intersection with a walkable surface
        /*if (Physics.Raycast(cameraRay, out hit)) {
            Transform walkableSurface = hit.transform.Find("WalkOnable");
            if (walkableSurface != null) {
                mousePosition = cameraRay.GetPoint(hit.distance);
                direction = new Vector3(mousePosition.x, transform.position.y, mousePosition.z);
                transform.LookAt(direction);
                penis.localEulerAngles = transform.localEulerAngles;
            } 
        } else */

        // A ray passes through a plane at Crazy Joe's feet
        if (crazyJoePlace.Raycast(cameraRay, out rayLength)) {
            mousePosition = cameraRay.GetPoint(rayLength);
            direction = new Vector3(mousePosition.x, transform.position.y, mousePosition.z);
            transform.LookAt(direction);
            penis.localEulerAngles = transform.localEulerAngles;
        }

        // Draw a line from the camera to the mouse
        if (debug) {
            Debug.DrawLine(cameraRay.origin, mousePosition, Color.red);
        }
    }

    public Vector3 GetLookAtDirection() {
        return direction;
    }

    public Vector3 GetMousePosition() {
        return mousePosition;
    }
}
