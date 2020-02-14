using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Spawner
{
    GameObject SpawnNextObject(int typeOfSpawner, int index);
    Vector3 GetTotalSize();
    void IncreaseScaling();
    void InitializeSpawnPoints();
    void SetSpawnPointLocations();
    void AdjustSpawner(SpawnableObject spawnableObject, GameObject spawnPoint);
    void SpawnInitialObjects();
    void StartSpawning();
}
