using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PissEffects : MonoBehaviour
{
    public List<GameObject> pissEffects = new List<GameObject>();
    private GameObject effectToSpawn;

    // Start is called before the first frame update
    void Start()
    {
        effectToSpawn = pissEffects[0];
    }

    public GameObject GetPissEffect() {
        return effectToSpawn;
    }
}
