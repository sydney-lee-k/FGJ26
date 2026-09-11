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
        Scout,
        Patrol,
        Idle
    }
    
    public enum AlertState
    {
        hasPath,
        lookingForPath
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
    private bool chaseAlert;

    [Header("Look Around")]
    [SerializeField] private float lookAroundSpeed = 0.5f;
    [SerializeField] private float minLookAroundAngle = 60;
    [SerializeField] private float maxLookAroundAngle = 90;
    [SerializeField]private int minLookAroundCount = 2;
    [SerializeField]private int maxLookAroundCount = 4;
    private float lookAroundTimer;
    private float lookAroundStartingYRotation;
    private Coroutine lookAroundCoroutine;
    private float timeBeforeCanLook;


    [Header("Alerted")]
    [SerializeField][Range(0,100)] private float lookAroundChance;
    
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
            else if (state == EnemyState.Chase)
            {
                chaseAlert = true;
                state = EnemyState.Scout;
            }
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
        Vector2 randomDirection = Random.onUnitCircle;
        Vector3 randomPosition;
        if (chaseAlert)
        {
            chaseAlert = false;

            float rng = Random.Range(10f, 20f);
            randomPosition = transform.position + transform.forward * rng + new Vector3(randomDirection.x, 0f, randomDirection.y) * rng;
        }
        else
        {
            Debug.Log("Called");
            randomPosition = transform.position + new Vector3(randomDirection.x, 0f, randomDirection.y) * Random.Range(10f, 20f);
        }
        
        GoCheck(randomPosition);
    }



    private IEnumerator LookAround()
    {
        state = EnemyState.Idle;
        float elapsed = 0f;

        aiPath.isStopped = true;
        aiPath.enableRotation = false;

        int lookCount = Random.Range(minLookAroundCount, maxLookAroundCount+1);
        bool startDir = Random.value < 0.5f;
        float lookAroundAngle = Random.Range(minLookAroundAngle, maxLookAroundAngle);
        lookAroundStartingYRotation = transform.eulerAngles.y;

        while (lookCount > 0)
        {
            float angle = Mathf.Sin(elapsed * lookAroundSpeed) * lookAroundAngle;
            transform.rotation = Quaternion.Euler(0f, lookAroundStartingYRotation + (startDir ? angle : -angle), 0f);
            float previousAngle = Mathf.Sin((elapsed - Time.deltaTime) * lookAroundSpeed);
            if (previousAngle * Mathf.Sin(elapsed * lookAroundSpeed) < 0f)
            {
                lookCount--;
            }

            elapsed += Time.deltaTime;

            yield return null;
        }

        lookAroundCoroutine = null;
        aiPath.isStopped = false;
        aiPath.enableRotation = true;

        if (alerted) state = EnemyState.Scout;
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
                Vector2 randomDirection = Random.onUnitCircle;
                Vector3 randomPosition;
                if (chaseAlert)
                {
                    chaseAlert = false;
                    float rng = Random.Range(10f, 20f);
                    randomPosition = transform.position + transform.forward * rng + new Vector3(randomDirection.x, 0f, randomDirection.y) * rng;
                }
                else
                {
                    float rng = Random.Range(10f, 20f);
                    randomPosition = transform.position + transform.forward * rng/4 + new Vector3(randomDirection.x, 0f, randomDirection.y) * Random.Range(10f, 20f);
                }
        
                GoCheck(randomPosition);
                float lookAroundRng = Random.Range(0, 100);
                if (!chaseAlert && 100 - lookAroundChance <= lookAroundRng)
                {
                    if (lookAroundCoroutine == null && timeBeforeCanLook <= 0) lookAroundCoroutine = StartCoroutine(LookAround());
                }
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
        Gizmos.DrawWireSphere(transform.forward * 10f, 10);
        
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
