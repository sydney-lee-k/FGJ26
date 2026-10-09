using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FieldOfView : MonoBehaviour
{
    [Header("View Area Settings")]
    [Range(0, 360)] public float viewRadius = 10;
    public float viewAngle = 90;
    [SerializeField] private float detectionOffset = 1f;
    [SerializeField] private float detectionTimer = 0.2f;
    private float curDetectionTimer;

    [Header("Layers")]
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private LayerMask obstacleMask;

    [Header("View Mesh Settings")]
    [SerializeField] private float meshResolution;
    [SerializeField] private int edgeResolveIterations;
    [SerializeField] private float edgeDistanceThreshold;
    [SerializeField] private MeshFilter viewMeshFilter;

    //[HideInInspector]
    public List<Transform> visibleTargets = new();

    private Mesh viewMesh;

    // Increase targetBuffer if more than 64 target colliders can exist inside the viewRadius.
    private readonly List<Vector3> viewPoints = new();
    private readonly Collider[] targetBuffer = new Collider[64];

    private void Start()
    {
        viewMesh = new Mesh
        {
            name = "View Mesh"
        };

        viewMesh.MarkDynamic();
        viewMeshFilter.mesh = viewMesh;
        curDetectionTimer = detectionTimer;
    }

    private void Update()
    {
        if (curDetectionTimer > 0)
        {
            curDetectionTimer -= Time.deltaTime;
        }
        else
        {
            curDetectionTimer = detectionTimer;
            FindVisibleTargets();
        }
    }

    private void LateUpdate()
    {
        DrawFieldOfView();
    }
    
    private void FindVisibleTargets()
    {
        visibleTargets.Clear();
        int targetCount = Physics.OverlapSphereNonAlloc(transform.position, viewRadius, targetBuffer, targetMask);

        for (int i = 0; i < targetCount; i++)
        {
            Transform target = targetBuffer[i].transform;

            Vector3 directionToTarget = (target.position + new Vector3(0, detectionOffset, 0)) - transform.position;
            Debug.DrawRay(transform.position, directionToTarget, Color.red);
            float distanceToTarget = directionToTarget.magnitude;

            if (distanceToTarget <= 0f) continue;

            directionToTarget /= distanceToTarget;
            
            if (Vector3.Dot(transform.forward, directionToTarget) > Mathf.Cos(viewAngle * 0.5f * Mathf.Deg2Rad))
            {
                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask)) visibleTargets.Add(target);
            }
        }
    }

    private void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);

        if (stepCount <= 0) return;

        float stepAngleSize = viewAngle / stepCount;

        viewPoints.Clear();
        ViewCastInfo oldViewCast = default;

        // Go through each ray to determine how far the view can extend before hitting the environment.
        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - viewAngle * 0.5f + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);

            if (i > 0)
            {
                bool edgeThresholdExceeded = Mathf.Abs(oldViewCast.distance - newViewCast.distance) > edgeDistanceThreshold;

                // When adjacent rays give considerably different results, refine the
                // boundary between them to better follow corners.
                if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeThresholdExceeded))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if (edge.pointA != Vector3.zero)
                        viewPoints.Add(edge.pointA);

                    if (edge.pointB != Vector3.zero)
                        viewPoints.Add(edge.pointB);
                }
            }

            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }

        int vertexCount = viewPoints.Count + 1;

        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;

        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        viewMesh.Clear();

        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
    }

    private EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;

        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        // Narrow down the boundary between the two view casts.
        for (int i = 0; i < edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);

            bool edgeThresholdExceeded = Mathf.Abs(minViewCast.distance - newViewCast.distance) > edgeDistanceThreshold;
            if (newViewCast.hit == minViewCast.hit && !edgeThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }


    private ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 direction = DirectionFromAngle(globalAngle, true);

        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, viewRadius, obstacleMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + direction * viewRadius, viewRadius, globalAngle);
        }    
    }

    public Vector3 DirectionFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
            angleInDegrees += transform.eulerAngles.y;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float distance;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _distance, float _angle)
        {
            hit = _hit;
            point = _point;
            distance = _distance;
            angle = _angle;
        }
    }

    public struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }
}