using UnityEngine;

public static class Vector3Utils
{
    public static bool IsWithinDistance(Vector3 object1, Vector3 object2, float distance)
    {
        return (object1 - object2).sqrMagnitude <= distance * distance;
    }
}

public static class LayerUtils
{
    public static bool Contains(LayerMask mask, Component target)
    {
        return (mask.value & (1 << target.gameObject.layer)) != 0;
    }
}

