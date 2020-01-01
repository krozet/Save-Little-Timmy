using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
    Handles piss speed and swapping to different types of piss.
     */
public class Penis : MonoBehaviour
{
    public GameObject pissEmitter;

    Obi.ObiEmitter emitter;
    bool isPissing = false;
    float pissDamage = 0f;

    void Start()
    {
        emitter = pissEmitter.GetComponent<Obi.ObiEmitter>();
    }

    // Update is called once per frame
    void Update()
    {
        // Set emitter to penis position
        if (pissEmitter != null) {
            pissEmitter.transform.position = this.transform.position;
        } else {
            Debug.Log("Emitter = null");
        }

        if(isPissing) {
            emitter.speed = 1.7f;
        } else {
            emitter.speed = 0f;
        }
    }

    public void IsPissing(bool _isPissing, float _pissDamage) {
        isPissing = _isPissing;
        pissDamage = _pissDamage;
    }
}
