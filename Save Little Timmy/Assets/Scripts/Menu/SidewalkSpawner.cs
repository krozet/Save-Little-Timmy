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

    private int sideWalkLength = 4;

    private GameObject leftSidewalkEdgeSpawnPoint;
    private GameObject rightSidewalkEdgeSpawnPoint;
    private List<GameObject> centerSidewalkSpawnPoints;

    private List<GameObject> smallSidewalkEdges;
    private List<GameObject> smallSidewalkEdgeDefects;
    private List<GameObject> smallSidewalkCenterDefects;

    private bool spawnFirstObjects = false;
    private float veloicity = 2f;
    private Vector3 sizeOfSingleSidewalk;
    private Vector3 sizeOfTotalSidewalk;
    private Vector3 scaleFactor = new Vector3(10, 1, 10);

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
        IncreaseScaling();

        // we are primarily looking for the length of the whole sidewalk, not so concerned with the width
        sizeOfSingleSidewalk = smallSidewalkCenter.GetComponent<Renderer>().bounds.size;
        sizeOfTotalSidewalk = sizeOfSingleSidewalk * sideWalkLength;

        centerSidewalkSpawnPoints = new List<GameObject>();

        InitializeSpawnPoints();
        SetSpawnPointLocations();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CheckCenterSidewalkInitialization() {
        foreach (GameObject sw in centerSidewalkSpawnPoints) {
            Debug.Log("Is init?: " + sw.GetComponent<SpawnPoint>().IsInitialized());
        }
    }

    private void InitializeSpawnPoints() {
        // Create edge side walk spawn points
        leftSidewalkEdgeSpawnPoint = Instantiate(spawnPointPrefab, Vector3.zero, Quaternion.identity);
        rightSidewalkEdgeSpawnPoint = Instantiate(spawnPointPrefab, Vector3.zero, Quaternion.identity);

        leftSidewalkEdgeSpawnPoint.GetComponent<SpawnPoint>().init(LEFT_SIDEWALK_SPAWN_POINT, 0);
        rightSidewalkEdgeSpawnPoint.GetComponent<SpawnPoint>().init(RIGHT_SIDEWALK_SPAWN_POINT, 0);
        // Create center side walk spawn points
        for (int i = 0; i < 6; i++) {
            centerSidewalkSpawnPoints.Add(Instantiate(spawnPointPrefab, Vector3.zero, Quaternion.identity));
            centerSidewalkSpawnPoints[i].GetComponent<SpawnPoint>().init(CENTER_SIDEWALK_SPAWN_POINT, i);
        }
    }

    private void IncreaseScaling() {
        smallSidewalkCenter.transform.localScale = scaleFactor;
        largeSidewalkEdgeDefect.transform.localScale = scaleFactor;

        foreach (GameObject obj in smallSidewalkEdges) {
            obj.transform.localScale = scaleFactor;
        }

        foreach (GameObject obj in smallSidewalkEdgeDefects) {
            obj.transform.localScale = scaleFactor;
        }

        foreach (GameObject obj in smallSidewalkCenterDefects) {
            obj.transform.localScale = scaleFactor;
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
        Vector3 startingPos = transform.position;
        startingPos.x -= (sizeOfTotalSidewalk.x / 2f) + (sizeOfSingleSidewalk.x/2f);
        leftSidewalkEdgeSpawnPoint.transform.position = startingPos;
        startingPos.x += sizeOfSingleSidewalk.x;

        for (int i = 0; i <= centerSidewalkSpawnPoints.Count - 1; i++) {
            centerSidewalkSpawnPoints[i].transform.position = startingPos;
            startingPos.x += sizeOfSingleSidewalk.x;
        }

        rightSidewalkEdgeSpawnPoint.transform.position = startingPos;
    }

    private void SpawnInitialSidewalks() {
        SpawnNextObject(LEFT_SIDEWALK_SPAWN_POINT, 0);
        SpawnNextObject(RIGHT_SIDEWALK_SPAWN_POINT, 0);

        for (int i = 0; i <= centerSidewalkSpawnPoints.Count - 1; i++) {
            SpawnNextObject(CENTER_SIDEWALK_SPAWN_POINT, i);
        }
    }

    private void SpawnLeftSidewalk() {
        GameObject temp;
        Quaternion spawnRotation = Quaternion.Euler(transform.rotation.x, -90, transform.rotation.z);
        bool isLargeSidewalk = false;
        int randInt = Random.Range(1, 40);
        if (randInt == 1) {
            if (Random.Range(1, 3) != 1) {
                // defect small edges
                temp = Instantiate(smallSidewalkEdgeDefects[Random.Range(0, smallSidewalkEdgeDefects.Count - 1)], leftSidewalkEdgeSpawnPoint.transform.position, spawnRotation);
            }
            // large defect edge
            temp = Instantiate(largeSidewalkEdgeDefect, leftSidewalkEdgeSpawnPoint.transform.position, spawnRotation);
            isLargeSidewalk = true;
        } else {
            // regular small edges
            temp = Instantiate(smallSidewalkEdges[Random.Range(0, smallSidewalkEdges.Count - 1)], leftSidewalkEdgeSpawnPoint.transform.position, spawnRotation);
        }

        AdjustSpawner(temp, leftSidewalkEdgeSpawnPoint, isLargeSidewalk);
    }

    private void SpawnRightSidewalk() {
        GameObject temp;
        Quaternion spawnRotation = Quaternion.Euler(transform.rotation.x, 90, transform.rotation.z);
        bool isLargeSidewalk = false;
        int randInt = Random.Range(1, 40);
        if (randInt == 1) {
            if (Random.Range(1, 3) != 1) {
                // defect small edges
                temp = Instantiate(smallSidewalkEdgeDefects[Random.Range(0, smallSidewalkEdgeDefects.Count - 1)], rightSidewalkEdgeSpawnPoint.transform.position, spawnRotation);
            }
            // large defect edge
            temp = Instantiate(largeSidewalkEdgeDefect, rightSidewalkEdgeSpawnPoint.transform.position, spawnRotation);
            isLargeSidewalk = true;
        } else {
            // regular small edges
            temp = Instantiate(smallSidewalkEdges[Random.Range(0, smallSidewalkEdges.Count - 1)], rightSidewalkEdgeSpawnPoint.transform.position, spawnRotation);
        }

        AdjustSpawner(temp, rightSidewalkEdgeSpawnPoint, isLargeSidewalk);
    }

    private void SpawnCenterSidewalk(int index) {
        GameObject temp;
        int randInt = Random.Range(1, 40);
        if (randInt == 1) {
            // small defect center
            temp = Instantiate(smallSidewalkCenterDefects[Random.Range(0, smallSidewalkCenterDefects.Count - 1)], centerSidewalkSpawnPoints[index].transform.position, Quaternion.identity);
        } else {
            // small center
            temp = Instantiate(smallSidewalkCenter, centerSidewalkSpawnPoints[index].transform.position, Quaternion.identity);
        }

        AdjustSpawner(temp, centerSidewalkSpawnPoints[index], false);
    }

    private void AdjustSpawner(GameObject sidewalk, GameObject spawnPoint, bool largeSidewalk) {
        // make new sidewalk a SpawnableObject
        SpawnableObject spawnableObject = sidewalk.AddComponent<SpawnableObject>();
        Vector3 position = spawnPoint.transform.position;
        if (largeSidewalk) {
            position.z -= sizeOfSingleSidewalk.z/2f;
        }
        spawnableObject.init(position, veloicity);
        // have spawn point match the size of the spawning obj
        spawnPoint.GetComponent<BoxCollider>().size = spawnableObject.GetColliderSize();
        // have spawn point match the scaling of the spawning obj
        spawnPoint.transform.localScale = spawnableObject.transform.localScale;
        spawnPoint.transform.rotation = spawnableObject.transform.rotation;
        spawnPoint.transform.position = new Vector3(spawnPoint.transform.position.x, spawnableObject.transform.position.y, spawnPoint.transform.position.z);
    }

    public void SpawnNextObject(int typeOfSpawner, int index) {
        switch (typeOfSpawner) {
            case LEFT_SIDEWALK_SPAWN_POINT:
                //SpawnLeftSidewalk();
                break;
            case RIGHT_SIDEWALK_SPAWN_POINT:
                //SpawnRightSidewalk();
                break;
            case CENTER_SIDEWALK_SPAWN_POINT:
                SpawnCenterSidewalk(index);
                break;
            default:
                break;
        }
    }

    public void StartSpawning() {
        SpawnInitialSidewalks();
    }
}
