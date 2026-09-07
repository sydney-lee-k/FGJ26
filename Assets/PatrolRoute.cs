using System.Collections.Generic;
using UnityEngine;

public class PatrolRoute : MonoBehaviour
{
    public List<Transform> waypoints = new List<Transform>();

    public Transform ClosestWaypoint(Transform seeker)
    {
        float? distance = null;
        Transform currentTarget = null;
        foreach (var waypoint in waypoints)
        {
            float currentDistance = Vector3.Distance(waypoint.position, seeker.position);

            if (distance == null || currentDistance < distance)
            {
                distance = currentDistance;
                currentTarget = waypoint;
            }
        }
        return currentTarget;
    }
}
