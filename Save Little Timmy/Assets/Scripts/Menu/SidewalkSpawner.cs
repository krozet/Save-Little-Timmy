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
     * Creates a sidewalk and begins moving it
     * Another sidewalk is spawned at the spawner and waits for
     * the sidewalk infront of it to move out of the spawner before
     * it begins moving
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
        leftSidewalkEdgeSpawnPoint.transform.parent = transform;
        rightSidewalkEdgeSpawnPoint = Instantiate(spawnPointPrefab, Vector3.zero, Quaternion.identity);
        rightSidewalkEdgeSpawnPoint.transform.parent = transform;

        leftSidewalkEdgeSpawnPoint.GetComponent<SpawnPoint>().init(LEFT_SIDEWALK_SPAWN_POINT, 0);
        rightSidewalkEdgeSpawnPoint.GetComponent<SpawnPoint>().init(RIGHT_SIDEWALK_SPAWN_POINT, 0);
        // Create center side walk spawn points
        for (int i = 0; i < sideWalkLength - 2; i++) {
            centerSidewalkSpawnPoints.Add(Instantiate(spawnPointPrefab, Vector3.zero, Quaternion.identity));
            centerSidewalkSpawnPoints[i].GetComponent<SpawnPoint>().init(CENTER_SIDEWALK_SPAWN_POINT, i);
            centerSidewalkSpawnPoints[i].transform.parent = transform;
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

    // Spawns two sidewalks per spawner - the initial sidewalk and the next sidewalk
    private void SpawnInitialSidewalks() {
        GameObject sidewalk;

        // Create the initial left sidewalk that will begin moving immediately
        sidewalk = SpawnNextObject(LEFT_SIDEWALK_SPAWN_POINT, 0);
        sidewalk.GetComponent<SpawnableObject>().IsMoving(true);
        // Create the next sidewalk that will wait to move till the initial left sidewalk
        // has left the spawn point
        sidewalk = SpawnNextObject(LEFT_SIDEWALK_SPAWN_POINT, 0);
        leftSidewalkEdgeSpawnPoint.GetComponent<SpawnPoint>().SetInitialNextObject(sidewalk);

        // Create the initial right sidewalk that will begin moving immediately
        sidewalk = SpawnNextObject(RIGHT_SIDEWALK_SPAWN_POINT, 0);
        sidewalk.GetComponent<SpawnableObject>().IsMoving(true);
        // Create the next sidewalk that will wait to move till the initial right sidewalk
        // has left the spawn point
        sidewalk = SpawnNextObject(RIGHT_SIDEWALK_SPAWN_POINT, 0);
        rightSidewalkEdgeSpawnPoint.GetComponent<SpawnPoint>().SetInitialNextObject(sidewalk);

        for (int i = 0; i <= centerSidewalkSpawnPoints.Count - 1; i++) {
            // Create the initial right sidewalk that will begin moving immediately
            sidewalk = SpawnNextObject(CENTER_SIDEWALK_SPAWN_POINT, i);
            sidewalk.GetComponent<SpawnableObject>().IsMoving(true);
            // Create the next sidewalk that will wait to move till the initial right sidewalk
            // has left the spawn point
            sidewalk = SpawnNextObject(CENTER_SIDEWALK_SPAWN_POINT, i);
            centerSidewalkSpawnPoints[i].GetComponent<SpawnPoint>().SetInitialNextObject(sidewalk);
        }
    }

    private GameObject SpawnLeftSidewalk() {
        GameObject temp;
        bool isLargeSidewalk = false;
        int randInt = Random.Range(1, 40);
        if (randInt == 1) {
            if (Random.Range(1, 3) != 1) {
                // defect small edges
                temp = Instantiate(smallSidewalkEdgeDefects[Random.Range(0, smallSidewalkEdgeDefects.Count - 1)], leftSidewalkEdgeSpawnPoint.transform.position, Quaternion.identity);
            }
            // large defect edge
            temp = Instantiate(largeSidewalkEdgeDefect, leftSidewalkEdgeSpawnPoint.transform.position, Quaternion.identity);
            isLargeSidewalk = true;
        } else {
            // regular small edges
            temp = Instantiate(smallSidewalkEdges[Random.Range(0, smallSidewalkEdges.Count - 1)], leftSidewalkEdgeSpawnPoint.transform.position, Quaternion.identity);
        }
        //temp.transform.RotateAround(temp.transform.position, Vector3.up, -90);
        temp.transform.parent = transform;

        InitializeSpawnableObject(temp, leftSidewalkEdgeSpawnPoint, isLargeSidewalk, SpawnableObject.ROTATE_HOUSE_LEFT);
        AdjustSpawner(temp.GetComponent<SpawnableObject>(), leftSidewalkEdgeSpawnPoint);

        return temp;
    }

    private GameObject SpawnRightSidewalk() {
        GameObject temp;
        //Quaternion spawnRotation = Quaternion.Euler(transform.rotation.x, 90, transform.rotation.z);
        bool isLargeSidewalk = false;
        int randInt = Random.Range(1, 40);
        if (randInt == 1) {
            if (Random.Range(1, 3) != 1) {
                // defect small edges
                temp = Instantiate(smallSidewalkEdgeDefects[Random.Range(0, smallSidewalkEdgeDefects.Count - 1)], rightSidewalkEdgeSpawnPoint.transform.position, Quaternion.identity);
            }
            // large defect edge
            temp = Instantiate(largeSidewalkEdgeDefect, rightSidewalkEdgeSpawnPoint.transform.position, Quaternion.identity);
            isLargeSidewalk = true;
        } else {
            // regular small edges
            temp = Instantiate(smallSidewalkEdges[Random.Range(0, smallSidewalkEdges.Count - 1)], rightSidewalkEdgeSpawnPoint.transform.position, Quaternion.identity);
        }
        //temp.transform.Rotate(0, 90, 0);
        temp.transform.parent = transform;

        InitializeSpawnableObject(temp, rightSidewalkEdgeSpawnPoint, isLargeSidewalk, SpawnableObject.ROTATE_HOUSE_RIGHT);
        AdjustSpawner(temp.GetComponent<SpawnableObject>(), rightSidewalkEdgeSpawnPoint);

        return temp;
    }

    private GameObject SpawnCenterSidewalk(int index) {
        GameObject temp;
        int randInt = Random.Range(1, 40);
        if (randInt == 1) {
            // small defect center
            temp = Instantiate(smallSidewalkCenterDefects[Random.Range(0, smallSidewalkCenterDefects.Count - 1)], centerSidewalkSpawnPoints[index].transform.position, Quaternion.identity);
        } else {
            // small center
            temp = Instantiate(smallSidewalkCenter, centerSidewalkSpawnPoints[index].transform.position, Quaternion.identity);
        }
        temp.transform.parent = transform;

        InitializeSpawnableObject(temp, centerSidewalkSpawnPoints[index], false, SpawnableObject.ROTATE_HOUSE_FORWARD);
        AdjustSpawner(temp.GetComponent<SpawnableObject>(), centerSidewalkSpawnPoints[index]);

        return temp;
    }

    // Creates and attaches the script to make the sidewalk a SpawnableObject
    private void InitializeSpawnableObject(GameObject sidewalk, GameObject spawnPoint, bool largeSidewalk, int rotationDirection) {
        // make new sidewalk a SpawnableObject
        SpawnableObject spawnableObject = sidewalk.AddComponent<SpawnableObject>();
        Vector3 position = spawnPoint.transform.position;
        if (largeSidewalk) {
            position.z -= sizeOfSingleSidewalk.z / 2f;
        }

        spawnableObject.init(position, veloicity, rotationDirection);
    }

    private void AdjustSpawner(SpawnableObject spawnableObject, GameObject spawnPoint) {
        // have spawn point match the size of the spawning obj
        spawnPoint.GetComponent<BoxCollider>().size = spawnableObject.GetColliderSize();
        // have spawn point match the scaling of the spawning obj
        spawnPoint.transform.localScale = spawnableObject.transform.localScale;
        spawnPoint.transform.rotation = spawnableObject.transform.rotation;
        spawnPoint.transform.position = new Vector3(spawnPoint.transform.position.x, spawnableObject.transform.position.y, spawnPoint.transform.position.z);
    }

    public GameObject SpawnNextObject(int typeOfSpawner, int index) {
        GameObject sidewalk;

        switch (typeOfSpawner) {
            case LEFT_SIDEWALK_SPAWN_POINT:
                sidewalk = SpawnLeftSidewalk();
                break;
            case RIGHT_SIDEWALK_SPAWN_POINT:
                sidewalk = SpawnRightSidewalk();
                break;
            case CENTER_SIDEWALK_SPAWN_POINT:
                sidewalk = SpawnCenterSidewalk(index);
                break;
            default:
                sidewalk = SpawnCenterSidewalk(index);
                break;
        }
        return sidewalk;
    }

    public void StartSpawning() {
        SpawnInitialSidewalks();
    }
}
