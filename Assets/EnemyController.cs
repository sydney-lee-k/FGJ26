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
    [SerializeField] private float lookAroundSpeed = 0.5f;
    [SerializeField] private float minLookAroundAngle = 60;
    [SerializeField] private float maxLookAroundAngle = 90;
    [SerializeField]private float minLookAroundTime;
    [SerializeField]private float maxLookAroundTime;
    private float lookAroundTimer;
    private float lookAroundStartingYRotation;
    private Coroutine lookAroundCoroutine;
    private float timeBeforeCanLook;


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
            case EnemyState.Scout:
                Scout();
                break;
            case EnemyState.Patrol:
                Patrolling();
                break;
            case EnemyState.Idle:
                break;
        }
        
        if(state != previousState) timeBeforeCanLook = 1; //Whenever state changes, it gives a 1 sec cooldown before it can be told to look around.
        if(timeBeforeCanLook > 0) timeBeforeCanLook -= Time.deltaTime;
        previousState = state;
    }

    private void Detection()
    {
        Vector3 directionToPlayer = player.position - eyeSpot.position;
        Vector3 direction = directionToPlayer.normalized;

        if (Vector3Utils.IsWithinDistance(eyeSpot.position, player.position, detectionRange) && Vector3.Dot(eyeSpot.forward, direction) >= Mathf.Cos(coneAngle * 0.5f * Mathf.Deg2Rad))
        {
            Vector3 castOrigin = eyeSpot.position;

            if (Physics.CheckSphere(castOrigin, 0.1f, sightBlockLayers, QueryTriggerInteraction.Ignore))
            {
                //The head is clipping with the wall, don't spot player
                return;
            }
            bool blocked = Physics.SphereCast(castOrigin, 0.2f, direction, out RaycastHit hit, directionToPlayer.magnitude, sightBlockLayers);

            if (!blocked)
            {
                state = EnemyState.Chase;
                alerted = true;
            }
            else if (state == EnemyState.Chase) state = EnemyState.Scout;
        }
        else if (state == EnemyState.Chase) state = EnemyState.Scout;
    }
    
    private void Chase()
    {
        if (previousState != state)
        {
            destinationSetter.target = player.transform;
            aiPath.maxSpeed = chaseSpeed;
            aiPath.endReachedDistance = chaseDistance;
        }

        if (aiPath.reachedDestination)
        {
            Vector3 directionToPlayer = player.position - transform.position;
            directionToPlayer.y = 0f;

            if (directionToPlayer.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRotation,
                    aiPath.rotationSpeed * Time.deltaTime
                );
            }
        }
    }
    
    private void Alerted()
    {
        if (previousState != state)
        {
            aiPath.maxSpeed = alertSpeed;
            aiPath.endReachedDistance = 1;

            remainingTimeBeforeDirectionChange = 0f;
        }

        if (remainingTimeBeforeDirectionChange <= 0)
        {
            Vector2 randomDirection = Random.onUnitCircle;
            Vector3 randomPosition = transform.position + new Vector3(randomDirection.x, 0f, randomDirection.y) * Random.Range(10, 20);
            NNInfo nearest = AstarPath.active.GetNearest(randomPosition);

            if (nearest.node != null && nearest.node.Walkable)
            {
                scoutSpot.transform.position = nearest.position;
                destinationSetter.target = scoutSpot.transform;
            }

            remainingTimeBeforeDirectionChange = Random.Range(minTimeBeforeDirectionChange, maxTimeBeforeDirectionChange);
            return;
        }
        remainingTimeBeforeDirectionChange -= Time.deltaTime;

        if (previousState == state && aiPath.reachedDestination)
        {
            remainingTimeBeforeDirectionChange = 0f;

            if (lookAroundCoroutine == null && timeBeforeCanLook <= 0) lookAroundCoroutine = StartCoroutine(LookAround());
        }
    }

    private IEnumerator LookAround()
    {
        state = EnemyState.Idle;
        float elapsed = 0f;

        aiPath.isStopped = true;
        aiPath.enableRotation = false;

        float time = Random.Range(minLookAroundTime, maxLookAroundTime);
        float lookAroundAngle = Random.Range(minLookAroundAngle, maxLookAroundAngle);
        lookAroundStartingYRotation = transform.eulerAngles.y;

        while (elapsed < time)
        {
            float angle = Mathf.Sin(elapsed * lookAroundSpeed) * lookAroundAngle;

            transform.rotation = Quaternion.Euler(0f, lookAroundStartingYRotation + angle, 0f);

            elapsed += Time.deltaTime;

            yield return null;
        }

        lookAroundCoroutine = null;
        aiPath.isStopped = false;
        aiPath.enableRotation = true;

        if (alerted) state = EnemyState.Alert;
        else state = EnemyState.Patrol;
        
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
            else
            {
                if (lookAroundCoroutine == null && timeBeforeCanLook <= 0) lookAroundCoroutine = StartCoroutine(LookAround());
            }
        }
    }

    public void GoCheck(Vector3 position, bool alert = false)
    {
        //Stop looking around, go check right away
        if (lookAroundCoroutine != null)
        {
            StopCoroutine(lookAroundCoroutine);
            lookAroundCoroutine = null;
            aiPath.isStopped = false;
            aiPath.enableRotation = true;
        }
        
        //Implement panic. Essentially idea is if panic is true, dont return to patrol but do same panic as if they had spotted and then lost the player.
        scoutSpot.transform.position = position;
        destinationSetter.target = scoutSpot.transform;
        if (!alerted) alerted = alert;
        state = EnemyState.Scout;
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
