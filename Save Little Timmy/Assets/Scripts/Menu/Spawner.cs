using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Spawner
{
    GameObject SpawnNextObject(int typeOfSpawner, int index);
    void StartSpawning();
}
