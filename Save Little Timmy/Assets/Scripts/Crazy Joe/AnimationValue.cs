using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AnimationValue
{
    // Player movement and look direction animation point values
    // animationMovementDirection, animationLookDirection
    public static int IDLE = 0;
    public static int W, deg0 = 1;
    public static int WD, deg45 = 2;
    public static int D, deg90 = 3;
    public static int SD, deg135 = 4;
    // 180 and -180
    public static int S, deg180 = 5;
    public static int AS, deg225 = 6;
    public static int A, deg270 = 7;
    public static int AW, deg315 = 8;

    // Player movement animation
    public static int RUN_FORWARD = 0;
    public static int FORWARD_STRAFE_RIGHT = 1;
    public static int STRAFE_RIGHT = 2;
    public static int BACKWARD_STRAFE_RIGHT = 3;
    public static int BACKWARD = 4;
    public static int BACKWARD_STRAFE_LEFT = 5;
    public static int STRAFE_LEFT = 6;
    public static int FORWARD_STRAFE_LEFT = 7;
}
