using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AnimationValue
{
    // Player movement direction
    public const int W = 1;
    public const int WD = 2;
    public const int D = 3;
    public const int SD = 4;
    public const int S = 5;
    public const int AS = 6;
    public const int A = 7;
    public const int AW = 8;

    // Player look direction
    public const int deg0 = 1;
    public const int deg45 = 2;
    public const int deg90 = 3;
    public const int deg135 = 4;
    public const int deg180 = 5;
    public const int deg225 = 6;
    public const int deg270 = 7;
    public const int deg315 = 8;

    // Player movement animation
    public const int RUN_FORWARD = 0;
    public const int FORWARD_STRAFE_RIGHT = 1;
    public const int STRAFE_RIGHT = 2;
    public const int BACKWARD_STRAFE_RIGHT = 3;
    public const int BACKWARD = 4;
    public const int BACKWARD_STRAFE_LEFT = 5;
    public const int STRAFE_LEFT = 6;
    public const int FORWARD_STRAFE_LEFT = 7;

    public static void PrintPlayerMovementAnimation(int value) {
        switch(value) {
            case 0:
                Debug.Log("Player Movement Animation = RUN_FORWARD");
                break;
            case 1:
                Debug.Log("Player Movement Animation = FORWARD_STRAFE_RIGHT");
                break;
            case 2:
                Debug.Log("Player Movement Animation = STRAFE_RIGHT");
                break;
            case 3:
                Debug.Log("Player Movement Animation = BACKWARD_STRAFE_RIGHT");
                break;
            case 4:
                Debug.Log("Player Movement Animation = BACKWARD");
                break;
            case 5:
                Debug.Log("Player Movement Animation = BACKWARD_STRAFE_LEFT");
                break;
            case 6:
                Debug.Log("Player Movement Animation = STRAFE_LEFT");
                break;
            case 7:
                Debug.Log("Player Movement Animation = FORWARD_STRAFE_LEFT");
                break;
            default:
                break;
        }
    }

    public static void PrintPlayerMovementDirection(int value) {
        switch (value) {
            case 1:
                Debug.Log("Player Movement Direction = W");
                break;
            case 2:
                Debug.Log("Player Movement Direction = WD");
                break;
            case 3:
                Debug.Log("Player Movement Direction = D");
                break;
            case 4:
                Debug.Log("Player Movement Direction = SD");
                break;
            case 5:
                Debug.Log("Player Movement Direction = S");
                break;
            case 6:
                Debug.Log("Player Movement Direction = AS");
                break;
            case 7:
                Debug.Log("Player Movement Direction = A");
                break;
            case 8:
                Debug.Log("Player Movement Direction = AW");
                break;
            default:
                Debug.Log("Player Movement set Incorrectly");
                break;
        }
    }
}
