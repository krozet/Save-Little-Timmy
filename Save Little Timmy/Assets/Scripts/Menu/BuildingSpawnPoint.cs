using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSpawnPoint : MonoBehaviour
{
    MenuSceneGenerator menuSceneGenerator;

    // Start is called before the first frame update
    void Start()
    {
        menuSceneGenerator = GameObject.Find("MenuSceneGenerator").GetComponent<MenuSceneGenerator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) {

    }

    private void OnTriggerStay(Collider other) {

    }

    private void OnTriggerExit(Collider other) {
        menuSceneGenerator.RandomInstanciation();
    }
}
