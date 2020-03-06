using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Used for controlling the accelration of the piss to reach max speed
public class Kegels : MonoBehaviour
{
    // Kegel states
    public const int NOT_PISSING = -69;
    public const int ACCELERATING = 1;
    public const int DECACCELERATING = 2;
    // hyperDeaccelerate is use to stop pissing completely
    public const int HYPER_DECACCELERATING = 3;
    public const int MAX_SPEED = 4;

    // Larger = faster acceleration
    // Works to take a percentage of the value of the accelerationValues
    public float rateOfAcceleration = 0.1f;
    public float rateOfHyperDeacceleration = 0.5f;

    // Bounds used for accleration speeds
    int min = -5;
    int max = 10;
    int[] accelerationValues;
    int acclerationValuesIndex;

    bool needsToBeReset;
    int state;

    float speed = 0f;

    // Start is called before the first frame update
    void Start()
    {
        state = NOT_PISSING;
        accelerationValues = new int[50];
        acclerationValuesIndex = 0;
        resetAccelerationValues();      
        needsToBeReset = false;
    }

    // Update is called once per frame
    void Update()
    {
        HandlePissState();
    }

    void HandlePissState() {
        // handle acceleration of piss
        if (state == ACCELERATING) {
            Accelerate();
        } else {
            // handle deacceleration of piss
            if (state == DECACCELERATING) {
                Deaccelerate(DECACCELERATING);
            } else if (state == HYPER_DECACCELERATING) {
                Deaccelerate(HYPER_DECACCELERATING);
            }
            // see if piss has completely stopped
            if (speed <= 0f) {
                speed = 0f;
                state = NOT_PISSING;
            }
            // reset acceleration rate values if the user has stopped pissing completely
            if (state == NOT_PISSING && needsToBeReset) {
                resetAccelerationValues();
                needsToBeReset = false;
            } else if (state != NOT_PISSING){
                needsToBeReset = true;
            }
        }
    }
    // Generates realistic acceleration
    void resetAccelerationValues() {
        acclerationValuesIndex = 0;
        for (int i = 0; i < accelerationValues.Length; i++) {
            accelerationValues[i] = Random.Range(min, max);
        }
    }

    void Accelerate() {
        speed += accelerationValues[acclerationValuesIndex++ % accelerationValues.Length] * rateOfAcceleration;
    }

    void Deaccelerate(int deaccelerationType) {
        switch(deaccelerationType) {
            case DECACCELERATING:
                speed -= System.Math.Abs(accelerationValues[acclerationValuesIndex++ % accelerationValues.Length] * rateOfAcceleration);
                break;
            case HYPER_DECACCELERATING:
                speed -= System.Math.Abs(accelerationValues[acclerationValuesIndex++ % accelerationValues.Length] * rateOfHyperDeacceleration);
                break;
            default:
                break;
        }
    }

    public float GetPissSpeed(float maxSpeed) {
        // User isn't pissing anymore, so deaccelerate till there is no piss left
        if (maxSpeed == NOT_PISSING) {
            HyperDeaccelerlatePiss();
        } else {
            // accelerate to reach max speed
            if (speed < maxSpeed) {
                state = ACCELERATING;
                // deaccelerate to reach max speed
            } else if (speed > maxSpeed) {
                state = DECACCELERATING;
            }
        }
        return speed;
    }

    // hyper deaccelerate till there is no piss left
    public void HyperDeaccelerlatePiss() {
        if (speed > 0f && state != NOT_PISSING) {
            state = HYPER_DECACCELERATING;
        }
    }
}
