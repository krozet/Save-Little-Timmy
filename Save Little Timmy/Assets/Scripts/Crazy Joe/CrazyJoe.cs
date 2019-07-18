using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrazyJoe : MonoBehaviour
{
    SpawnPiss spawnPiss;
    PlayerController playerController;
    float currentPissMeter;
    float maxPissMeter = 1000;
    // This will be converted to damage per second
    float pissDamage = 0.1f;

    bool isPissing = false;

    // Start is called before the first frame update
    void Start()
    {
        currentPissMeter = maxPissMeter;
        playerController = GetComponent<PlayerController>();
        Transform penis = transform.Find("Penis");
        spawnPiss = new SpawnPiss();
        spawnPiss.init(playerController);
    }

    // Update is called once per frame
    void Update()
    {
        if (isPissing) {
            Piss();
        }
    }

    // This is called when CrazyJoe collides with a Trigger
    void OnTriggerEnter(Collider other) {
        // Checks if collision was with a water bottle
        if (other.CompareTag("WaterBottle")) {
            Debug.Log("Crazy Joe picked up the water bottle");

            WaterBottle waterBottle = other.gameObject.GetComponent<WaterBottle>();

            //prevents user from picking up bottles if full
            if (currentPissMeter < maxPissMeter) {
                waterBottle.Pickup();

                float fuelAmount = waterBottle.GetPissFuelAmount();
                RefillPissMeter(fuelAmount);
            }

        }
        
    }

    public float GetPissDamage() {
        return pissDamage / Time.deltaTime;
    }

    // This method takes in a piss fuel amount and updates CrazyJoe's current piss meter
    public void RefillPissMeter(float pissFuelQuantity) {
        currentPissMeter += pissFuelQuantity;

        if (currentPissMeter > maxPissMeter) {
            currentPissMeter = maxPissMeter;
        }

        if (currentPissMeter < 0) {
            currentPissMeter = 0;
        }
    }

    // Called by Player Controller
    public void IsPissing(bool _isPissing) {
        isPissing = _isPissing;
    }

    // If there is piss left in the Piss Meter,
    // then decrease the meter and spawn piss particle effects
    private void Piss() {
        if (currentPissMeter > 0) {
            currentPissMeter -= GetPissDamage();

            if (currentPissMeter < 0) {
                currentPissMeter = 0;
            }

            spawnPiss.SpawnPissEffect();

            Debug.Log("Current Piss Meter = " + currentPissMeter + " / " + maxPissMeter);
        } else {
            Debug.Log("You pissed away all your piss");
        }

    }
   
}
