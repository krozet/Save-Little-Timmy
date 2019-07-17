using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBottle : MonoBehaviour {

    public GameObject pickupEffect;

    void OnTriggerEnter (Collider other)
    {
        if (other.CompareTag("CrazyJoe"))
        {
            Pickup();
        }   
    }

    void Pickup()
    {
        // Spawn an effect
        Instantiate(pickupEffect, transform.position, transform.rotation);

        // Fill up water Gauge

        // Remove WaterBottle
        Destroy(gameObject);
    }
}
