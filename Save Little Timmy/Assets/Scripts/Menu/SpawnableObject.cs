using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Spawnable Menu Object
public class SpawnableObject : MonoBehaviour
{
    Vector3 size;
    Vector3 spawnPoint;
    Rigidbody rb;
    float velocity;

    // Update is called once per frame
    void Update()
    {
        
    }

    public void init(Vector3 _spawnPoint, float _velocity) {
        size = GetComponent<Collider>().bounds.size;
        rb = GetComponent<Rigidbody>();
        spawnPoint = _spawnPoint;

        SetForwardVelocity(_velocity);
        SetStartPosition();
        RotateHouseRight();
    }

    public void SetStartPosition() {
        Vector3 startPosition = spawnPoint;
        startPosition.z -= size.z / 2;
        transform.position = startPosition;
    }

    public void SetForwardVelocity(float _velocity) {
        velocity = _velocity;
        rb.velocity = Vector3.forward * velocity;
    }

    // House right = stage left
    public void RotateHouseRight() {
        transform.rotation = Quaternion.LookRotation(Vector3.right, Vector3.zero);
    }

    // House left = stage right
    public void RotateHouseLeft() {
        transform.rotation = Quaternion.LookRotation(Vector3.left, Vector3.zero);
    }

}
