using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// version 0.1

public static class CoSephUtils
{
    // tag constants
    // layer constants


    public static bool RandomBool()
    {
        return (Random.Range(0, 2) == 1);
    }

    // bounds the input angle to -180...180
    public static float BoundAngle(float angle)
    {
        float angleOut;

        if (angle <= -180f) angleOut = angle + 360f;
        else if (angle > 180f) angleOut = angle - 360f;
        else angleOut = angle;

        return angleOut;
    }

#if UNITY_EDITOR
    // call this during OnDrawGizmos to show the bounds of a rect
    public static void DrawRectGizmo(Color rectColor, Rect rectangle)
    {
        Gizmos.color = rectColor;
        Gizmos.DrawLine(new Vector2(rectangle.xMin, rectangle.yMin), new Vector2(rectangle.xMin, rectangle.yMax));
        Gizmos.DrawLine(new Vector2(rectangle.xMax, rectangle.yMin), new Vector2(rectangle.xMax, rectangle.yMax));
        Gizmos.DrawLine(new Vector2(rectangle.xMin, rectangle.yMin), new Vector2(rectangle.xMax, rectangle.yMin));
        Gizmos.DrawLine(new Vector2(rectangle.xMin, rectangle.yMax), new Vector2(rectangle.xMax, rectangle.yMax));
    }
#endif
}
