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
}
