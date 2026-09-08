using System;
using Pathfinding;
using UnityEditor;
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

    [Header("Detection")]
    [SerializeField] private Transform eyeSpot;
    [SerializeField] private float detectionRange;
    [SerializeField] private float coneAngle;
    [SerializeField] private LayerMask sightBlockLayers;
    
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
        Detection();
        
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

    private void Detection()
    {
        Vector3 directionToPlayer = (player.transform.position - transform.position);

        //First see if player is even within detection range, then see if they're in the cone, and finally check if you can actually raycast to them. If you can: Chase
        if (Vector3Utils.IsWithinDistance(transform.position, player.transform.position, detectionRange) &&
            Vector3.Dot(eyeSpot.forward, directionToPlayer.normalized) >= Mathf.Cos(coneAngle * 0.5f * Mathf.Deg2Rad) &&
            !Physics.Raycast(eyeSpot.position, directionToPlayer, directionToPlayer.magnitude, sightBlockLayers))
        {
            state = EnemyState.Chase;
        }
        else if (state == EnemyState.Chase)
        {
            state = EnemyState.Scout;
        }
    }
    
    private void Chase()
    {
        if (previousState != state)
        {
            destinationSetter.target = player.transform;
        }
    }
    
    private void Scout()
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

    private void Patrolling()
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

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        switch (state)
        {
            case EnemyState.Chase:
                Handles.color = new Color(1, 0, 0, 0.2f);
                break;
            case EnemyState.Scout:
                Handles.color = new Color(1, 0.5f, 0, 0.2f);
                break;
            case EnemyState.Patrol:
                Handles.color = new Color(1, 1, 0, 0.2f);
                break;
            case EnemyState.Idle:
                Handles.color = new Color(0.5f, 0.5f, 0.5f, 0.2f);
                break;
        }   

        //Cone
        Handles.DrawSolidArc(eyeSpot.position, Vector3.up, Quaternion.Euler(0f, -coneAngle * 0.5f, 0f) * eyeSpot.forward, coneAngle, detectionRange);

        //Looking at player
        if (state == EnemyState.Chase)
        {
            Handles.color = Color.green;
            Handles.DrawLine(eyeSpot.position, player.position);
        }
        else
        {
            Handles.color = Color.green;
            Handles.DrawLine(eyeSpot.position, eyeSpot.position + transform.forward * detectionRange);
        }
    }
#endif


}
