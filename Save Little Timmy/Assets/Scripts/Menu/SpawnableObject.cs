using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Spawnable Menu Object
public class SpawnableObject : MonoBehaviour
{
    Vector3 size;
    Vector3 spawnPoint;
    BoxCollider col;
    Rigidbody rb;
    Vector3 velocity = new Vector3(0,0,0);

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.localPosition += velocity * Time.deltaTime;
    }

    public void init(Vector3 _spawnPoint, float _velocity) {
        col = GetComponent<BoxCollider>();
        if (col == null) {
            col = gameObject.AddComponent<BoxCollider>();
        }

        rb = GetComponent<Rigidbody>();
        if (rb == null) {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.useGravity = false;
        rb.isKinematic = true;

        size = col.bounds.size;

        SetForwardVelocity(_velocity);
        SetStartPosition(_spawnPoint);
    }

    public Vector3 GetColliderSize() {
        return col.size;
    }

    public void SetStartPosition(Vector3 _spawnPoint) {
        spawnPoint = _spawnPoint;
        Vector3 startPosition = spawnPoint;
        startPosition.y += gameObject.GetComponent<Collider>().bounds.size.y;
        //startPosition.z -= size.z / 2;
        transform.position = startPosition;
    }

    public void SetForwardVelocity(float _velocity) {
        velocity = new Vector3(0,0,_velocity);
        //rb.velocity = Vector3.forward * velocity;
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
