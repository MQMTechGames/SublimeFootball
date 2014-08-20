using UnityEngine;

public static class ParabolicShot
{
    public static Vector3 CalculateVelocity(float maxVDistance, Vector3 distToPoint)
    {
        float hDistance = Mathf.Sqrt( distToPoint.x * distToPoint.x + distToPoint.z * distToPoint.z );

        float g = Physics.gravity.magnitude;
        float vSpeed = Mathf.Sqrt(2f * g * maxVDistance);

        float totalTime = 2f * vSpeed / g;
        float hSpeed = hDistance / totalTime;

        distToPoint.y = 0f;
        return Vector3.up * vSpeed + distToPoint.normalized * hSpeed;
    }
}
