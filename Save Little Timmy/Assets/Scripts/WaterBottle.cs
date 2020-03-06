using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBottle : MonoBehaviour {

    CrazyJoe crazyJoe;
    public GameObject pickupEffect;

    void OnTriggerEnter (Collider other)
    {
        Debug.Log("on trigger entered");
        if (other.CompareTag("CrazyJoe")) {
               // Pickup();
        }   
    }

    public void Pickup()
    {
        // Spawn an effect
        Quaternion rotationOffset = Quaternion.Euler(transform.rotation.x - 90, transform.rotation.y, transform.rotation.z);
        Instantiate(pickupEffect, transform.position, rotationOffset);
        
        // Remove WaterBottle
        Destroy(gameObject);
    }

    // amount of Piss Fuel this waterBottle will give
    public float GetPissFuelAmount() {
        return 250f;
    }
}
