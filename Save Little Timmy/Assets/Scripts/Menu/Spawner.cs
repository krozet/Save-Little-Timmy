using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Spawner
{
    public void SpawnNextObject(int typeOfSpawner, int index);
    public void StartSpawning();
}
