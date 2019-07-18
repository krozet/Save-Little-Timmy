using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPiss
{
    // Point where piss will spawn from
    public GameObject penis;

    private PlayerController playerController;
    private List<GameObject> instantiatedPiss;

    public void init(PlayerController _playerController) {
        // For now, grab the first piss effect at position 0 in the List
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
            piss = Object.Instantiate(penis.GetComponent<PissEffects>().GetPissEffect(), penis.transform.position, penis.transform.rotation);
        } else {
            Debug.Log("Can't find penis...");
        }
    }
}
