using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    int typeOfSpawner;
    int index = 0;
    SidewalkSpawner spawner;
    bool initialized = false;

    public void init(int _typeOfSpawn, int _index) {
        initialized = true;
        spawner = GameObject.Find("MenuSceneManager").GetComponentInChildren<SidewalkSpawner>();
        typeOfSpawner = _typeOfSpawn;
        index = _index;
    }

    public bool IsInitialized() {
        return initialized;
    }

    private void OnTriggerExit(Collider other) {
        if (initialized) {
            if (spawner != null) {
                spawner.SpawnNextObject(typeOfSpawner, index);
            } else {
                Debug.Log("Spawner is null: " + gameObject.transform.position);
            }
        } else {
            Debug.Log("Not initialized!: " + gameObject.transform.position);
        }
    }
}
