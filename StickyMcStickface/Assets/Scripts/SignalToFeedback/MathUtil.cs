using UnityEngine;

public struct MathUtil
{
    public static float SignedAngle(Vector3 from, Vector3 to)
    {
        Vector3 xzFrom = new Vector3(from.x, 0.0f, from.z);
        Vector3 xzTo = new Vector3(to.x, 0.0f, to.z);
        float angle = Vector3.Angle(xzFrom, xzTo);
        if (Vector3.Cross(xzFrom, xzTo).y < 0)
            return -angle;
        return angle;
    }
}