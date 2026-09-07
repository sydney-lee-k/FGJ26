using System;
using Pathfinding;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    public enum EnemyState
    {
        Chase,
        SoftChase,
        Patrol,
        Idle
    }

    public EnemyState state = EnemyState.Patrol;
    [SerializeField] private PatrolRoute currentRoute;
    [SerializeField] private int currentRouteWaypointIndex;
    [SerializeField] private float nextWaypointDistance;
    [SerializeField] private bool patrolForward;
    [SerializeField] private bool randomizePatrolDirection;
    private AIDestinationSetter destinationSetter;
    private PatrolController patrolController;

    private void Start()
    {
        destinationSetter = GetComponent<AIDestinationSetter>();
        patrolController = PatrolController.Instance;
        
        if (state == EnemyState.Patrol)
        {
            SeekPatrolPath(currentRoute ? currentRoute : null);
        }
    }

    private void Update()
    {
        switch (state)
        {
            case EnemyState.Chase:
                break;
            case EnemyState.SoftChase:
                break;
            case EnemyState.Patrol:
                Patrolling();
                break;
            case EnemyState.Idle:
                break;
        }
    }

    public void Patrolling()
    {
        if (Vector3.Distance(transform.position, destinationSetter.target.position) <= nextWaypointDistance)
        {
            if (patrolForward)
            {
                currentRouteWaypointIndex = (currentRouteWaypointIndex + 1) % currentRoute.waypoints.Count;
            }
            else
            {
                currentRouteWaypointIndex -= 1;
                if(currentRouteWaypointIndex < 0) currentRouteWaypointIndex = currentRoute.waypoints.Count - 1;
            }
            destinationSetter.target = currentRoute.waypoints[currentRouteWaypointIndex];
        }
    }
    
    public void SeekPatrolPath(PatrolRoute route = null)
    {
        if (!route)
        {
            int rng = Random.Range(0, patrolController.routes.Count - 1);
            route = patrolController.routes[rng];
            currentRoute = route;
        }
        if(randomizePatrolDirection) patrolForward = Random.Range(0,1) == 0;

        destinationSetter.target = currentRoute.ClosestWaypoint(transform);
        currentRouteWaypointIndex = currentRoute.waypoints.IndexOf(destinationSetter.target);
    }
}
