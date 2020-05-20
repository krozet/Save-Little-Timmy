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

    public int currentLookDirection = AnimationValue.deg0;
    public int currentMovementDirection = AnimationValue.W;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = FindObjectOfType<Camera>();
        crazyJoe = GetComponent<CrazyJoe>();
        penis = crazyJoe.GetComponentInChildren<Penis>().transform;
        animator = GetComponentInChildren<Animator>();

        SetCurrentLookDirection();
    }

    void runTestValues(int x, int y) {
        Debug.Log("Ran these values, movement = " + x + " and look = " + y + " gets a value of = " + GetMovementAnimationValue(x,y));
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

        SetCurrentLookDirection();

        SetCurrentMovementDirection();

        SetAnimationBlendValue();



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

        animator.SetFloat("velocity", currentSpeed);
    }

    private void SetCurrentMovementDirection() {
        switch(Input.GetAxisRaw("Horizontal")) {
            //left
            case -1:
                // up/left
                if (currentMovementDirection == AnimationValue.W ) {
                    currentMovementDirection = AnimationValue.AW;
                // down/left
                } else if (currentMovementDirection == AnimationValue.S) {
                    currentMovementDirection = AnimationValue.AS;
                } else {
                    currentMovementDirection = AnimationValue.A;
                }
                break;
            // right
            case 1:
                // up/right
                if (currentMovementDirection == AnimationValue.W) {
                    currentMovementDirection = AnimationValue.WD;
                    // down/right
                } else if (currentMovementDirection == AnimationValue.S) {
                    currentMovementDirection = AnimationValue.SD;
                } else {
                    currentMovementDirection = AnimationValue.D;
                }
                break;
            default:
                break;
        }

        switch (Input.GetAxisRaw("Vertical")) {
            // down
            case -1:
                // down/left
                if (currentMovementDirection == AnimationValue.A) {
                    currentMovementDirection = AnimationValue.AS;
                    // down/right
                } else if (currentMovementDirection == AnimationValue.D) {
                    currentMovementDirection = AnimationValue.SD;
                } else {
                    currentMovementDirection = AnimationValue.S;
                }
                break;
            // up
            case 1:
                // up/left
                if (currentMovementDirection == AnimationValue.A) {
                    currentMovementDirection = AnimationValue.AW;
                    // up/right
                } else if (currentMovementDirection == AnimationValue.D) {
                    currentMovementDirection = AnimationValue.WD;
                } else {
                    currentMovementDirection = AnimationValue.W;
                }
                break;
            default:
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
            Quaternion lookDirection = Quaternion.LookRotation((direction - transform.position), Vector3.up);
            Vector3 cross = Vector3.Cross(transform.rotation * Vector3.forward, lookDirection * Vector3.forward);
            //Debug.Log("Look direction = " + cross);
            transform.LookAt(direction);
            //Debug.Log("Rotation = " + transform.rotation.eulerAngles);
            penis.localEulerAngles = transform.localEulerAngles;
        }

        // Draw a line from the camera to the mouse
        if (debug) {
            Debug.DrawLine(cameraRay.origin, mousePosition, Color.red);
        }
    }

    private void SetCurrentLookDirection() {
        float degree = transform.rotation.eulerAngles.y;
        float midDegree = 22.5f;

        if ((degree >= 0 && degree < 0 + midDegree) || (degree >= 360 - midDegree && degree <=360)) {
            currentLookDirection = AnimationValue.deg0;
        } else if (degree >= 45 - midDegree && degree < 90 - midDegree) {
            currentLookDirection = AnimationValue.deg45;
        } else if (degree >= 90 - midDegree && degree < 135 - midDegree) {
            currentLookDirection = AnimationValue.deg90;
        } else if (degree >= 135 - midDegree && degree < 180 - midDegree) {
            currentLookDirection = AnimationValue.deg135;
        } else if (degree >= 180 - midDegree && degree < 225 - midDegree) {
            currentLookDirection = AnimationValue.deg180;
        } else if (degree >= 225 - midDegree && degree < 270 - midDegree) {
            currentLookDirection = AnimationValue.deg225;
        } else if (degree >= 270 - midDegree && degree < 315 - midDegree) {
            currentLookDirection = AnimationValue.deg270;
        } else {
            currentLookDirection = AnimationValue.deg315;
        }
    }

    private void SetAnimationBlendValue() {
        int movementAnimationValue = GetMovementAnimationValue(currentMovementDirection, currentLookDirection);
        float degree = transform.rotation.eulerAngles.y;
        // use this to properly offset the look direction from the movement direction
        float offset = movementAnimationValue * 45f;
        float adjustedDegree = degree + offset;
        // if the degree is now negative, readjust to fit 0-360 scale
        if (adjustedDegree < 0) {
            adjustedDegree += 360;
        }

        if (adjustedDegree > 360) {
            adjustedDegree -= 360;
        }

        animator.SetFloat("degrees", adjustedDegree);

        Debug.Log("degree = " + degree +
            " offset = " + offset +
            " adjustedDegree = " + adjustedDegree);
    }

    public int GetMovementAnimationValue(int animationMovementDirection, int animationLookDirection) {
        int distance = 0;
        for (int i = animationMovementDirection; i != animationLookDirection; i++) {
            if (i % 8 == 0) {
                i = 0;
            }
            distance++;
        }
        return distance;
    }

    public Vector3 GetLookAtDirection() {
        return direction;
    }

    public Vector3 GetMousePosition() {
        return mousePosition;
    }
}
