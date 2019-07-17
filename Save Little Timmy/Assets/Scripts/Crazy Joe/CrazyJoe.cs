using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrazyJoe : MonoBehaviour
{
    SpawnPiss spawnPiss;
    PlayerController playerController;
    float currentPissMeter;
    float maxPissMeter = 100;
    // This will be converted to damage per second
    float pissDamage = 20;

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
            spawnPiss.SpawnPissEffect();
        }
    }

    public float GetPissDamage() {
        return pissDamage / Time.deltaTime;
    }

    public void RefillPissMeter(float pissFuelQuantity) {
        currentPissMeter += pissFuelQuantity;

        if (currentPissMeter > maxPissMeter) {
            currentPissMeter = maxPissMeter;
        }

        if (currentPissMeter < 0) {
            currentPissMeter = 0;
        }
    }

    public void Piss() {
        isPissing = true;
    }

    public void StopPiss() {
        isPissing = false;
    }
}
