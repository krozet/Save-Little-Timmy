using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    int typeOfSpawner;
    int index = 0;
    Spawner spawner;

    public void init(int _typeOFSpawn, int _index, Spawner _spawner) {
        typeOfSpawner = _typeOFSpawn;
        index = _index;
        spawner = _spawner;
    }

    private void OnTriggerExit(Collider other) {
        spawner.SpawnNextObject(typeOfSpawner, index);
    }
}
