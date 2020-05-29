using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AnimationValue
{
    // Player movement direction
    public const int IDLE = 0;
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
            case 0:
                Debug.Log("Player Movement Direction = IDLE");
                break;
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

    public static void PrintPlayerLookDirection(int value) {
        switch (value) {
            case 1:
                Debug.Log("Player Look Direction = deg0");
                break;
            case 2:
                Debug.Log("Player Look Direction = deg45");
                break;
            case 3:
                Debug.Log("Player Look Direction = deg90");
                break;
            case 4:
                Debug.Log("Player Look Direction = deg135");
                break;
            case 5:
                Debug.Log("Player Look Direction = deg180");
                break;
            case 6:
                Debug.Log("Player Look Direction = deg225");
                break;
            case 7:
                Debug.Log("Player Look Direction = deg270");
                break;
            case 8:
                Debug.Log("Player Look Direction = deg315");
                break;
            default:
                Debug.Log("Player Look set Incorrectly");
                break;
        }
    }

    public static void PrintAllAnimationValues(int playerLookDirection, int playerMovementDirection, int playerMovementAnimation) {
        string pld = "";
        string pmd = "";
        string pma = "";

        switch (playerLookDirection) {
            case 1:
                pld = "Player Look Direction = deg0";
                break;
            case 2:
                pld = "Player Look Direction = deg45";
                break;
            case 3:
                pld = "Player Look Direction = deg90";
                break;
            case 4:
                pld = "Player Look Direction = deg135";
                break;
            case 5:
                pld = "Player Look Direction = deg180";
                break;
            case 6:
                pld = "Player Look Direction = deg225";
                break;
            case 7:
                pld = "Player Look Direction = deg270";
                break;
            case 8:
                pld = "Player Look Direction = deg315";
                break;
            default:
                pld = "Player Look Direction set incorrectly";
                break;
        }

        switch (playerMovementDirection) {
            case 0:
                pmd = "\nPlayer Movement Direction = IDLE";
                break;
            case 1:
                pmd = "\nPlayer Movement Direction = W";
                break;
            case 2:
                pmd = "\nPlayer Movement Direction = WD";
                break;
            case 3:
                pmd = "\nPlayer Movement Direction = D";
                break;
            case 4:
                pmd = "\nPlayer Movement Direction = SD";
                break;
            case 5:
                pmd = "\nPlayer Movement Direction = S";
                break;
            case 6:
                pmd = "\nPlayer Movement Direction = AS";
                break;
            case 7:
                pmd = "\nPlayer Movement Direction = A";
                break;
            case 8:
                pmd = "\nPlayer Movement Direction = AW";
                break;
            default:
                pmd = "\nPlayer Movment Direction set incorrectly";
                break;
        }

        switch (playerMovementAnimation) {
            case 0:
                pma = "\nPlayer Movement Animation = RUN_FORWARD";
                break;
            case 1:
                pma = "\nPlayer Movement Animation = FORWARD_STRAFE_RIGHT";
                break;
            case 2:
                pma = "\nPlayer Movement Animation = STRAFE_RIGHT";
                break;
            case 3:
                pma = "\nPlayer Movement Animation = BACKWARD_STRAFE_RIGHT";
                break;
            case 4:
                pma = "\nPlayer Movement Animation = BACKWARD";
                break;
            case 5:
                pma = "\nPlayer Movement Animation = BACKWARD_STRAFE_LEFT";
                break;
            case 6:
                pma = "\nPlayer Movement Animation = STRAFE_LEFT";
                break;
            case 7:
                pma = "\nPlayer Movement Animation = FORWARD_STRAFE_LEFT";
                break;
            default:
                pma = "\nPlayer Movement Animation set incorrectly";
                break;
        }

        Debug.Log("\n------------------------\nFull Animation Value Log:\n" + 
            pld + pmd + pma +
            "\n------------------------");
    }
}
