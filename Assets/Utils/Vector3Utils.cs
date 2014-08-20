using UnityEngine;
using System.Collections;

public static class Vector3Utils
{
    public static Vector2 ToDirXZ(Vector3 a, Vector3 b)
    {
        return new Vector2(a.x - b.x, a.z - b.z);
    }

    public static float MagnitudeXZ(Vector3 a, Vector3 b)
    {
        return new Vector2(a.x - b.x, a.z - b.z).magnitude;
    }
}
