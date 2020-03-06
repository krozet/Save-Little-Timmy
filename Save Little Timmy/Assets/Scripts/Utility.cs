using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    static public GameObject GetChildGameObject(GameObject fromGameObject, string withName) {
        Transform[] ts = fromGameObject.transform.GetComponentsInChildren<Transform>();
        foreach (Transform t in ts) if (t.gameObject.name == withName) return t.gameObject;
        return null;
    }

    // Adjust these MAX values!
    public static class PissedOnParticleEffectType {
        public static int SMOKE_PARTICLE_INDEX = 0;
        public static int SMOKE_PARICLE_MAX = 30;
        public static int BLOOD_PARTICLE_INDEX = 1;
        public static int BLOOD_PARTICLE_MAX = 30;
    }
}
