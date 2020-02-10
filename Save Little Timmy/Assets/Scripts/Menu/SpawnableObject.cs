using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Spawnable Menu Object
public class SpawnableObject : MonoBehaviour
{
    public const int ROTATE_HOUSE_LEFT = 0;
    public const int ROTATE_HOUSE_RIGHT = 1;
    public const int ROTATE_HOUSE_FORWARD = 2;
    Vector3 size;
    Vector3 spawnPoint;
    BoxCollider col;
    Rigidbody rb;
    Renderer mesh;
    Vector3 velocity = new Vector3(0,0,0);

    bool isMoving = false;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isMoving) {
            transform.localPosition += velocity * Time.deltaTime;
        }
    }

    public void init(Vector3 _spawnPoint, float _velocity, int rotateDirection, float maxHeightOfObjects = 0) {
        col = GetComponent<BoxCollider>();
        if (col == null) {
            col = gameObject.AddComponent<BoxCollider>();
        }

        mesh = GetComponent<Renderer>();
        if (mesh == null) {
            Debug.Log("Fix yo mesh");
        }

        rb = GetComponent<Rigidbody>();
        if (rb == null) {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.useGravity = false;
        rb.isKinematic = true;

        size = mesh.bounds.size;

        SetForwardVelocity(_velocity);
        SetStartPosition(_spawnPoint, maxHeightOfObjects);
        SetRotationDirection(rotateDirection);
    }

    public void IsMoving(bool _isMoving) {
        isMoving = _isMoving;
    }

    private void SetRotationDirection(int rotateDirection) {
        switch (rotateDirection) {
            case ROTATE_HOUSE_LEFT:
                RotateHouseLeft();
                break;
            case ROTATE_HOUSE_RIGHT:
                RotateHouseRight();
                break;
            case ROTATE_HOUSE_FORWARD:
                break;
            default:
                break;
        }
    }

    // Call this to start moving the object
    public void Begin() {
        SetPositionDirectlyBehindObjectAhead();
        IsMoving(true);
    }

    public Vector3 GetColliderSize() {
        return col.size;
    }

    public void SetStartPosition(Vector3 _spawnPoint, float maxHeightOfObjects) {
        spawnPoint = _spawnPoint;
        Vector3 startPosition = spawnPoint;
        // moves it to the center of the spawnPoint and sets it to the max height a sidewalk can be
        startPosition += new Vector3(size.x/2f, maxHeightOfObjects, size.z/2f);
        transform.position = startPosition;
    }

    // adds the distanceBetween the objAhead and the current object so that it will appear directly behind the objAhead
    public void SetPositionDirectlyBehindObjectAhead() {
        GameObject objectAhead = FindObjectAhead();
        if (objectAhead != null) {
            float distanceBetween = objectAhead.transform.position.z - transform.position.z - size.z;
            // if the distanceBetween is less than 0, then that means the size doesn't not need to be accounted for
            if (distanceBetween < 0) {
                distanceBetween += size.z;
            }
            // closes the gap
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + distanceBetween);
        }
    }

    // locates the objAhead, if there is one
    private GameObject FindObjectAhead() {
        RaycastHit objectHit;
        // Shoot raycast
        if (Physics.Raycast(transform.position, Vector3.forward, out objectHit, 500)) {
            return objectHit.collider.gameObject;
        }
        return null;
    }

    public void SetForwardVelocity(float _velocity) {
        velocity = new Vector3(0,0,_velocity);
    }

    // House right = stage left
    public void RotateHouseRight() {
        transform.rotation = Quaternion.LookRotation(Vector3.right, Vector3.zero);
        // adjusts for the rotation of the small pieces
        transform.position += new Vector3(0, 0, -size.z);
    }

    // House left = stage right
    public void RotateHouseLeft() {
        transform.rotation = Quaternion.LookRotation(Vector3.left, Vector3.zero);
        // adjusts for the rotation of the small pieces
        transform.position += new Vector3(-size.x, 0, 0);
    }

}
