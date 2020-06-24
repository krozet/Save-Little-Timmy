using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class OccluderGroupProxy : MonoBehaviour
{

    FOWOccluder[] occluders;
    // Start is called before the first frame update
    void Start()
    {
        occluders = GetComponentsInChildren<FOWOccluder>();
    }

    public void OnTriggerEnter(Collider other){
        foreach(var item in occluders){
            item.OnTriggerEnter(other);
        }
    }
	public void OnTriggerExit(Collider other){
        foreach(var item in occluders){
            item.OnTriggerExit(other);
        }
    }
}
