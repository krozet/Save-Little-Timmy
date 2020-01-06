﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
    Handles piss speed and swapping to different types of piss.
     */
public class Penis : MonoBehaviour
{
    public GameObject pissEmitter;
    public bool debug = true;

    PlayerController controller;
    Kegels kegels;
    Camera mainCamera;
    Obi.ObiEmitter emitter;
    bool isPissing = false;
    float pissDamage = 0f;

    void Start()
    {
        emitter = pissEmitter.GetComponent<Obi.ObiEmitter>();
        kegels = GetComponent<Kegels>();
        mainCamera = FindObjectOfType<Camera>();
        controller = GetComponentInParent(typeof(PlayerController)) as PlayerController;
    }

    // Update is called once per frame
    void Update()
    {
        // Change!
        isPissing = true;

        // Set emitter to penis position
        if (pissEmitter != null) {
            pissEmitter.transform.position = transform.position;
            pissEmitter.transform.localEulerAngles = transform.localEulerAngles;
        } else {
            Debug.Log("Emitter = null");
        }

        if(isPissing) {
            // Makes piss strength equal to the distance of mouse from the player
            float distanceBetweenPoints = (pissEmitter.transform.position - controller.GetMousePosition()).magnitude;
            // using a fixed value determined by testing speed over actual distance in game units go from 2.77 to 3.18
            float adjustedSpeed = distanceBetweenPoints/.292f;

            if (adjustedSpeed > 10f) {
                emitter.speed = 10f;
            } else {
                emitter.speed = adjustedSpeed;
            }

            //Debug.Log("Here's the distance: " + distanceBetweenPoints);
            if (debug) {
                // Debug line from penis to mouse
                Debug.DrawLine(pissEmitter.transform.position, controller.GetMousePosition(), Color.blue, 1 / 60f);
            }
        } else {
            emitter.speed = 0f;
        }
    }

    public void IsPissing(bool _isPissing, float _pissDamage) {
        isPissing = _isPissing;
        pissDamage = _pissDamage;
    }
}
