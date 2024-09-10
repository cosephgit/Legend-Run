using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// version 0.1
// last update 29/8/23

public static class CoSephUtils
{
    // these methods check for components ONLY in the children of an object
    public static T[] GetComponentsOnlyInChildren<T>(this MonoBehaviour script) where T : class
    {
        List<T> group = new List<T>();

        //collect only if its an interface or a Component
        if (typeof(T).IsInterface
        || typeof(T).IsSubclassOf(typeof(Component))
        || typeof(T) == typeof(Component))
        {
            foreach (Transform child in script.transform)
            {
                group.AddRange(child.GetComponentsInChildren<T>());
            }
        }

        return group.ToArray();
    }

    public static T GetComponentOnlyInChildren<T>(this MonoBehaviour script) where T : class
    {
        T result;

        //collect only if its an interface or a Component
        if (typeof(T).IsInterface
        || typeof(T).IsSubclassOf(typeof(Component))
        || typeof(T) == typeof(Component))
        {
            foreach (Transform child in script.transform)
            {
                result = child.GetComponentInChildren<T>();
                if (result != null) return result;
            }
        }

        return null;
    }

    // simple coin flip
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

    public static float VolumeToDecibels(float volume)
    {
        // convert the slider value (0...1) value to a decibel value (-80...0)
        if (volume <= 0.0001)
        {
            return -80;
        }
        else
        {
            return Mathf.Log10(volume) * 20;
        }
    }
    public static float DecibelsToVolume(float decibels)
    {
        // convert the decibel value (-80...0) value to a slider value (0...1)
        if (decibels <= -80)
        {
            return 0;
        }
        else
        {
            return Mathf.Pow(10, (decibels / 20f));
        }
    }

    // convert the 0...1 progress value into an s-shaped progress curve
    public static float CalcSCurve(float value)
    {
        float curve = value - 0.5f; // -0.5...0.5
        curve = curve / Mathf.Sqrt(1 + (curve * curve)); // -(0.5/sqrt(1.25))...(0.5/sqrt(1.25))
        curve *= Mathf.Sqrt(1.25f); // -0.5...0.5
        curve += 0.5f; // 0...1
        return curve;
    }

    public static float ClampAngle(float angleBase)
    {
        float angleClamped = angleBase;

        while (angleClamped > 180f) angleClamped -= 360f;
        while (angleClamped < -180f) angleClamped += 360f;

        return angleClamped;
    }
    public static Vector3 ClampAngleAll(Vector3 eulers)
    {
        Vector3 eulersClamped = eulers;
        eulersClamped.x = ClampAngle(eulersClamped.x);
        eulersClamped.y = ClampAngle(eulersClamped.y);
        eulersClamped.z = ClampAngle(eulersClamped.z);
        return eulersClamped;
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
