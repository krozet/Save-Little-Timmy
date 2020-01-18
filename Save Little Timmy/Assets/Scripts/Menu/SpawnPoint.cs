using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    int typeOfSpawner;
    int index = 0;
    Spawner spawner;

    public void init(int _typeOfSpawn, int _index, Spawner _spawner) {
        typeOfSpawner = _typeOfSpawn;
        index = _index;
        spawner = _spawner;
    }

    private void OnTriggerExit(Collider other) {
        if (spawner != null) {
            spawner.SpawnNextObject(typeOfSpawner, index);
        }
    }
}
