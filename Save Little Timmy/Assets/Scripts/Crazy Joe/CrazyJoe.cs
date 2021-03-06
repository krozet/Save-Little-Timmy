﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrazyJoe : MonoBehaviour
{
    Penis penis;
    // Create empty Audio Manager object
    AudioManager audioManager;
    bool isPissing = false;

    float currentPissMeter;
    float maxPissMeter = 100f;
    float pissDamage = 1f;
    float health;
    float maxHealth = 100f;

    void Start()
    {
        // Stores the AudioManager script
        audioManager = GameObject.Find("Audio Manager").GetComponent<AudioManager>();

        currentPissMeter = maxPissMeter;
        penis = GetComponentInChildren<Penis>();
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (isPissing) {
            Piss();
        } else {
            penis.IsPissing(false, GetPissDamage());
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
            // Plays crazy joe fire winced at crazy joe location
            audioManager.AMPlayOneShotAttached(AudioManager.CRAZYJOE_FIRE_WINCE_INDEX, AudioManager.CRAZYJOE_FIRE_WINCE_PATH, gameObject);
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
            //currentPissMeter -= GetPissDamage();

            if (currentPissMeter < 0) {
                currentPissMeter = 0;
            }

            if(penis != null) {
                penis.IsPissing(true, GetPissDamage());
            } else {
                Debug.Log("Can't find your penis...");
            }

            //Debug.Log("Current Piss Meter = " + currentPissMeter + " / " + maxPissMeter);
        } else {
            penis.IsPissing(false, GetPissDamage());
            //Debug.Log("You pissed away all your piss - Press 'Space' to refill piss");
        }

    }
   
}
