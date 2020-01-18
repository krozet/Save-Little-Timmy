using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Spawner
{
    void SpawnNextObject(int typeOfSpawner, int index);
    void StartSpawning();
}
