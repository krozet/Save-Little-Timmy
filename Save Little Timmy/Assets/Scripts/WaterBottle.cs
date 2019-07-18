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
        
        // This is NOT where we are going to update CrazyJoe's piss meter

        // Remove WaterBottle
        Destroy(gameObject);
    }

    // This method is called by the CrazyJoe script to see how much
    // Piss Fuel this waterBottle will give him
    public float GetPissFuelAmount() {
        // Change this number to a more appropriate value (maybe 200f or 400f, just keep it lower than 1000f)
        float fuelAmount = 0f;

        return fuelAmount;
    }
}
