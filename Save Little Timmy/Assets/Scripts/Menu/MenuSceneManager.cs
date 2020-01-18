using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSceneManager : MonoBehaviour
{
    public GameObject[] objects;

    private GameObject sidewalkSpawner;
    private List<GameObject> instanciatedObjects;
    private float speed = 10f;

    // Start is called before the first frame update
    void Start()
    {
        instanciatedObjects = new List<GameObject>();
        sidewalkSpawner = GameObject.Find("SidewalkSpawner");
        sidewalkSpawner.GetComponent<SidewalkSpawner>().StartSpawning();
        //RandomInstanciation();
    }

    // Update is called once per frame
    void Update()
    {
        /*foreach (GameObject obj in instanciatedObjects) {
            obj.GetComponent<Rigidbody>().velocity = obj.transform.TransformDirection(Vector3.forward * speed);
        }*/
    }

    /*public void RandomInstanciation() {
        int randomIndex = (int)Random.Range(0, objects.Length - 1f);
        GameObject temp = Instantiate(objects[1],
                                            sidewalkSpawner.transform.position,
                                            Quaternion.identity);
        //temp.GetComponent<Rigidbody>().velocity = Vector3.forward * speed;
        temp.GetComponent<SpawnableObject>().init(sidewalkSpawner.transform.position, speed);
        instanciatedObjects.Add(temp);
    }*/
}
