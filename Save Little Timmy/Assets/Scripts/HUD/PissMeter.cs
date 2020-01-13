using Obi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PissMeter : MonoBehaviour
{
    ObiCollider[] colliders;
    bool once = true;
    bool twice = true;
    
    // Start is called before the first frame update
    void Start()
    {
        colliders = GetComponentsInChildren<ObiCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        adjustPhases();
    }

    // FRONT PLATE IS TOO LOW
    // PARTICLES ARE RUNNING INTO IT ON START

    // used to reset values due to bug in obi
    void adjustPhases() {
        if (once) {
            foreach (ObiCollider collider in colliders) {
                collider.AccurateContacts = true;
            }
            once = false;
        } else if (twice) {
            foreach (ObiCollider collider in colliders) {
                collider.AccurateContacts = false;
            }
            twice = false;
        }
    }
}
