using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSceneManager : MonoBehaviour
{
    public GameObject[] objects;

    private GameObject sidewalkSpawner;
    private GameObject roadSpawner;

    // Start is called before the first frame update
    void Start()
    {
        sidewalkSpawner = GameObject.Find("SidewalkSpawner");
        roadSpawner = GameObject.Find("RoadSpawner");

        SetSpawnerPositions();
        StartSpawning();
    }

    void SetSpawnerPositions() {
        SidewalkSpawner sidewalkSpawnerScript = sidewalkSpawner.GetComponent<SidewalkSpawner>();
        RoadSpawner roadSpawnerScript = roadSpawner.GetComponent<RoadSpawner>();

        Vector3 sidewalkSize = sidewalkSpawnerScript.GetTotalSize();
        Vector3 roadSize = roadSpawnerScript.GetTotalSize();
        Vector3 newRoadPos = new Vector3(roadSpawner.transform.position.x + sidewalkSize.x + roadSize.x/2f, roadSpawner.transform.position.y, roadSpawner.transform.position.z);

        float minSidewalkHeight = sidewalkSpawnerScript.GetMinHeight();

        roadSpawnerScript.SetMaxHeight(minSidewalkHeight);
        roadSpawner.transform.position = newRoadPos;
    }

    void StartSpawning() {
        sidewalkSpawner.GetComponent<SidewalkSpawner>().StartSpawning();
        roadSpawner.GetComponent<RoadSpawner>().StartSpawning();
    }

    void Update()
    {
        
    }
}
