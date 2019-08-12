using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrazyJoe : MonoBehaviour
{
    SpawnPiss spawnPiss;
    PlayerController playerController;
    float currentPissMeter;
    float maxPissMeter = 100f;
    // This will be converted to damage per second
    float pissDamage = 1f;

    bool isPissing = false;

    float health = 100f;
    float maxHealth = 100f;
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

    private void OnCollisionEnter(Collision collision) {
        // Checks if collision was with fire
        if (collision.gameObject.tag == "Fire") {
            Debug.Log("Crazy Joe is on FIRE");
            Fire fire = collision.gameObject.GetComponent<Fire>();
            TakeDamage(fire.GetFireDamage());
        }
    }

    private void OnCollisionStay(Collision collision) {
        // Checks if collision was with fire
        if (collision.gameObject.tag == "Fire") {

            TakeDamage(1);
            Debug.Log("Crazy Joe health: " + health);

        }
    }

    public void TakeDamage(float damage) {
        if (health > 0) {
            health -= damage;
            if (health <= 0) {
                // You have died
                Debug.Log("YOU ARE DEAD");
            }
        } else {
            // You are still dead
            Debug.Log("YOU ARE STILL DEAD - Click 'H' to refill HP");
        }
    }

    public void HealHP(float amount) {
        health += amount;
        if (health > maxHealth) {
            health = maxHealth;
        }
    }

    public float GetPissDamage() {
        return pissDamage;
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
            Debug.Log("You pissed away all your piss - Press 'Space' to refill piss");
        }

    }
   
}
