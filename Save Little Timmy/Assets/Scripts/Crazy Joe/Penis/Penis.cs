using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Penis : MonoBehaviour
{
    public Transform emitterTransform;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (emitterTransform != null) {
            emitterTransform.transform.position = this.transform.position;
        } else {
            Debug.Log("Emitter = null");
        }
    }
}
