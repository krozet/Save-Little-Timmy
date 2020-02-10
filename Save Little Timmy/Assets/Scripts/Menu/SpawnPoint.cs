using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    int typeOfSpawner;
    int index = 0;

    SidewalkSpawner spawner;
    GameObject nextObject;

    bool initialized = false;
    int sentinel = 0;

    public void init(int _typeOfSpawn, int _index) {
        initialized = true;
        spawner = GameObject.Find("MenuSceneManager").GetComponentInChildren<SidewalkSpawner>();
        typeOfSpawner = _typeOfSpawn;
        index = _index;
    }

    // Used for debuging spawn points
    public bool IsInitialized() {
        return initialized;
    }

    // This needs to be called on the first time when spawning begins
    // to set a reference to the first nextObject waiting to move
    public void SetInitialNextObject(GameObject _nextObject) {
        nextObject = _nextObject;
    }

    // When an object leaves the spawn point, begin moving nextObject and create a nextObject to be moved
    private void OnTriggerExit(Collider other) {
        if (sentinel == 0) {
            if (initialized) {
                if (spawner != null) {
                    // Starts moving nextObject
                    SpawnableObject spawnableObject = nextObject.GetComponent<SpawnableObject>();
                    spawnableObject.Begin();

                    // Sets the nextObject to move
                    nextObject = spawner.SpawnNextObject(typeOfSpawner, index);
                } else {
                    Debug.Log("Spawner is null: " + gameObject.transform.position);
                }
            } else {
                Debug.Log("Not initialized!: " + gameObject.transform.position);
            }

            sentinel++;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag != "Ground") {
            if (sentinel == 1) {
                sentinel = 0;
            }
        }
    }
}
