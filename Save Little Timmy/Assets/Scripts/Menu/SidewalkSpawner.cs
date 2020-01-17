using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SidewalkSpawner : MonoBehaviour, Spawner
{
    public const int LEFT_SIDEWALK_SPAWN_POINT = 0;
    public const int RIGHT_SIDEWALK_SPAWN_POINT = 1;
    public const int CENTER_SIDEWALK_SPAWN_POINT = 2;

    public GameObject spawnPointPrefab;

    public GameObject smallSidewalkEdge;
    public GameObject smallSidewalkEdge2;
    public GameObject smallSidewalkEdgeDefect;
    public GameObject smallSidewalkEdgeDefect2;
    public GameObject smallSidewalkEdgeDefect3;
    public GameObject largeSidewalkEdgeDefect;
    public GameObject smallSidewalkCenter;
    public GameObject smallSidewalkCenterDefect;
    public GameObject smallSidewalkCenterDefect2;

    public int sideWalkLength = 4;

    private GameObject leftSidewalkEdgeSpawnPoint;
    private GameObject rightSidewalkEdgeSpawnPoint;
    private List<GameObject> centerSidewalkSpawnPoints;

    private List<GameObject> smallSidewalkEdges;
    private List<GameObject> smallSidewalkEdgeDefects;
    private List<GameObject> smallSidewalkCenterDefects;

    private bool spawnFirstObjects = false;
    private float veloicity = 10f;

    /*
     * Holds data for the Sidewalk segment to be spawned
     * 
     *                length = 4
     *                _ _ _ _
     *    width = 2   _ _ _ _
     * 
     */

    // Start is called before the first frame update
    void Start()
    {
        AddSidewalksToLists();
        InitializeSpawnPoints();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InitializeSpawnPoints() {
        // Create edge side walk spawn points
        leftSidewalkEdgeSpawnPoint = Instantiate(spawnPointPrefab, Vector3.zero, Quaternion.identity);
        rightSidewalkEdgeSpawnPoint = Instantiate(spawnPointPrefab, Vector3.zero, Quaternion.identity);

        leftSidewalkEdgeSpawnPoint.GetComponent<SpawnPoint>().init(LEFT_SIDEWALK_SPAWN_POINT, 0, this);
        rightSidewalkEdgeSpawnPoint.GetComponent<SpawnPoint>().init(RIGHT_SIDEWALK_SPAWN_POINT, 0, this);
        // Create center side walk spawn points
        for (int i = 0; i <= sideWalkLength - 2; i++) {
            GameObject temp = Instantiate(spawnPointPrefab, Vector3.zero, Quaternion.identity);
            temp.GetComponent<SpawnPoint>().init(CENTER_SIDEWALK_SPAWN_POINT, i, this);
            centerSidewalkSpawnPoints.Add(temp);
        }
    }

    private void AddSidewalksToLists() {
        smallSidewalkEdges = new List<GameObject>();
        smallSidewalkEdges.Add(smallSidewalkEdge);
        smallSidewalkEdges.Add(smallSidewalkEdge2);

        smallSidewalkEdgeDefects = new List<GameObject>();
        smallSidewalkEdgeDefects.Add(smallSidewalkEdgeDefect);
        smallSidewalkEdgeDefects.Add(smallSidewalkEdgeDefect2);
        smallSidewalkEdgeDefects.Add(smallSidewalkEdgeDefect3);

        smallSidewalkCenterDefects = new List<GameObject>();
        smallSidewalkCenterDefects.Add(smallSidewalkCenterDefect);
        smallSidewalkCenterDefects.Add(smallSidewalkCenterDefect2);
    }

    private void SetSpawnPointLocations() {

    }

    private void SpawnLeftSidewalk() {
        GameObject temp;
        int randInt = Random.Range(1, 20);
        if (randInt == 1) {
            if (Random.Range(1, 2) == 1) {
                // defect small edges
                temp = Instantiate(smallSidewalkEdgeDefects[Random.Range(0, smallSidewalkEdgeDefects.Count - 1)], leftSidewalkEdgeSpawnPoint.transform.position, Quaternion.identity);
            }
            // large defect edge
            temp = Instantiate(largeSidewalkEdgeDefect, leftSidewalkEdgeSpawnPoint.transform.position, Quaternion.identity);
        } else {
            // regular small edges
            temp = Instantiate(smallSidewalkEdges[Random.Range(0, smallSidewalkEdges.Count - 1)], leftSidewalkEdgeSpawnPoint.transform.position, Quaternion.identity);
        }
        // make new sidewalk a SpawnableObject
        temp.AddComponent<SpawnableObject>();
        temp.GetComponent<SpawnableObject>().init(leftSidewalkEdgeSpawnPoint.transform.position, veloicity);
    }

    private void SpawnRightSidewalk() {
        GameObject temp;
        int randInt = Random.Range(1, 20);
        if (randInt == 1) {
            if (Random.Range(1, 2) == 1) {
                // defect small edges
                temp = Instantiate(smallSidewalkEdgeDefects[Random.Range(0, smallSidewalkEdgeDefects.Count - 1)], rightSidewalkEdgeSpawnPoint.transform.position, Quaternion.identity);
            }
            // large defect edge
            temp = Instantiate(largeSidewalkEdgeDefect, rightSidewalkEdgeSpawnPoint.transform.position, Quaternion.identity);
        } else {
            // regular small edges
            temp = Instantiate(smallSidewalkEdges[Random.Range(0, smallSidewalkEdges.Count - 1)], rightSidewalkEdgeSpawnPoint.transform.position, Quaternion.identity);
        }
        // make new sidewalk a SpawnableObject
        temp.AddComponent<SpawnableObject>();
        temp.GetComponent<SpawnableObject>().init(rightSidewalkEdgeSpawnPoint.transform.position, veloicity);
    }

    private void SpawnCenterSidewalk(int index) {
        GameObject temp;
        int randInt = Random.Range(1, 20);
        if (randInt == 1) {
            // small defect center
            temp = Instantiate(smallSidewalkCenterDefects[Random.Range(0, smallSidewalkCenterDefects.Count - 1)], centerSidewalkSpawnPoints[index].transform.position, Quaternion.identity);
        } else {
            // small center
            temp = Instantiate(smallSidewalkCenter, centerSidewalkSpawnPoints[index].transform.position, Quaternion.identity);
        }
        // make new sidewalk a SpawnableObject
        temp.AddComponent<SpawnableObject>();
        temp.GetComponent<SpawnableObject>().init(centerSidewalkSpawnPoints[index].transform.position, veloicity);
    }

    public void SpawnNextObject(int typeOfSpawner, int index) {
        switch (typeOfSpawner) {
            case LEFT_SIDEWALK_SPAWN_POINT:
                SpawnLeftSidewalk();
                break;
            case RIGHT_SIDEWALK_SPAWN_POINT:
                SpawnRightSidewalk();
                break;
            case CENTER_SIDEWALK_SPAWN_POINT:
                SpawnCenterSidewalk(index);
                break;
            default:
                break;
        }
    }

    public void StartSpawning() {
        SpawnNextObject(LEFT_SIDEWALK_SPAWN_POINT, 0);
        SpawnNextObject(RIGHT_SIDEWALK_SPAWN_POINT, 0);

        for (int i = 0; i <= centerSidewalkSpawnPoints.Count - 1; i++) {
            SpawnNextObject(CENTER_SIDEWALK_SPAWN_POINT, i);
        }
    }
}
