using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadSpawner : MonoBehaviour, Spawner
{
    public const int LEFT_SIDE_ROAD = 0;
    public const int RIGHT_SIDE_ROAD = 1;

    public GameObject spawnPointPrefab;

    public GameObject yellowEdgeRoadWithDivisionLane;

    private List<GameObject> leftSideRoadSpawnPoints;
    private List<GameObject> rightSideRoadSpawnPoints;

    private float veloicity = 1f;
    private Vector3 sizeOfSingleRoad;
    private Vector3 sizeOfTotalRoad;
    private Vector3 scaleFactor = new Vector3(1, 1, 1);

    float maxHeightOfRoad = 0f;

    /*
     * Holds data for the Road segment to be spawned
     * Creates a Road and begins moving it
     * Another Road is spawned at the spawner and waits for
     * the Road infront of it to move out of the spawner before
     * it begins moving
     * 
     * Left Side Road
     *    |
     *    v
     * | - - | - - |
     *          ^
     *          |
     *        Right Side Road
     */

    // Start is called before the first frame update
    void Start()
    {
        IncreaseScaling();
        InitializeSpawnPoints();

        sizeOfSingleRoad = yellowEdgeRoadWithDivisionLane.GetComponentInChildren<MeshRenderer>().bounds.size;
        sizeOfTotalRoad = new Vector3((sizeOfSingleRoad.x * (leftSideRoadSpawnPoints.Count + rightSideRoadSpawnPoints.Count)), sizeOfSingleRoad.y, sizeOfSingleRoad.z);

        SetSpawnPointLocations();
    }

    // This will increase the side of each individual sidewalk squares
    public void IncreaseScaling() {
        yellowEdgeRoadWithDivisionLane.transform.localScale = scaleFactor;
    }

    public void SetMaxHeight(float _maxHeightOfRoad) {
        maxHeightOfRoad = _maxHeightOfRoad;
    }

    public void InitializeSpawnPoints() {
        leftSideRoadSpawnPoints = new List<GameObject>();
        rightSideRoadSpawnPoints = new List<GameObject>();

        for (int i = 0; i < 2; i++) {
            leftSideRoadSpawnPoints.Add(Instantiate(spawnPointPrefab, Vector3.zero, Quaternion.identity));
            leftSideRoadSpawnPoints[i].transform.parent = transform;
            leftSideRoadSpawnPoints[i].GetComponent<SpawnPoint>().init(i, i);
        }

        for (int i = 0; i < 2; i++) {
            rightSideRoadSpawnPoints.Add(Instantiate(spawnPointPrefab, Vector3.zero, Quaternion.identity));
            rightSideRoadSpawnPoints[i].transform.parent = transform;
            rightSideRoadSpawnPoints[i].GetComponent<SpawnPoint>().init(i , i);
        }
    }

    // Ensures that road spawn points are right next to each other in a line
    public void SetSpawnPointLocations() {
        Vector3 startingPos = transform.position;
        startingPos.x -= (sizeOfTotalRoad.x / 2f) + (sizeOfSingleRoad.x / 2f);

        for (int i = 0; i <= leftSideRoadSpawnPoints.Count - 1; i++) {
            leftSideRoadSpawnPoints[i].transform.position = startingPos;
            startingPos.x += sizeOfSingleRoad.x;
        }

        for (int i = 0; i <= rightSideRoadSpawnPoints.Count - 1; i++) {
            rightSideRoadSpawnPoints[i].transform.position = startingPos;
            startingPos.x += sizeOfSingleRoad.x;
        }
    }

    // Spawns two roads per spawner - the initial road and the next road
    public void SpawnInitialObjects() {
        GameObject road;

        for (int i = 0; i <= rightSideRoadSpawnPoints.Count - 1; i++) {
            // Create the initial right road that will begin moving immediately
            road = SpawnNextObject(RIGHT_SIDE_ROAD, i);
            road.GetComponent<SpawnableObject>().IsMoving(true);
            // Create the next road that will wait to move till the initial right road
            // has left the spawn point
            road = SpawnNextObject(RIGHT_SIDE_ROAD, i);
            rightSideRoadSpawnPoints[i].GetComponent<SpawnPoint>().SetInitialNextObject(road);
        }

        for (int i = 0; i <= leftSideRoadSpawnPoints.Count - 1; i++) {
            // Create the initial left road that will begin moving immediately
            road = SpawnNextObject(LEFT_SIDE_ROAD, i);
            road.GetComponent<SpawnableObject>().IsMoving(true);
            // Create the next road that will wait to move till the initial left road
            // has left the spawn point
            road = SpawnNextObject(LEFT_SIDE_ROAD, i);
            leftSideRoadSpawnPoints[i].GetComponent<SpawnPoint>().SetInitialNextObject(road);
        }
    }

    private GameObject SpawnRightSideOfRoad(int index) {
        GameObject temp;

        temp = Instantiate(yellowEdgeRoadWithDivisionLane, rightSideRoadSpawnPoints[index].transform.position, Quaternion.identity);
        temp.transform.parent = transform;

        switch (index) {
            case 0:
                InitializeSpawnableObject(temp, rightSideRoadSpawnPoints[index], SpawnableObject.ROTATE_HALF_TURN);
                break;
            case 1:
                InitializeSpawnableObject(temp, rightSideRoadSpawnPoints[index], SpawnableObject.ROTATE_HOUSE_FORWARD);
                break;
            default:
                break;
        }

        AdjustSpawner(temp.GetComponent<SpawnableObject>(), rightSideRoadSpawnPoints[index]);

        return temp;
    }

    private GameObject SpawnLeftSideOfRoad(int index) {
        GameObject temp;

        temp = Instantiate(yellowEdgeRoadWithDivisionLane, leftSideRoadSpawnPoints[index].transform.position, Quaternion.identity);
        temp.transform.parent = transform;

        switch (index) {
            case 0:
                InitializeSpawnableObject(temp, leftSideRoadSpawnPoints[index], SpawnableObject.ROTATE_HALF_TURN);
                break;
            case 1:
                InitializeSpawnableObject(temp, leftSideRoadSpawnPoints[index], SpawnableObject.ROTATE_HOUSE_FORWARD);
                break;
            default:
                break;
        }

        AdjustSpawner(temp.GetComponent<SpawnableObject>(), leftSideRoadSpawnPoints[index]);

        return temp;
    }

    // Creates and attaches the script to make the road a SpawnableObject
    private void InitializeSpawnableObject(GameObject road, GameObject spawnPoint, int rotationDirection) {
        // make new sidewalk a SpawnableObject
        SpawnableObject spawnableObject = road.AddComponent<SpawnableObject>();
        Vector3 position = spawnPoint.transform.position;

        spawnableObject.init(position, veloicity, rotationDirection, maxHeightOfRoad);
    }

    // Ensures that the spawn point is of correct dimensions to check for when a road leaves the spawner trigger
    public void AdjustSpawner(SpawnableObject spawnableObject, GameObject spawnPoint) {
        // have spawn point match the size of the spawning obj
        spawnPoint.GetComponent<BoxCollider>().size = spawnableObject.GetColliderSize();
        // have spawn point match the scaling of the spawning obj
        spawnPoint.transform.localScale = new Vector3(spawnableObject.transform.localScale.x, 100, spawnableObject.transform.localScale.z);
        spawnPoint.transform.rotation = spawnableObject.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject SpawnNextObject(int typeOfRoad, int index) {
        GameObject road;

        switch (typeOfRoad) {
            case LEFT_SIDE_ROAD:
                road = SpawnLeftSideOfRoad(index);
                break;
            case RIGHT_SIDE_ROAD:
                road = SpawnRightSideOfRoad(index);
                break;
            default:
                road = SpawnRightSideOfRoad(index);
                break;
        }
        return road;
    }

    public void StartSpawning() {
        SpawnInitialObjects();
    }

    public Vector3 GetTotalSize() {
        return sizeOfTotalRoad;
    }
}
