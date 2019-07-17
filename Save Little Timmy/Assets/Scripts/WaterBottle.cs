using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBottle : MonoBehaviour {

    public GameObject pickupEffect;

    void OnTriggerEnter (Collider other)
    {
        Debug.Log("on trigger entered");
        if (other.CompareTag("CrazyJoe"))
        {
            Pickup();
        }   
    }

    void Pickup()
    {
        // Spawn an effect
        Quaternion rotationOffset = Quaternion.Euler(transform.rotation.x - 90, transform.rotation.y, transform.rotation.z);
        Instantiate(pickupEffect, transform.position, rotationOffset);
        
        // Fill up water Gauge

        // Remove WaterBottle
        Destroy(gameObject);
    }
}
