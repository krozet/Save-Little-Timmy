﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPiss
{
    // Point where piss will spawn from
    public GameObject penis;
    // List holding (potentially) mulitple piss effects
    public List<GameObject> pissEffects = new List<GameObject>();

    private PlayerController playerController;
    private GameObject effectToSpawn;
    private List<GameObject> instantiatedPiss;

    // Start is called before the first frame update
    //void Start()
    //{
    //    // For now, grab the first piss effect at position 0 in the List
    //    effectToSpawn = pissEffects[0];
    //    instantiatedPiss = new List<GameObject>();
    //    // Get a reference to the PlayerController script
    //    playerController = GetComponent<PlayerController>();
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    //if(Input.GetMouseButton(0)) {
    //    //    SpawnPissEffect();
    //    //}
    //}

    public void init(PlayerController _playerController) {
        // For now, grab the first piss effect at position 0 in the List
        effectToSpawn = pissEffects[0];
        instantiatedPiss = new List<GameObject>();
        // Get a reference to the PlayerController script
        playerController = _playerController;
        penis = GameObject.FindGameObjectWithTag("Penis");
    }

    public void SpawnPissEffect() {
        GameObject piss;

        if (penis != null) {
            if (playerController != null) {
                penis.transform.LookAt(playerController.GetLookAtDirection());
            }
            //piss = Object.Instantiate(penis.GetPissEffect(), penis.transform.position, penis.transform.rotation);
        } else {
            Debug.Log("Can't find penis head");
        }
    }
}
