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
    private float currentDegree = 0f;
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
    private int currentMovementAnimationValue;

    LogMaster logMaster;

    // Start is called before the first frame update
    void Start()
    {
        logMaster = new LogMaster();

        mainCamera = FindObjectOfType<Camera>();
        crazyJoe = GetComponent<CrazyJoe>();
        penis = crazyJoe.GetComponentInChildren<Penis>().transform;
        animator = GetComponentInChildren<Animator>();

        currentMovementAnimationValue = AnimationValue.RUN_FORWARD;
        currentMovementDirection = AnimationValue.W;
        SetCurrentLookDirection();
    }

    void RunTestValues(int x, int y) {
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

        SetAnimation();

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
        bool isMovingHorizontal = false;
        bool isMovingVertical = false;
        if (currentSpeed <= 0.3f) {
            currentMovementDirection = AnimationValue.IDLE;
        }
        switch (Input.GetAxisRaw("Horizontal")) {
            //left
            case -1:
                isMovingHorizontal = true;
                // up/left
                if (currentMovementDirection == AnimationValue.W ) {
                    currentMovementDirection = AnimationValue.AW;
                // down/left
                } else if (currentMovementDirection == AnimationValue.S) {
                    currentMovementDirection = AnimationValue.AS;
                } else if (isMovingVertical == false) {
                    currentMovementDirection = AnimationValue.A;
                }
                break;
            // right
            case 1:
                isMovingHorizontal = true;
                // up/right
                if (currentMovementDirection == AnimationValue.W) {
                    currentMovementDirection = AnimationValue.WD;
                    // down/right
                } else if (currentMovementDirection == AnimationValue.S) {
                    currentMovementDirection = AnimationValue.SD;
                } else if (isMovingVertical == false) {
                    currentMovementDirection = AnimationValue.D;
                }
                break;
            default:
                isMovingHorizontal = false;
                break;
        }

        switch (Input.GetAxisRaw("Vertical")) {
            // down
            case -1:
                isMovingVertical = true;
                // down/left
                if (currentMovementDirection == AnimationValue.A) {
                    currentMovementDirection = AnimationValue.AS;
                    // down/right
                } else if (currentMovementDirection == AnimationValue.D) {
                    currentMovementDirection = AnimationValue.SD;
                } else if (isMovingHorizontal == false) {
                    currentMovementDirection = AnimationValue.S;
                }
                break;
            // up
            case 1:
                isMovingVertical = true;
                // up/left
                if (currentMovementDirection == AnimationValue.A) {
                    currentMovementDirection = AnimationValue.AW;
                    // up/right
                } else if (currentMovementDirection == AnimationValue.D) {
                    currentMovementDirection = AnimationValue.WD;
                } else if (isMovingHorizontal == false) {
                    currentMovementDirection = AnimationValue.W;
                }
                break;
            default:
                isMovingVertical = false;
                break;
        }

        animator.SetBool("isMovingHorizontal", isMovingHorizontal);
        animator.SetBool("isMovingVertical", isMovingVertical);
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

    // determines the direction the player is mostly facing
    private void SetCurrentLookDirection() {
        // try using mouse/plane intersection rotation?
        float degree = transform.rotation.eulerAngles.y;
        float cameraRotationOffset = mainCamera.transform.rotation.eulerAngles.y;
        float midDegree = 22.5f;

        if ((GetDegreeAndOffsetBetween0and360(degree, cameraRotationOffset, false) >= 0 && GetDegreeAndOffsetBetween0and360(degree, cameraRotationOffset, false) < 0 + midDegree)
            || (GetDegreeAndOffsetBetween0and360(degree, cameraRotationOffset, false) >= 360 - midDegree && GetDegreeAndOffsetBetween0and360(degree, cameraRotationOffset, false) <= 360)) {
            currentLookDirection = AnimationValue.deg0;
        } else if (GetDegreeAndOffsetBetween0and360(degree, cameraRotationOffset, false) >= 45 - midDegree && GetDegreeAndOffsetBetween0and360(degree, cameraRotationOffset, false) < 90 - midDegree) {
            currentLookDirection = AnimationValue.deg45;
        } else if (GetDegreeAndOffsetBetween0and360(degree, cameraRotationOffset, false) >= 90 - midDegree && GetDegreeAndOffsetBetween0and360(degree, cameraRotationOffset, false) < 135 - midDegree) {
            currentLookDirection = AnimationValue.deg90;
        } else if (GetDegreeAndOffsetBetween0and360(degree, cameraRotationOffset, false) >= 135 - midDegree && GetDegreeAndOffsetBetween0and360(degree, cameraRotationOffset, false) < 180 - midDegree) {
            currentLookDirection = AnimationValue.deg135;
        } else if (GetDegreeAndOffsetBetween0and360(degree, cameraRotationOffset, false) >= 180 - midDegree && GetDegreeAndOffsetBetween0and360(degree, cameraRotationOffset, false) < 225 - midDegree) {
            currentLookDirection = AnimationValue.deg180;
        } else if (GetDegreeAndOffsetBetween0and360(degree, cameraRotationOffset, false) >= 225 - midDegree && GetDegreeAndOffsetBetween0and360(degree, cameraRotationOffset, false) < 270 - midDegree) {
            currentLookDirection = AnimationValue.deg225;
        } else if (GetDegreeAndOffsetBetween0and360(degree, cameraRotationOffset, false) >= 270 - midDegree && GetDegreeAndOffsetBetween0and360(degree, cameraRotationOffset, false) < 315 - midDegree) {
            currentLookDirection = AnimationValue.deg270;
        } else {
            currentLookDirection = AnimationValue.deg315;
        }

        //Debug.Log("Center of Degree = " + GetDegreeBetween0and360((currentLookDirection * 45), cameraRotationOffset, false));
    }

    private float GetDegreeAndOffsetAndMidDegree(float degree, float offset, float midDegree, bool addition) {
        if (addition) {
            degree += midDegree;
        } else {
            degree -= midDegree;
        }

        return GetDegreeAndOffsetBetween0and360(degree, offset, false);
    }

    private float GetDegreeAndOffsetBetween0and360(float degree, float offset, bool addition) {
        if(addition) {
            degree += offset;
        } else {
            degree -= offset;
        }

        return GetDegreeBetween0and360(degree);
    }

    public float GetDegreeBetween0and360(float degree) {
        while (degree < 0) {
            degree += 360;
        }

        return degree % 360;
    }

    // sets the degree based on where the player is moving and what directiong they are looking
    private float GetAnimationBlendValue() {
        int movementAnimationValue = GetMovementAnimationValue(currentMovementDirection, currentLookDirection);
        // account for the camera's rotation
        float cameraRotationOffset = mainCamera.transform.rotation.eulerAngles.y;
        float degree = transform.rotation.eulerAngles.y;
        // use this to properly offset the look direction from the movement direction
        currentMovementAnimationValue = movementAnimationValue;
        float offset = currentMovementAnimationValue * 45f;
        float lookDirOffset = ((currentLookDirection - 1) * 45f);

        // make the offset your base point of referrence
        // take difference between your currentLookDirection and where you are looking at 0-360 around you
        // add that difference to your offset to determine what animation should be currently blended
        //float adjustedDegree = (degree - cameraRotationOffset) + offset - ((currentLookDirection - 1) * 45f);
        float adjustedDegree = (degree - cameraRotationOffset);

        adjustedDegree = GetDegreeBetween0and360(adjustedDegree);
        logMaster.Append("adjustedDegree", adjustedDegree);
        logMaster.Append("lookDirOffset", lookDirOffset);
        logMaster.Append("adjusted - lookDirOffset", (adjustedDegree - lookDirOffset));

        logMaster.PrintLongLog();
        AnimationValue.PrintPlayerMovementAnimation(currentMovementAnimationValue);
        //AnimationValue.PrintAllAnimationValues(currentLookDirection, currentMovementDirection, currentMovementAnimationValue);

        //TODO
        //make method that takes the "adjust - lookDirOffset and makes sure that when currenetLookDirection == deg0, it handles when it goes into the 340~ - 360 range
        //then link the animation controller
        //set Animator.playerMovementAnimation value equal to currentMovementAnimationValue
        //set degrees equal to "adjusted - lookDirOffset

        return adjustedDegree;
    }

    // consider using layered blend trees
        //forward blend tree
            // left/forward/right
        //left blend tree
            // forward/left/down
        //down blend tree
            // left/down/right
        // right blend tree
            // down/right/forward
        // ranging from -90 to 90?
    private void SetAnimation() {
        //logMaster.Append("Before currentDegree", currentDegree);
        currentDegree = Mathf.Lerp(currentDegree, GetAnimationBlendValue(), 0.05f);
        //logMaster.Append("After currentDegree", currentDegree);
        animator.SetFloat("degrees", currentDegree);
        //logMaster.PrintLongLog();
    }


    // finds the AnimationValue.(Player movement animation)
    public int GetMovementAnimationValue(int animationMovementDirection, int animationLookDirection) {
        int distance = animationMovementDirection - animationLookDirection;
        if (distance < 0) {
            distance += 8;
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
