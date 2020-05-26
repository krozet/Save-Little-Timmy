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

    // revisit this later
    // move out of Utility and make own class
    public static class Logger {

        public static void D<T>(string[] logs, List<T> values, bool longLog = false) {
            string message = "";
            if (longLog) {
                for (int i = 0; i < logs.Length; i++) {
                    if (i < values.Count) {
                        message += logs[i] + " = " + values[i].ToString() + "\n";
                    } else {
                        message += logs[i] + "\n";
                    }
                }
                Debug.Log(message);
            } else {
                // each log gets its own debug.log
                for (int i = 0; i < logs.Length; i++) {
                    if (i < values.Count) {
                        Debug.Log(logs[i] + " = " + values[i]);
                    } else {
                        Debug.Log(logs[i]);
                    }
                }
            }
        }
    }
}
