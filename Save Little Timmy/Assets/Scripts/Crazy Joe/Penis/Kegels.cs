using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Used for controlling the accelration of the piss to reach max speed
public class Kegels : MonoBehaviour
{
    // Kegel states
    public static int NOT_PISSING = 0;
    public static int ACCELERATING = 1;
    public static int DECACCELERATING = 2;
    public static int MAX_SPEED = 3;

    public int state;

    // Bounds used for accleration speeds
    int min = -9;
    int max = 10;
    int[] accelerationValues;

    bool needsToBeReset;

    float speed = 0f;

    // Start is called before the first frame update
    void Start()
    {
        state = NOT_PISSING;
        accelerationValues = new int[50];
        resetAccelerationValues();
        needsToBeReset = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (state == NOT_PISSING && needsToBeReset) {
            resetAccelerationValues();
            needsToBeReset = false;
        } else {
            needsToBeReset = true;
        }
    }

    // Generates realistic acceleration
    void resetAccelerationValues() {
        for (int i = 0; i < accelerationValues.Length; i++) {
            int randomNum = Random.Range(min, max);
            if (i == 0) {
                accelerationValues[i] = randomNum;
            } else {
                accelerationValues[i] = accelerationValues[i - 1] + randomNum;
            }
        }
    }
}
