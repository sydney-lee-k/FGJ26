using System;
using System.Collections;
using Pathfinding;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    public enum EnemyState
    {
        Chase,
        Alert,
        LookAround,
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
    public bool alerted; //When alerted, will not return to patrol.

    [Header("Speed")]
    [SerializeField] private float chaseSpeed = 4;
    [SerializeField] private float alertSpeed = 4;
    [SerializeField] private float scoutSpeed = 4;
    [SerializeField] private float patrolSpeed = 4;
    
    [Header("Chase")]
    [SerializeField] private float chaseDistance;
    
    [Header("Look Around")]
    [SerializeField]private float minLookAroundTime;
    [SerializeField]private float maxLookAroundTime;
    private float remainingLookAroundTime;
    private float lookAroundTimer;
    private float lookAroundAngle;
    private float lookAroundStartingYRotation;

    [Header("Alerted")]
    [SerializeField]private float minTimeBeforeDirectionChange;
    [SerializeField]private float maxTimeBeforeDirectionChange;
    private float remainingTimeBeforeDirectionChange;
    
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
        remainingLookAroundTime = Random.Range(minLookAroundTime, maxLookAroundTime);
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
            case EnemyState.Alert:
                Alerted();
                break;
            case EnemyState.LookAround:
                LookAround();
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
            Vector3.Dot(eyeSpot.forward, directionToPlayer.normalized) >= Mathf.Cos(coneAngle * 0.5f * Mathf.Deg2Rad))
        {
            bool blocked = Physics.SphereCast(eyeSpot.position, 0.2f, directionToPlayer.normalized, out RaycastHit hit, directionToPlayer.magnitude, sightBlockLayers);
            if (!blocked)
            {
                state = EnemyState.Chase;
                alerted = true;
            }
            else if (state == EnemyState.Chase)
            {
                state = EnemyState.Scout;
            }
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
            aiPath.maxSpeed = chaseSpeed;
            aiPath.endReachedDistance = chaseDistance;
        }
    }
    
    private void Alerted()
    {
        if (previousState != state)
        {
            scoutSpot.transform.position = destinationSetter.target.position;
            destinationSetter.target = scoutSpot.transform;
            aiPath.maxSpeed = alertSpeed;
            aiPath.endReachedDistance = 1;
        }

        if (aiPath.reachedDestination)
        {
            remainingTimeBeforeDirectionChange = Random.Range(minTimeBeforeDirectionChange, maxTimeBeforeDirectionChange);
            state = EnemyState.LookAround;
            return;
        }

        if (remainingTimeBeforeDirectionChange <= 0)
        {
            Vector3 randomPosition = transform.position + Random.insideUnitSphere * 10f;
            NNInfo nearest = AstarPath.active.GetNearest(randomPosition);
            if (nearest.node != null && nearest.node.Walkable)
            {
                scoutSpot.transform.position = (Vector3)nearest.position;
                destinationSetter.target = scoutSpot.transform;
            }
            remainingTimeBeforeDirectionChange = Random.Range(minTimeBeforeDirectionChange, maxTimeBeforeDirectionChange);
        }
        else
        {
            remainingTimeBeforeDirectionChange -= Time.deltaTime;
        }
    }
    
    private void LookAround()
    {
        if (previousState != state)
        {
            aiPath.isStopped = true;
            aiPath.enableRotation = false;

            lookAroundAngle = Random.Range(60f, 120f);
            lookAroundTimer = 0f;
            lookAroundStartingYRotation = transform.eulerAngles.y;

            remainingLookAroundTime = Random.Range(minLookAroundTime, maxLookAroundTime);
        }

        if (remainingLookAroundTime <= 0f)
        {
            aiPath.enableRotation = true;
            aiPath.isStopped = false;

            state = alerted ? EnemyState.Alert : EnemyState.Patrol;
            return;
        }

        lookAroundTimer += Time.deltaTime;
        remainingLookAroundTime -= Time.deltaTime;

        float angle = Mathf.Sin(lookAroundTimer * 2f) * lookAroundAngle;

        transform.rotation = Quaternion.Euler(0f, lookAroundStartingYRotation + angle, 0f);
    }



    
    private void Scout()
    {
        if (previousState != state)
        {
            scoutSpot.transform.position = destinationSetter.target.position;
            destinationSetter.target = scoutSpot.transform;
            aiPath.maxSpeed = scoutSpeed;
            aiPath.endReachedDistance = 1;
        }
        
        if (aiPath.remainingDistance <= aiPath.endReachedDistance)
        {
            if (alerted)
            {
                state = EnemyState.Alert;
            }
            else //Return to patrol if not alerted. For example if looking into specific noise, but not
            {
                state = EnemyState.LookAround;
            }
        }
    }

    public void GoCheck(Vector3 position, bool alert = false)
    {
        //Implement panic. Essentially idea is if panic is true, dont return to patrol but do same panic as if they had spotted and then lost the player.
        scoutSpot.transform.position = position;
        destinationSetter.target = scoutSpot.transform;
        state = EnemyState.Scout;
        if (!alerted) alerted = alert;
    }

    private void Patrolling()
    {
        if (previousState != state)
        {
            SeekPatrolPath(currentRoute ? currentRoute : null);
            aiPath.maxSpeed = patrolSpeed;
            aiPath.endReachedDistance = 1;
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
        if(randomizePatrolDirection) patrolForward = Random.Range(0,2) == 0;

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
            case EnemyState.LookAround:
                Handles.color = new Color(1, 0.33f, 0, 0.2f);
                break;
            case EnemyState.Alert:
                Handles.color = new Color(1, 0.33f, 0, 0.2f);
                break;
            case EnemyState.Scout:
                Handles.color = new Color(1, 0.66f, 0, 0.2f);
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
