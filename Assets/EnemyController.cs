using System;
using Pathfinding;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    public enum EnemyState
    {
        Chase,
        Scout,
        Patrol,
        Idle
    }

    [Header("State Management")]
    public EnemyState state = EnemyState.Patrol;
    
    [Header("Patrolling")]
    [SerializeField] private PatrolRoute currentRoute;
    [SerializeField] private int currentRouteWaypointIndex;
    [SerializeField] private bool patrolForward;
    [SerializeField] private bool randomizePatrolDirection;

    [Header("Scout")]
    [SerializeField] private GameObject scoutSpot;

    [Header("Pathfinding")]
    private AIDestinationSetter destinationSetter;
    private PatrolController patrolController;
    private EnemyState previousState;
    private Transform player;
    private AIPath aiPath;
    private void Start()
    {
        destinationSetter = GetComponent<AIDestinationSetter>();
        patrolController = PatrolController.Instance;
        scoutSpot = Instantiate(new GameObject(), transform.position, Quaternion.identity);
        scoutSpot.name = "ScoutSpot";
        player = GameObject.FindGameObjectWithTag("Player").transform;
        aiPath = GetComponent<AIPath>();
        
        if (state == EnemyState.Patrol)
        {
            SeekPatrolPath(currentRoute ? currentRoute : null);
        }
        previousState = state;
    }

    private void Update()
    {
        switch (state)
        {
            case EnemyState.Chase:
                Chase();
                break;
            case EnemyState.Scout:
                Scout();
                break;
            case EnemyState.Patrol:
                Patrolling();
                break;
            case EnemyState.Idle:
                break;
        }
        
        previousState = state;
    }
    
    public void Chase()
    {
        if (previousState != state)
        {
            destinationSetter.target = player.transform;
        }
    }
    
    public void Scout()
    {
        if (previousState != state)
        {
            scoutSpot.transform.position = destinationSetter.target.position;
            destinationSetter.target = scoutSpot.transform;
        }
        
        if (aiPath.remainingDistance <= aiPath.endReachedDistance)
        {
            state = EnemyState.Patrol;
            SeekPatrolPath(currentRoute);
        }
    }

    public void Patrolling()
    {
        if (previousState != state)
        {
            SeekPatrolPath(currentRoute ? currentRoute : null);
        }

        if (aiPath.remainingDistance <= aiPath.endReachedDistance)
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
