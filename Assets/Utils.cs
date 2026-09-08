using UnityEngine;

public static class Vector3Utils
{
    public static bool IsWithinDistance(Vector3 object1, Vector3 object2, float distance)
    {
        return (object1 - object2).sqrMagnitude <= distance * distance;
    }
}

